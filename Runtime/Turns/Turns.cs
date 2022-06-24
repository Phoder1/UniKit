using System;
using System.Collections;
using UniKit.Attributes;
using UniKit.Patterns;
using UniRx;
using UnityEngine;

namespace UniKit
{
    public interface IReadonlyTurns
    {
        IReadOnlyReactiveProperty<float> TurnsPerSecond { get; }
        IReadOnlyReactiveProperty<int> TurnNumber { get; }
        TimeSpan TurnLength { get; }
        bool IsActive { get; }
    }
    public interface ITurns : IReadonlyTurns
    {
        new IReactiveProperty<float> TurnsPerSecond { get; }
        TokenMachine Active { get; }
        void StartTurns();
        void StopTurns();
        IDisposable Pause() => Active.GetToken();
    }
    [Serializable]
    public class Turns : ITurns
    {
        [SerializeField, Inline(true)]
        private ReactiveProperty<float> turnsPerSecond = new ReactiveProperty<float>(1);

        private readonly ReactiveProperty<int> turnCount = new ReactiveProperty<int>(0);

        public IReadOnlyReactiveProperty<int> TurnNumber => turnCount;

        public IReactiveProperty<float> TurnsPerSecond => turnsPerSecond;

        public TimeSpan TurnLength { get; private set; }

        public TokenMachine Active => new TokenMachine();

        IReadOnlyReactiveProperty<float> IReadonlyTurns.TurnsPerSecond => TurnsPerSecond;

        public bool IsActive => !Active.Locked;

        IDisposable turns;
        public void StartTurns()
        {
            if (turns == null)
                turns = Observable.FromCoroutine(CallTurn).Subscribe();
        }

        private IEnumerator CallTurn()
        {
            TurnLength = TimeSpan.FromSeconds(1 / TurnsPerSecond.Value);
            turnCount.Value++;

            yield return new WaitForSeconds((float)TurnLength.TotalSeconds);

            if (IsActive)
                turnCount.Value++;

            yield return CallTurn();
        }

        public void StopTurns()
        {
            turns?.Dispose();
            turns = null;
        }
    }
}
