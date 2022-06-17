using System;
using System.Collections.Generic;
using UniRx;

namespace Phoder1.Patterns
{
    public interface ITokenReciever
    {
        Token GetToken();
    }
    public sealed class TokenMachine : ITokenReciever, IDisposable, IObservable<bool>
    {
        public event Action OnLock;
        public event Action OnRelease;

        private readonly List<WeakReference<Token>> _tokens = new();
        private readonly Subject<bool> _subject = new Subject<bool>();
        public int TokenCount => _tokens.Count;
        public bool Locked => TokenCount > 0;
        public void ForceRelease()
        {
            foreach (var token in _tokens)
                GetTokenFromReference(token).Dispose();
        }

        public bool Released => TokenCount == 0;

        public TokenMachine(Action OnLock = null, Action OnRelease = null)
        {
            if (OnLock != null)
                this.OnLock += OnLock;

            if (OnRelease != null)
                this.OnRelease += OnRelease;
        }

        public Token GetToken()
        {
            Token newToken = new Token();
            AddToken(newToken);
            return newToken;
        }

        private Token GetTokenFromReference(WeakReference<Token> reference)
        {
            if (reference != null && reference.TryGetTarget(out var token))
                return token;
            return null;
        }

        private static bool IsValid(WeakReference<Token> tokenRef)
            => tokenRef != null && tokenRef.TryGetTarget(out Token token) && IsValid(token);

        private static bool IsValid(Token token)
            => token != null && !token.Released;

        private void AddToken(Token newToken)
        {
            if (_tokens.Find((x) => x.TryGetTarget(out Token token) && token == newToken) != null)
                return;

            _tokens.Add(new WeakReference<Token>(newToken));

            if (_tokens.Count == 1)
            {
                OnLock?.Invoke();
                _subject.OnNext(true);
            }

            newToken.OnRelease += TokenReleased;

            void TokenReleased(Token releasedToken)
            {
                int index = _tokens.FindIndex((x) => x.TryGetTarget(out Token token) && token == releasedToken);

                if (index == -1)
                    return;

                _tokens.RemoveAt(index);

                if (_tokens.Count == 0)
                {
                    OnRelease?.Invoke();
                    _subject.OnNext(false);
                }
            }
        }

        public void Dispose()
        {
            ForceRelease();
        }

        public IDisposable Subscribe(System.IObserver<bool> observer) => _subject.Subscribe(observer);
    }
    public sealed class Token : IDisposable
    {
        private bool _released = false;
        public bool Released => _released;
        /// <summary>
        /// When the Check-In count reaches 0.
        /// </summary>
        public event Action<Token> OnRelease = null;
        ~Token()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (_released)
                return;

            _released = true;
            OnRelease?.Invoke(this);
        }

    }
}
