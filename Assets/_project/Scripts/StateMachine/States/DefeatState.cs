using System;
using UnityEngine;

public class DefeatState : IState
{
    public event Action Entered;
    public event Action Exited;

    public void Enter()
    {
        Time.timeScale = 0f;
        Entered?.Invoke();
    }

    public void Exit()
    {
        Exited?.Invoke();
    }
}