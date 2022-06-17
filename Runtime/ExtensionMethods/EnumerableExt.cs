using System;
using System.Collections;
using System.Collections.Generic;

namespace Phoder1.Core
{
    public static class EnumerableExt
    {
        public static string ToDisplayName(this IEnumerable enumerable, string seperator = ",")
        {
            if (enumerable == null)
                return null;

            string name = null;

            foreach (var current in enumerable)
            {
                if (name != null)
                    name += seperator + " ";

                name += current.ToString();
            }

            return name;
        }
        public static string ToDisplayName<T>(this IEnumerable<T> enumerable, Func<T, string> toDisaplyName, string seperator = ",")
        {
            if (toDisaplyName == null || enumerable == null)
                return null;

            string name = null;

            foreach (var current in enumerable)
            {
                if (name != null)
                    name += seperator + " ";

                name += toDisaplyName.Invoke(current);
            }

            return name;
        }

        public static List<T> Sort<T>(this IReadOnlyList<T> list, Comparison<T> comparison)
        {
            List<T> newArray = new List<T>(list);
            newArray.Sort(comparison);
            return newArray;
        }
        public static List<T> Where<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            if (predicate == null)
                throw new NullReferenceException();

            List<T> newList = new List<T>();

            for (int i = 0; i < list.Count; i++)
                if (predicate.Invoke(list[i]))
                    newList.Add(list[i]);

            return newList;
        }
        public static IEnumerable<TOut> Convert<TIn, TOut>(this IEnumerable<TIn> collection, Converter<TIn, TOut> converter)
        {
            foreach (var item in collection)
                yield return converter(item);
        }
    }
}
