using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.Core
{
    public static class FactoryExt
    {
        public static T[] Create<T>(this IFactory<T>[] factories)
            => Array.ConvertAll(factories, (x) => x.Create());
    }
}
