using System;

namespace UniKit.Patterns
{
    public static class FactoryExt
    {
        public static T[] Create<T>(this IFactory<T>[] factories)
            => Array.ConvertAll(factories, (x) => x.Create());
    }
}
