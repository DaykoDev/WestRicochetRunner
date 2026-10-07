using System;
using System.Collections.Generic;

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
        Type stateType = typeof(TState);

        if (_currentState is TState)
        {
            return;
        }

        if (_states.TryGetValue(stateType, out IState nextState) == false)
        {
            throw new InvalidOperationException($"State {stateType.Name} is not registered.");
        }

        _currentState?.Exit();

        _currentState = nextState;
        _currentState.Enter();
    }
}