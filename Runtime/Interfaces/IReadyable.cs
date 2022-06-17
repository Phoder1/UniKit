using UniRx;

public interface IReadyable : IReactiveProperty<bool>
{
    bool Ready { get; }
}
