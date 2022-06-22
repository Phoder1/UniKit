using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace UniKit.Effects
{
    public interface IEffectRecipient<out T>
    {
        IDisposable ApplyEffect(IEffect<T> effect);
    }
    public interface IEffect<in T>
    {
        IDisposable ApplyEffect(T affected);
    }

    public interface IInstantEffectRecipient<out T>
    {
        void ApplyEffect(IInstantEffect<T> effect);
    }
    public interface IInstantEffect<in T>
    {
        void ApplyEffect(T affected);
    }
    public interface IOverTimeEffect<in T> : IEffect<T>
    {
        float Duration { get; set; }
        public IDisposable ApplyEffect(T affected, MonoBehaviour coroutineContainer, float durationScalar = 1f);
    }

    [Serializable]
    public class OverTimeEffect<TEffect, T> : IOverTimeEffect<T> where TEffect : IEffect<T>
    {
        [SerializeField]
        private float _duration;
        [SerializeField]
        private TEffect _effect;
        public float Duration { get => _duration; set => _duration = value; }
        public IDisposable ApplyEffect(T affected) => _effect.ApplyEffect(affected);

        public IDisposable ApplyEffect(T affected, MonoBehaviour coroutineContainer, float durationScalar = 1f)
        {
            return _effect.ApplyAsOvertimeEffect(affected, Duration * durationScalar, coroutineContainer);
        }
    }
    public class Effect<T> : IEffect<T>
    {
        private Func<T, IDisposable> _effect;

        public Effect(Func<T, IDisposable> effect)
        {
            _effect = effect;
        }

        public IDisposable ApplyEffect(T affected)
            => _effect?.Invoke(affected);
    }
    public class InstantEffect<T> : IInstantEffect<T>
    {
        private Action<T> _effect;
        public InstantEffect(Action<T> action)
        {
            _effect = action ?? throw new NullReferenceException(nameof(action));
        }

        public void ApplyEffect(T affected)
            => _effect?.Invoke(affected);
    }
    public static class EffectHelper
    {
        public static IDisposable ApplyAsOvertimeEffect<T>(this IEffect<T> effect, T affected, float duration, MonoBehaviour coroutineContainer)
        {
            if (effect == null)
                return null;

            var effectInstance = effect.ApplyEffect(affected);

            if (effectInstance != null)
                coroutineContainer.StartCoroutine(WaitForCancel());

            return effectInstance;


            IEnumerator WaitForCancel()
            {
                yield return new WaitForSeconds(duration);

                effectInstance?.Dispose();

                effectInstance = null;
            }
        }
        public static IDisposable ApplyEffect<T>(this IEnumerable<IEffect<T>> effects, T affected)
        {
            if (effects == null || !effects.Any())
                return null;

            var disposable = new CompositeDisposable();

            foreach (var effect in effects)
            {
                if (effect == null)
                    continue;

                var instance = effect.ApplyEffect(affected);

                if (instance != null)
                    disposable.Add(instance);
            }

            return disposable.Count == 0 ? null : disposable;
        }
        public static void ApplyEffect<T>(this IEnumerable<IInstantEffect<T>> effects, T affected)
        {
            foreach (var effect in effects)
                if (effect != null)
                    effect.ApplyEffect(affected);
        }
        public static IDisposable ApplyEffect<T>(this IEnumerable<IOverTimeEffect<T>> effects, MonoBehaviour coroutineContainer, T affected, float durationScalar = 1f)
        {
            if (effects == null || !effects.Any())
                return null;

            var disposable = new CompositeDisposable();

            foreach (var effect in effects)
            {
                if (effect == null)
                    continue;

                var instance = effect.ApplyEffect(affected, coroutineContainer, durationScalar);

                if (instance != null)
                    disposable.Add(instance);
            }

            return disposable.Count == 0 ? null : disposable;
        }
        public static IEffect<TOut> Convert<TOut, TIn>(this IEffect<TIn> origin, Converter<TOut, TIn> converter)
        {
            if (converter == null)
                return null;

            return new Effect<TOut>((x) => origin.ApplyEffect(converter.Invoke(x)));
        }
        public static IEffect<T> Combine<T>(this IEffect<T> effect, params IEffect<T>[] effects)
        {

            IEffect<T>[] newArray = effects
                .Append(effect)
                .Where((x) => x != null)
                .ToArray();

            return newArray.Combine();
        }
        public static IEffect<T> Combine<T>(this IEffect<T>[] effects)
        {
            if (effects == null || !effects.Any())
                return null;

            return new Effect<T>(NewEffect);

            IDisposable NewEffect(T affected)
            {
                List<IDisposable> instances = Array.ConvertAll(effects, (x) => x?.ApplyEffect(affected))
                    .Where((x) => x != null);

                return new CompositeDisposable(instances);
            }
        }
        public static IEffect<T> ToEffect<T>(this IEffectRecipient<T> recipient, IEffect<T> effect)
        {
            return new Effect<T>((T) => recipient.ApplyEffect(effect));
        }
    }
}
