using System;
using UnityEngine;

namespace WestRicochet.StateMachine.States
{
    public class VictoryState : IState
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
}