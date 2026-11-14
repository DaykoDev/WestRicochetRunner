using System;

namespace WestRicochet.StateMachine.States
{
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
}