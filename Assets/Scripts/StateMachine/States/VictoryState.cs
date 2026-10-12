using System;

public class VictoryState : IState
{
    public event Action Entered;
    public event Action Exited;

    public void Enter()
    {
        Entered?.Invoke();
    }

    public void Exit()
    {
        Exited?.Invoke();
    }
}