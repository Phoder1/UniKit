using System;

namespace Phoder1.Core.Cooldown
{
    public interface ICooldownDefinition
    {
        /// <summary>
        /// Whether the abillity starts with it's cooldown ready
        /// </summary>
        bool StartReady { get; }
        /// <summary>
        /// The total duration needed to wait before the abillity can be reactivated
        /// </summary>
        float Cooldown { get; }
        /// <summary>
        /// If disabled the class will no longer continue to count cooldown
        /// </summary>
        bool CountCooldown { get; }
    }
    public interface IReadonlyCooldownHandler : ICooldownDefinition
    {
        /// <summary>
        /// The time passed since the last activation
        /// </summary>
        float CooldownMeter { get; }
        /// <summary>
        /// The cooldown progress (between 0 and 1)(CooldownMeter/Cooldown)
        /// </summary>
        float CooldownProgress { get; }
        bool CooldownReady { get; }
    }
    public interface ISetonlyCooldownHandler : IResetable, IReadyHandler
    {
        /// <summary>
        /// The total duration needed to wait before the abillity can be reactivated
        /// </summary>
        float Cooldown { set; }

        /// <summary>
        /// Progresses the cooldown meter by deltaTime
        /// </summary>
        /// <param name="deltaTime">The time passed since the last update call</param>
        void UpdateCooldown(float deltaTime);
        /// <summary>
        /// If disabled the class will no longer continue to count cooldown
        /// </summary>
        bool CountCooldown { set; }
        /// <summary>
        /// Sets the cooldown meter to 0s
        /// </summary>
        void Deplete();
        /// <summary>
        /// Forces the cooldown to be ready
        /// </summary>
        void ForceReady();
    }
    /// <summary>
    /// Used to calculate cooldown of actions
    /// </summary>
    public interface ICooldownHandler : ISetonlyCooldownHandler, IReadonlyCooldownHandler 
    {
        event Action<float> OnCooldownChange;
        event Action<float> OnCooldownfillPrecentage;
        new bool CountCooldown { get; set; }
    }
    /// <summary>
    /// Used to check ready state and react accordinly
    /// </summary>
    public interface IReadyHandler
    {
        bool IsReady { get; }
        /// <summary>
        /// Invoked when ready
        /// </summary>
        event Action OnReady;
        /// <summary>
        /// Invoked when no longer ready
        /// </summary>
        event Action OnUnready;

        /// <summary>
        /// Forces a ready check
        /// </summary>
        bool CheckReadyState();
    }
    public interface IProlongedOperation
    {
        event Action OnStartedUse;
        event Action OnEndedUse;
        event Action OnCanceledUse;
        event Action OnStartUseFailed;
        bool CanUse { get; set; }
        bool IsUsing { get; }
        /// <summary>
        /// Finishing use
        /// </summary>
        bool TryStartUse();
        void ForceStartUse();
        void EndUse();
        /// <summary>
        /// Cancel Use
        /// </summary>
        void CancelUse();
    }
}
