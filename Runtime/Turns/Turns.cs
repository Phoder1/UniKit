using UniKit.Core.Attributes;
using UniKit.Patterns;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace UniKit
{
    public interface ITurns
    {
        IReactiveProperty<float> TurnsPerSecond { get; }
        IReadOnlyReactiveProperty<int> TurnCount { get; }
        TimeSpan TurnLength { get; }
        TokenMachine Active { get; }
        void StartTurns();
        IDisposable Pause() => Active.GetToken();
    }
    [Serializable]
    public class Turns : ITurns
    {
        [SerializeField, Inline(true)]
        private ReactiveProperty<float> turnsPerSecond = new ReactiveProperty<float>(1);

        private readonly ReactiveProperty<int> turnCount = new ReactiveProperty<int>(0);

        public IReadOnlyReactiveProperty<int> TurnCount => turnCount;

        public IReactiveProperty<float> TurnsPerSecond => turnsPerSecond;

        public TimeSpan TurnLength { get; private set; }

        public TokenMachine Active => new TokenMachine(null, null);

        public void StartTurns()
        {
            MainThreadDispatcher.StartCoroutine(CallTurn());
        }

        private IEnumerator CallTurn()
        {
            TurnLength = TimeSpan.FromSeconds(1 / TurnsPerSecond.Value);
            turnCount.Value++;

            yield return new WaitForSeconds((float)TurnLength.TotalSeconds);

            yield return CallTurn();
        }
    }
}
