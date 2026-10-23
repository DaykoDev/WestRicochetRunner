public interface IPayloadedState<TPayload> : IState
{
    void Enter(TPayload payload);
}