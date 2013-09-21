using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace morfel.FiniteStateMachine
{
    public abstract class StateMachine<TMachine, TState> : MonoBehaviour 
        where TState : struct
        where TMachine : StateMachine<TMachine, TState>
    {
        public bool DebugEnabled { get; protected set; }
        private TState? _currentState;
        private TState? _previousState;
        private readonly Dictionary<TState, State<TState, TMachine>> _instances;

        protected StateMachine()
        {
            _instances = new Dictionary<TState, State<TState, TMachine>>();
        }

        protected void SetDebug(bool to)
        {
            DebugEnabled = to;
        }

        protected void AddState(TState state, State<TState, TMachine> instance)
        {
            if (_instances.ContainsKey(state))
            {
                if (DebugEnabled)
                Debug.LogWarning(String.Format(
                    "State machine already contains key {0}", state
                    ));
                return;
            }
            if (instance == null)
            {
                if (DebugEnabled)
                Debug.LogWarning(String.Format(
                    "State instance must not be null! {0}", state
                    ));
                return;
            }
            instance.Bind(this as TMachine);
            _instances.Add(state, instance);
            if (_currentState == null)
            {
                _currentState = state;
            }
        }

        public void GotoPreviousState(object data=null)
        {
            if (_previousState != null)
            {
                GotoState(_previousState.Value, data);
            }
        }

        public void GotoState(TState state, object data=null)
        {
            if (!_instances.ContainsKey(state))
            {
                if (DebugEnabled)
                Debug.LogWarning(String.Format(
                    "State '{0}' does not exist in State machine", state
                    ));
                return;
            }

            if (state.Equals(_currentState.Value))
            {
                if (DebugEnabled)
                Debug.LogWarning(String.Format(
                    "Already in State '{0}'", state
                    ));
                return;
            }

            _previousState = _currentState;
            _currentState = state;

            if (_previousState != null)
            {
                var previousInstance = _instances[_previousState.Value];
                previousInstance.OnExit();
            }
            var currentInstance = _instances[_currentState.Value];
            currentInstance.OnEnter(data);
        }

        private void Awake()
        {
            DebugEnabled = false;
            Initialize();
            foreach (var kvp in _instances)
            {
                kvp.Value.OnAwake();
            }
        }

        protected abstract void Initialize();

        public void Update()
        {
            if (!EnforceExistingState()) return;
            _instances[_currentState.Value].OnUpdate();
        }

        public void FixedUpdate()
        {
            if (!EnforceExistingState()) return;
            _instances[_currentState.Value].OnFixedUpdate();
        }

        private bool EnforceExistingState()
        {
            if (_currentState == null)
            {
                if (DebugEnabled)
                Debug.LogWarning("No states defined");
                return false;
            }
            return true;
        }
    }
}
