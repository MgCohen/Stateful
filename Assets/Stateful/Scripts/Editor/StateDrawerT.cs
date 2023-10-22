using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using System;

namespace Scaffold.Stateful
{
    public abstract class StateDrawer<TState> : IStateDrawer where TState : class, IState
    {
        public StateDrawer(int index, SerializedProperty stateProp, IStatefulBehaviour stateful)
        {
            this.PropertyIndex = index;
            this.Stateful = stateful;
            this.StateProp = stateProp;
            this.State = StateProp.boxedValue as TState;
            this.Anim = new AnimBool(false);
            Anim.valueChanged.AddListener(Animate);
        }

        #region Variables
        public int PropertyIndex { get; protected set; }
        public IStatefulBehaviour Stateful { get; protected set; }
        public TState State { get; protected set; }
        public string Name { get; protected set; }
        public SerializedProperty StateProp { get; protected set; }
        public AnimBool Anim { get; protected set; }
        #endregion

        #region State Access

        public event Action StateAnimation = delegate { };

        
        public void UpdateStateProp(int index, SerializedProperty stateProp)
        {
            StateProp = stateProp;
            PropertyIndex = index;
        }

        public IState GetState()
        {
            return State;
        }

        public virtual string GetStateName()
        {
            return State.StateName;
        }
        #endregion

        #region Drawing
        public void DrawState()
        {
            var rect = EditorGUILayout.BeginVertical(StatefulStyles.StateStyle);
            DrawHeader();
            using (var scope = new EditorGUILayout.FadeGroupScope(Anim.faded))
            {
                if (scope.visible)
                {
                    DrawBody();
                }
            }

            //this is the background click, its invisible and draw AFTER the icon button, due to unity weird rendering stack
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                Anim.target = !Anim.target;
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            bool selected = Stateful.GetCurrentState().Equals(State);
            LedStyle led = selected ? LedStyle.green : LedStyle.off;
            if (StatefulLayout.LedButton(led) && !selected)
            {
                Stateful.ChangeState(State);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                Undo.RecordObject(Stateful as MonoBehaviour, $"State Change to {State}");
                PrefabUtility.RecordPrefabInstancePropertyModifications(Stateful as MonoBehaviour);
            }

            StatefulLayout.AllignedLabel(GetStateName(), TextAnchor.MiddleLeft);

            if (Stateful.Strategy == StateStrategy.Variable)
            {
                GUILayout.FlexibleSpace();
                if (StatefulLayout.IconButton("winbtn_graph_close_h", 16))
                {
                    DisposeDrawer();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawBody()
        {
            if (this == null || StateProp == null)
            {
                return;
            }

            StatefulLayout.Divider();
            EditorGUILayout.Space(2);

            List<SerializedProperty> properties = StateProp.GetDirectChildren().ToList();
            foreach (var prop in properties)
            {
                EditorGUILayout.PropertyField(prop);
            }

            StateProp.serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space(2);
        }
        #endregion

        private void Animate()
        {
            StateAnimation?.Invoke();
        }

        private void DisposeDrawer()
        {
            Stateful.RemoveState(State);
            StateProp = null;
            State = default(TState);
            Stateful = null;
            Anim.valueChanged.RemoveAllListeners();
        }
    }
}