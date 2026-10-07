using System;

public class DefeatState : IState
{
    public event Action Entered;
    public event Action Exited;

    public event Action MainMenuRequested;

    public void Enter()
    {
        Entered?.Invoke();
    }

    public void Exit()
    {
        Exited?.Invoke();
    }

    public void RequestMainMenu()
    {
        MainMenuRequested?.Invoke();
    }
}