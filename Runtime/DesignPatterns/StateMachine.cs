using System;
using System.Collections.Generic;

namespace Phoder1.Patterns
{
    public interface IState
    {
        void OnEnter(IState from);
        void OnExit(IState to);
    }
    public class StateMachine<T> : IEquatable<StateMachine<T>>, IState where T : IState
    {
        private T _state;

        public StateMachine(T currentState)
        {
            _state = currentState;
        }

        public T State
        {
            get => _state;
            set
            {
                if (_state.Equals(value))
                    return;

                if (_state != null)
                    _state.OnExit(value);

                if (value != null)
                    value.OnEnter(_state);

                _state = value;
            }
        }

        public void OnEnter(IState from)
        {
            if (_state != null)
                _state.OnEnter(from);
        }

        public void OnExit(IState to)
        {
            if (_state != null)
                _state.OnExit(to);
        }
        public bool IsState(T state) => state.Equals(State);
        public static implicit operator T(StateMachine<T> machine) => machine.State;
        #region Equality
        public override bool Equals(object obj)
            => obj != null && obj is StateMachine<T> _machine && Equals(_machine);
        public bool Equals(StateMachine<T> other)
            => other != null && EqualityComparer<T>.Default.Equals(_state, other._state);
        public override int GetHashCode() =>  _state.GetHashCode();
        public static bool operator ==(StateMachine<T> left, StateMachine<T> right)
            => EqualityComparer<StateMachine<T>>.Default.Equals(left, right);
        public static bool operator !=(StateMachine<T> left, StateMachine<T> right)
            => !(left == right);
        #endregion
    }
}
