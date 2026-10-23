using System;
using UnityEngine;

public class MainMenuState : IState
{
    public event Action Entered;
    public event Action Exited;

    public event Action StartRequested;

    public void Enter()
    {
        Time.timeScale = 1f;
        Entered?.Invoke();
    }

    public void Exit()
    {
        Exited?.Invoke();
    }

    public void RequestStart()
    {
        StartRequested?.Invoke();
    }
}