using UnityEditor;

namespace Scaffold.Stateful
{
    public class StateDrawer : StateDrawer<IState>
    {
        public StateDrawer(int index, SerializedProperty stateProp, IStatefulBehaviour stateful) : base(index, stateProp, stateful)
        {

        }
    }
}