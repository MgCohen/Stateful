using System;
using System.Collections.Generic;

namespace Scaffold.Stateful
{
    public interface IStatefulBehaviour
    {
        event Action<IState> StateChanged;

        StateStrategy Strategy { get; }

        IEnumerable<IState> BuildStates();

        void ChangeState(IState state);

        void EvaluateCurrentState();

        IState GetCurrentState();
        
        void AddState(IState state);
        
        void RemoveState(IState state);
    }
}
