using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scaffold.Stateful
{
    public class StateDrawerFactory
    {
        private static Dictionary<Type, Type> DrawerLookUp = new Dictionary<Type, Type>();

        public static IStateDrawer CreateDrawer(int index, SerializedProperty property, IStatefulBehaviour stateful)
        {
            IState state = property.boxedValue as IState;
            Type stateType = state.GetType();
            Type drawerType = GetDrawerType(stateType);
            return (IStateDrawer)Activator.CreateInstance(drawerType, new object[] { index, property, stateful});
        }

        private static Type GetDrawerType(Type stateType)
        {
            if (!DrawerLookUp.TryGetValue(stateType, out Type drawerType))
            {
                Type generic = typeof(StateDrawer<>);
                Type childType = generic.MakeGenericType(stateType);
                var types = ReflectionHelper.GetAllDerivedTypes(childType);
                drawerType = types.Count > 0 ? types[0] : typeof(StateDrawer);
                DrawerLookUp[stateType] = drawerType;
            }
            return drawerType;
        }
    }
}