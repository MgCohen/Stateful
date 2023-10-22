using System;
using System.Collections.Generic;
using System.Linq;

namespace Scaffold.Stateful
{

    public class StateFactory
    {
        private static Dictionary<Type, List<Type>> StateDictionary = new Dictionary<Type, List<Type>>();

        public static List<TState> BuildStates<TState>(IStatefulBehaviour behaviour, List<TState> currentStates) where TState : IState
        {
            List<TState> states = behaviour.Strategy switch
            {
                StateStrategy.Unique => BuildUniqueStates(behaviour, currentStates),
                StateStrategy.Variable => BuildVariableStates(behaviour, currentStates),
                StateStrategy.Shared => BuildGlobalStates(behaviour, currentStates),
                _ => throw new Exception("no defined state")
            };

            return states;
        }

        public static List<Type> GetStateTypes(IStatefulBehaviour stateful)
        {
            Type statefulType = stateful.GetType();
            if (!StateDictionary.TryGetValue(statefulType, out List<Type> types))
            {
                types = new List<Type>();
                Type generic = typeof(State<>);
                while (statefulType.Is(typeof(IStatefulBehaviour)))
                {
                    Type childType = generic.MakeGenericType(statefulType);
                    types.AddRange(ReflectionHelper.GetAllDerivedTypes(childType, false));
                    statefulType = statefulType.BaseType;
                }
                StateDictionary[statefulType] = types;
            }
            return types;
        }

        private static List<TState> BuildUniqueStates<TState>(IStatefulBehaviour stateful, List<TState> currentStates) where TState : IState
        {

            List<Type> statesToBuild = GetStateTypes(stateful);
            statesToBuild = statesToBuild.Where(s => !currentStates.Any(cs => cs.GetType() == s)).ToList();
            foreach (var stateType in statesToBuild)
            {
                TState state = (TState)BuildState(stateType, stateful);
                currentStates.Add(state);
            }
            return currentStates;
        }

        private static List<TState> BuildVariableStates<TState>(IStatefulBehaviour stateful, List<TState> currentStates) where TState : IState
        {
            return currentStates;
        }

        private static List<TState> BuildGlobalStates<TState>(IStatefulBehaviour stateful, List<TState> currentStates) where TState : IState
        {
            //get unique from global
            return currentStates;
        }

        public static IState BuildState(Type stateType, IStatefulBehaviour ui)
        {
            IState state = (IState)Activator.CreateInstance(stateType);
            System.Reflection.FieldInfo componentField = stateType.GetField("component", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            componentField.SetValue(state, ui);
            return state;
        }
    }

    public enum StateStrategy
    {
        Unique = 10,
        Variable = 20,
        Shared = 30
    }
}
