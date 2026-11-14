namespace WestRicochet.StateMachine
{
    public interface IPayloadedState<TPayload> : IState
    {
        void Enter(TPayload payload);
    }
}