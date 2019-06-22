using UnityEngine;

namespace SpaceRts
{
    public abstract class StateMachine : MonoBehaviour
    {
        private IState _currentState;

        public IState CurrentState
        {
            get { return _currentState; }

            internal set
            {
                if (_currentState != value)
                {
                    if (_currentState != null)
                    {
                        _currentState.OnLeave();
                    }

                    _currentState = value;

                    if (_currentState != null)
                    {
                        _currentState.OnEnter();
                    }
                }
            }
        }

        protected virtual void Update()
        {
            if (_currentState != null)
            {
                _currentState.Update();
            }
        }
    }
}
