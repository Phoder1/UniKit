using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Phoder1.Core
{
    [Flags]
    public enum ConditionEnum
    {
        Bigger = 1,
        Smaller = 2,
        Equal = 4
    }

    [Serializable]
    public class Condition<T>
        where T : IComparable<T>
    {
        [SerializeField]
        private ConditionEnum _condition;


        public Condition() { }
        public Condition(ConditionEnum condition)
        {
            _condition = condition;
        }

        public virtual bool MetCondition(T val1, T val2)
        {
            foreach (ConditionEnum con in Enum.GetValues(typeof(ConditionEnum)))
            {
                if (_condition.HasFlag(con))
                {
                    switch (con)
                    {
                        case ConditionEnum.Bigger:
                            if (val1.CompareTo(val2) > 0)
                                return true;
                            break;
                        case ConditionEnum.Smaller:
                            if (val1.CompareTo(val2) < 0)
                                return true;
                            break;
                        case ConditionEnum.Equal:
                            if (val1.CompareTo(val2) == 0)
                                return true;
                            break;
                    }
                }
            }
            return false;
        }
    }

    [Serializable]
    public class CachedCondition<T>
        where T : IComparable<T>
    {
        [SerializeField]
        private ValueCondition[] _valueConditions;
        public virtual bool MetCondition(T value)
            => Array.TrueForAll(_valueConditions, (x) => x.MetCondition(value));

        [Serializable]
        private struct ValueCondition
        {
            [SerializeField]
            private Condition<T> _condition;
            [SerializeField]
            private T _value;
            public bool MetCondition(T value)
                => _condition.MetCondition(value, _value);
        }
    }
    [Serializable]
    public class ConditionalEvent<T> : CachedCondition<T>
        where T : IComparable<T>
    {
        [SerializeField]
        private UnityEvent<T> OnConditionMet;
        public virtual bool CheckEvent(T value)
        {
            bool metCondition = MetCondition(value);

            if (metCondition)
                OnConditionMet?.Invoke(value);

            return metCondition;
        }
    }

    [Serializable]
    public class ConditionalEvents<T>
        where T : IComparable<T>
    {
        [SerializeField]
        private ConditionalEvent<T>[] _conditionalEvents;
        public IReadOnlyList<ConditionalEvent<T>> ConditionalEvent => _conditionalEvents;
        public bool CheckEvents(T value)
        {
            bool anyConditionMet = false;

            for (int i = 0; i < _conditionalEvents.Length; i++)
                if (_conditionalEvents[i].CheckEvent(value))
                    anyConditionMet = true;

            return anyConditionMet;
        }
    }
}
