using System;

public class BootstrapState : IState
{
    public event Action Completed;

    public void Enter()
    {
        Completed?.Invoke();
    }

    public void Exit()
    {
    }
}