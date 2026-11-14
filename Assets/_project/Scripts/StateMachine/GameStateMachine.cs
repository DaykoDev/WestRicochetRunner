using System;
using System.Collections.Generic;

namespace WestRicochet.StateMachine
{
    public class GameStateMachine
    {
        private readonly Dictionary<Type, IState> _states;

        private IState _currentState;

        public GameStateMachine(IEnumerable<IState> states)
        {
            _states = new Dictionary<Type, IState>();

            foreach (IState state in states)
            {
                _states.Add(state.GetType(), state);
            }
        }

        public void Enter<TState>() where TState : class, IState
        {
            if (TryChangeState(out TState state))
            {
                state.Enter();
            }
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            if (TryChangeState(out TState state))
            {
                state.Enter(payload);
            }
        }

        private bool TryChangeState<TState>(out TState nextState) where TState : class, IState
        {
            nextState = null;

            Type stateType = typeof(TState);

            if (_currentState is TState)
            {
                return false;
            }

            if (_states.TryGetValue(stateType, out IState registeredState) == false)
            {
                throw new InvalidOperationException($"State {stateType.Name} is not registered.");
            }

            nextState = (TState)registeredState;

            _currentState?.Exit();

            _currentState = nextState;

            return true;
        }
    }
}