using System;
using System.Collections;
using UniKit.Patterns;
using UniRx;
using UnityEngine;

namespace UniKit
{
    public interface IReadonlyTimedTurns
    {
        IReadOnlyReactiveProperty<float> SecondsPerTurn { get; }
        IReadOnlyReactiveProperty<int> TurnNumber { get; }
        IObservable<int> TurnEnded { get; }
        TimeSpan TurnLength { get; }
        bool IsActive { get; }
    }
    public interface ITimedTurns : IReadonlyTimedTurns
    {
        new IReactiveProperty<float> SecondsPerTurn { get; }
        TokenMachine Disabled { get; }
        void StartTurns();
        void StopTurns();
        IDisposable Pause() => Disabled.GetToken();
    }
    [Serializable]
    public class Turns : ITimedTurns
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

        IReadOnlyReactiveProperty<float> IReadonlyTimedTurns.SecondsPerTurn => SecondsPerTurn;

        public bool IsActive => !Disabled.Locked;

        public IObservable<int> TurnEnded => turnEnded;

        IDisposable turns;

        public Turns()
        {
            secondsPerTurn = new ReactiveProperty<float>(minTurnSpeed);
        }

        IDisposable pauseToken = null;
        private void InputListen(Unit obj)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (pauseToken == null)
                {
                    pauseToken = Disabled.GetToken();
                }
                else
                {
                    pauseToken.Dispose();
                    pauseToken = null;
                }
            }
        }

        public void StartTurns()
        {
            if (turns == null)
                turns = Observable.FromCoroutine(CallTurn).Subscribe();

            MainThreadDispatcher.UpdateAsObservable().Subscribe(InputListen);
        }

        private IEnumerator CallTurn()
        {
            while (true)
            {
                SecondsPerTurn.Value = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, (float)turnCount.Value / (float)maxSpeedTurnNumber);
                TurnLength = TimeSpan.FromSeconds(secondsPerTurn.Value);

                if (IsActive)
                    turnCount.Value++;
                float turnLengthInSeconds = Mathf.Max(0.05f, (float)TurnLength.TotalSeconds);

                if (turnLengthInSeconds > 0f)
                    yield return new WaitForSeconds(turnLengthInSeconds);

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
