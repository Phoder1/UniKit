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
        IReadOnlyReactiveProperty<float> SecondsPerTurn { get; }
        IReadOnlyReactiveProperty<int> TurnNumber { get; }
        IObservable<int> TurnEnded { get; }
        TimeSpan TurnLength { get; }
        bool IsActive { get; }
    }
    public interface ITurns : IReadonlyTurns
    {
        new IReactiveProperty<float> SecondsPerTurn { get; }
        TokenMachine Disabled { get; }
        void StartTurns();
        void StopTurns();
        IDisposable Pause() => Disabled.GetToken();
    }
    [Serializable]
    public class Turns : ITurns
    {
        [SerializeField, Tooltip("In seconds per turn")]
        private float minTurnSpeed = 2;
        [SerializeField, Tooltip("In seconds per turn")]
        private float maxTurnSpeed = 0.1f;
        [SerializeField]
        private int maxSpeedTurnNumber = 40;

        private ReactiveProperty<float> secondsPerTurn;

        private readonly ReactiveProperty<int> turnCount = new ReactiveProperty<int>(0);
        private readonly Subject<int> turnEnded = new Subject<int>();
        public IReadOnlyReactiveProperty<int> TurnNumber => turnCount;

        public IReactiveProperty<float> SecondsPerTurn => secondsPerTurn;

        public TimeSpan TurnLength { get; private set; }

        public TokenMachine Disabled => new TokenMachine();

        IReadOnlyReactiveProperty<float> IReadonlyTurns.SecondsPerTurn => SecondsPerTurn;

        public bool IsActive => !Disabled.Locked;

        public IObservable<int> TurnEnded => turnEnded;

        IDisposable turns;

        public Turns()
        {
            secondsPerTurn = new ReactiveProperty<float>(minTurnSpeed);
        }

        public void StartTurns()
        {
            if (turns == null)
                turns = Observable.FromCoroutine(CallTurn).Subscribe();
        }

        private IEnumerator CallTurn()
        {
            while (true)
            {
                SecondsPerTurn.Value = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, (float)turnCount.Value / (float)maxSpeedTurnNumber);
                TurnLength = TimeSpan.FromSeconds(secondsPerTurn.Value);

                if (IsActive)
                    turnCount.Value++;

                yield return new WaitForSeconds((float)TurnLength.TotalSeconds);

                turnEnded.OnNext(turnCount.Value);
            }
        }

        public void StopTurns()
        {
            turns?.Dispose();
            turns = null;
        }
    }
}
