using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scaffold.Stateful
{

    public abstract class StatefulBehaviour<TState> : MonoBehaviour, IStatefulBehaviour where TState : IState
    {
        [SerializeReference, HideInInspector]
        public TState CurrentState;
        [SerializeField, SerializeReference]
        protected List<TState> States = new List<TState>();

        public event Action<TState, TState> StateTransition = delegate { };
        public event Action<IState> StateChanged = delegate { };

        public virtual StateStrategy Strategy => StateStrategy.Unique;

        private void Start()
        {
            if (CurrentState != null)
            {
                TryIn();
            }
            EvaluateCurrentState();
        }

        private void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Tick();
            }
        }

        public virtual void ChangeState(TState state)
        {
            if (CurrentState != null && CurrentState.Equals(state))
            {
                return;
            }

            TState oldState = CurrentState;
            TryOut();
            CurrentState = state;
            TryIn();

            StateChanged?.Invoke(CurrentState);
            StateTransition?.Invoke(oldState, CurrentState);
        }

        private void TryIn()
        {
            try
            {
                CurrentState?.In();
            }
            catch
            {
                Debug.LogWarning($"Something went wrong while <b>ENTERING</b> state {CurrentState.StateName} - of stateful {name}.");
            }
        }

        private void TryOut()
        {
            try
            {
                CurrentState?.Out();
            }
            catch
            {
                Debug.LogWarning($"Something went wrong while <b>LEAVING</b> state {CurrentState.StateName} - of stateful {name}.");
            }
        }

        public void ChangeState<T>() where T : TState
        {
            TState targetState = States.FirstOrDefault(state => state.GetType() == typeof(T));
            if (targetState != null)
            {
                ChangeState(targetState);
                return;
            }

            Debug.Log($"{name} statefulBehaviour does not have a state of type {typeof(T)}");
            return;
        }

        public virtual void EvaluateCurrentState()
        {
            foreach (var state in States)
            {
                if (state.Evaluate())
                {
                    ChangeState(state);
                    return;
                }
            }

            if (CurrentState == null && States.Count > 0)
            {
                ChangeState(States[0]);
            }
        }

        public IState GetCurrentState()
        {
            return CurrentState;
        }

        protected virtual void FillStateList()
        {
            States = StateFactory.BuildStates<TState>(this, States);
        }

        public IEnumerable<IState> BuildStates()
        {
            States.RemoveAll(s => s == null);
            FillStateList();
            if(CurrentState == null || !States.Contains(CurrentState))
            {
                //we only evaluate if the current state is invalid, to avoid overriding editor changes
                EvaluateCurrentState();
            }
            return States.OfType<IState>();
        }

        public void ChangeState(IState state)
        {
            if (state is not TState validState)
            {
                Debug.LogError($"Trying to change {GetType().ReadableName()} - {name} into invalid state {state.GetType().ReadableName()}");
                return;
            }
            ChangeState(validState);
        }


        public virtual void AddState(IState state)
        {
            if (state is not TState tState)
            {
                throw new Exception("Trying to add state of wrong type");
            }

            States.Add(tState);
            EvaluateCurrentState();
        }

        public virtual void RemoveState(IState state)
        {
            if (state is not TState tState)
            {
                throw new Exception("Trying to remove state of wrong type");
            }

            States.Remove(tState);
            if (state.Equals(CurrentState))
            {
                CurrentState = default(TState);
                EvaluateCurrentState();
            }
        }
    }
}
