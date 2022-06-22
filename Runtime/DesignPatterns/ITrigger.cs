using System;

namespace UniKit.Projectile
{
    public interface ITrigger
    {
        event Action OnTrigger;
        IDisposable Subscribe(IAction action);
    }
}