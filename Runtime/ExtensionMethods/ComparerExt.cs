using System;

namespace Phoder1.Core
{
    public static class ComparerExt
    {
        public static Comparison<T> Inverse<T>(this Comparison<T> comparison)
        {
            if (comparison == null)
                return null;

            return InversedComparison;
            int InversedComparison(T x, T y) => -comparison.Invoke(x, y);
        }
    }
}
