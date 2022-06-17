using System;

namespace Phoder1.Projectile
{
    public interface ITrigger
    {
        event Action OnTrigger;
        IDisposable Subscribe(IAction action);
    }
}