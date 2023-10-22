using UnityEditor;
using static Scaffold.Stateful.Samples.StatefulAnimatorController;


namespace Scaffold.Stateful.Samples
{
    public class AnimatorStateDrawer : StateDrawer<AnimatorState>
    {
        public AnimatorStateDrawer(int index, SerializedProperty stateProp, IStatefulBehaviour stateful) : base(index, stateProp, stateful)
        {

        }

        protected override void DrawBody()
        {
            EditorGUI.BeginDisabledGroup(true);
            base.DrawBody();
            EditorGUI.EndDisabledGroup();
        }

        public override string GetStateName()
        {
            return State.StateName;
        }
    }
}