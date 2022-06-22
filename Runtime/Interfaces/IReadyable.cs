using UniRx;

namespace UniKit
{
    public interface IReadyable : IReactiveProperty<bool>
    {
        bool Ready { get; }
    }
}