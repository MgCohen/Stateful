using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;


namespace Scaffold.Stateful
{

    [CustomEditor(typeof(StatefulBehaviour<>), editorForChildClasses: true)]
    public class StatefulBehaviourEditor : Editor
    {
        public SerializedProperty StatesProp { get; protected set; }
        public SerializedProperty CurrentStateProp { get; protected set; }

        protected IStatefulBehaviour stateful;
        protected List<IStateDrawer> stateDrawers = new List<IStateDrawer>();
        protected IStateDrawer selectedStateDrawer;

        private int typeToCreateIndex = 0;
        private List<Type> typesToCreate;
        private string[] typeNames;

        protected virtual string[] PropertiesToIgnore => new string[]
        {
            "m_Script",
            "States",
            "CurrentState"
        };
        private string[] propertiesToIgnore;

        protected virtual void OnEnable()
        {
            if (target == null) return;

            StatesProp = serializedObject.FindProperty("States");
            CurrentStateProp = serializedObject.FindProperty("CurrentState");
            stateful = target as IStatefulBehaviour;
            if (stateful.Strategy == StateStrategy.Variable)
            {
                typesToCreate = StateFactory.GetStateTypes(stateful);
                typeNames = typesToCreate.Select(t => t.ReadableName()).ToArray();
            }
            PopulateStateInfo();
            propertiesToIgnore = PropertiesToIgnore;
        }

        protected virtual void PopulateStateInfo()
        {
            if (StateDrawerContainer.instance.GetDrawers(stateful, out stateDrawers)) //if its a new list, lets also build states as its the first time we are openning this behaviour
            {
                stateful.BuildStates();
            }
            RefreshStateInfo();
        }

        #region Drawing
        public override void OnInspectorGUI()
        {
            if (StatesProp.arraySize != stateDrawers.Count)
            {
                PopulateStateInfo();
            }

            EditorGUILayout.Space(3);
            DrawStates();
            DrawDefaultProperties();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawDefaultProperties()
        {
            DrawPropertiesExcluding(serializedObject, propertiesToIgnore);
        }

        protected virtual void DrawStates()
        {
            EditorGUILayout.BeginVertical(StatefulStyles.Deep);

            DrawStateList();
            DrawStateControls();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        protected virtual void DrawStateList()
        {
            if (StatesProp.arraySize > 0)
            {
                for (int i = 0; i < stateDrawers.Count; i++)
                {
                    stateDrawers[i].DrawState();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Stateful component without any defined states", MessageType.Warning);
            }
        }

        protected virtual void DrawStateControls()
        {
            if (stateful.Strategy != StateStrategy.Variable)
            {
                return;
            }

            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();

            if (typesToCreate.Count > 1)
            {
                typeToCreateIndex = EditorGUILayout.Popup(typeToCreateIndex, typeNames);
            }
            else
            {
                GUILayout.FlexibleSpace();
            }

            if (StatefulLayout.IconButton("Toolbar Plus More", "Add new state", 16))
            {
                IState newState = StateFactory.BuildState(typesToCreate[typeToCreateIndex], stateful);
                AddState(newState);
            }

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region State Management

        protected void RefreshStateInfo()
        {
            stateDrawers.RemoveAll(state => state.StateProp == null);
            for (int i = 0; i < StatesProp.arraySize; i++)
            {
                SerializedProperty childProp = StatesProp.GetArrayElementAtIndex(i);
                IState state = childProp.boxedValue as IState;
                IStateDrawer drawer = stateDrawers.FirstOrDefault(st => st.GetState() == state);
                if (drawer != null)
                {
                    stateDrawers.Remove(drawer);
                    stateDrawers.Insert(i, drawer);
                    drawer.UpdateStateProp(i, childProp);
                }
                else
                {
                    CreateStateDrawer(i, childProp);
                }
            }
            stateDrawers.RemoveRange(StatesProp.arraySize, stateDrawers.Count - StatesProp.arraySize);
        }

        protected void CreateStateDrawer(int index, SerializedProperty property)
        {
            IStateDrawer drawer = StateDrawerFactory.CreateDrawer(index, property, stateful);
            drawer.StateAnimation += base.Repaint;
            stateDrawers.Insert(index, drawer);
        }

        public void AddState(IState state)
        {
            stateful.AddState(state);
            //StatesProp.InsertArrayElementAtIndex(StatesProp.arraySize);
            //SerializedProperty stateProp = StatesProp.GetArrayElementAtIndex(StatesProp.arraySize - 1);
            //stateProp.boxedValue = state;
            //serializedObject.ApplyModifiedProperties();
            //CreateStateDrawer(StatesProp.arraySize - 1, stateProp);
            stateful.EvaluateCurrentState();
        }

        //public void RemoveState(IStateDrawer drawer)
        //{
        //    IState state = drawer.GetState();
        //    int index = stateDrawers.IndexOf(drawer);

        //    if ((CurrentStateProp.boxedValue as IState).Equals(state))
        //    {
        //        CurrentStateProp.managedReferenceValue = null;
        //        serializedObject.ApplyModifiedProperties();
        //        stateful.EvaluateCurrentState();
        //    }
        //    StatesProp.DeleteArrayElementAtIndex(index);
        //    RefreshStateInfo();
        //}
        #endregion
    }
}
