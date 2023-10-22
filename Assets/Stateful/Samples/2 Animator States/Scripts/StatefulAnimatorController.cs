using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.Animations;
using UnityEngine;

namespace Scaffold.Stateful.Samples
{
    public class StatefulAnimatorController : StatefulBehaviour<StatefulAnimatorController.AnimatorState>
    {
        [SerializeField] private Animator animator;

        protected override void FillStateList()
        {
            if (animator == null)
            {
                States.Clear();
                return;
            }
            try
            {
#if UNITY_EDITOR
                RuntimeAnimatorController animatorController = animator.runtimeAnimatorController;
                AnimatorControllerLayer[] layers = (animatorController as AnimatorController).layers;
                IEnumerable<AnimatorStateMachine> stateMachines = layers.Select(l => l.stateMachine);
                IEnumerable<ChildAnimatorState> animStates = stateMachines.SelectMany(sm => sm.states);
                States.Clear(); //as our states are managed by the animator, and we are just exposing, we can just clear to avoid reference problems
                foreach (var state in animStates)
                {
                    States.Add(new AnimatorState(state.state.name, state.state.motion, this));
                }
#endif
            }
            catch { }
        }

        public override void EvaluateCurrentState()
        {
            if (animator == null) return;


            AnimatorStateInfo activeAnimatorState = animator.GetCurrentAnimatorStateInfo(0);
            foreach (AnimatorState state in States)
            {
                if (activeAnimatorState.IsName(state.StateName))
                {
                    ChangeState(state);
                    return;
                }
            }
            base.EvaluateCurrentState();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                EvaluateCurrentState();
            }
        }
        private void OnValidate()
        {
            BuildStates();
        }
#endif

        [Serializable]
        public class AnimatorState : State<StatefulAnimatorController>
        {
            //avoid storing direct reference to the animator AnimatorState, as its editor only
            public AnimatorState(string stateName, Motion motion, StatefulAnimatorController component)
            {
                this.stateName = stateName;
                this.motion = motion;
                this.component = component;
            }

            public override string StateName => stateName;
            [SerializeField] private string stateName;

            public Motion Motion => motion;
            [SerializeField] private Motion motion;

            public override void In()
            {
                Animator animator = component.animator;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    RuntimeAnimatorController animatorController = animator.runtimeAnimatorController;
                    AnimatorControllerLayer[] layers = (animatorController as AnimatorController).layers;
                    AnimatorStateMachine stateMachine = layers[0].stateMachine;
                    ChildAnimatorState matchingState = stateMachine.states.FirstOrDefault(cas => cas.state.name == StateName);
                    if (matchingState.state != null)
                    {
                        stateMachine.defaultState = matchingState.state;
                        UnityEditor.EditorUtility.SetDirty(animatorController);
                    }
                    return;
                }
#endif
                animator.Play(StateName);
            }

            public override bool Equals(IState other)
            {
                if (other is not AnimatorState state)
                {
                    return false;
                }
                return state.stateName == stateName;
            }
        }
    }
}
