using System;
using System.Collections;
using UnityEngine;

namespace morfel.FiniteStateMachine
{
    public abstract class State<TState, TMachine>
        where TMachine : StateMachine<TMachine, TState>
        where TState : struct
    {
        public TMachine Machine;
        private bool _debug;

        public void Bind(TMachine machine)
        {
            Machine = machine;
            _debug = machine.DebugEnabled;
        }
        public State<TState, TMachine> SetDebug(bool to)
        {
            _debug = to;
            return this;
        }
        public virtual void OnReason() {}
        public virtual void OnAwake() {}
        public virtual void OnEnter(object data = null)
        {
            if (_debug)
            {
                Debug.Log(String.Format(
                    "{0}: Entering State '{1}'",
                    typeof (TMachine),
                    this.GetType()
                    ));
            }
        }
        public virtual void OnExit()
        {
            if (_debug)
            {
                Debug.Log(String.Format(
                    "{0}: Leaving State '{1}'",
                    typeof (TMachine),
                    this.GetType()
                    ));
            }
        } 
        public virtual void OnUpdate() {}
        public virtual void OnFixedUpdate() {}

        # region Proxy Calls
        protected void StartCoroutine(IEnumerator routine)
        {
            Machine.StartCoroutine(routine);
        }

        protected void GotoState(TState state, object data=null)
        {
            Machine.GotoState(state, data);
        }

        protected void GotoPreviousState(object data = null)
        {
            Machine.GotoPreviousState(data);
        }
        # endregion
    }
}
