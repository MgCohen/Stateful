using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scaffold.Stateful
{
    public class StateDrawerContainer : ScriptableSingleton<StateDrawerContainer>
    {
        private Dictionary<IStatefulBehaviour, List<IStateDrawer>> drawers = new Dictionary<IStatefulBehaviour, List<IStateDrawer>>();

        public bool GetDrawers(IStatefulBehaviour stateful, out List<IStateDrawer> list)
        {
            if (!drawers.TryGetValue(stateful, out list))
            {
                list = new List<IStateDrawer>();
                drawers[stateful] = list;
                return true;
            }
            return false;
        }
    }
}
