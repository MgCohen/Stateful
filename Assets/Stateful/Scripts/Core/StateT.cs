using System.Collections.Generic;
using UnityEngine;

namespace Scaffold.Stateful
{

    public abstract class State<T> : IState where T : IStatefulBehaviour
    {
        [SerializeField, HideInInspector] protected T component;

        public virtual string StateName => GetType().ReadableName();

        public virtual bool Equals(IState other)
        {
            if (other is not State<T> state)
            {
                return false;
            }

            return other == this;
        }

        public virtual bool Evaluate()
        {
            //defaults to false to force creation of validation logic, otherwise the state will never automatically change from what was manually setted
            return false;
        }

        public virtual void In() { }
        public virtual void Out() { }
        public virtual void Tick() { }
    }
}
