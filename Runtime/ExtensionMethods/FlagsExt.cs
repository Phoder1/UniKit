using UnityEngine;

namespace Phoder1.Core
{
    public static class FlagsExt
    {
        public static void Add(this ref int a, int b)
            => a |= b;
        public static void Remove(this ref int a, int b)
            => a &= ~b;
        public static bool Contains(this int a, int b)
            => a == (a | (1 << b));
        public static void Invert(this ref int flags)
            => flags = ~flags;
        public static int Inverted(this int flags)
            => ~flags;
        public static int XOR(this int a, int b)
            => a ^ b;
        public static int AND(this int a, int b)
            => a & b;
        public static int OR(this int a, int b)
            => a | b;
    }
    public static class LayerMaskExt
    {
        public static void Add(this ref LayerMask layerMask, int layer)
            => layerMask |= layer;
        public static void Remove(this ref LayerMask layerMask, int layer)
            => layerMask &= ~layer;
        public static bool Contains(this LayerMask layermask, int layer)
            => layermask == (layermask | (1 << layer));
        public static void Invert(this ref LayerMask layermask)
            => layermask = ~layermask;
        public static int Inverted(this LayerMask layermask)
            => ~layermask;
        public static LayerMask XOR(this LayerMask a, LayerMask b)
            => a ^ b;
        public static LayerMask AND(this LayerMask a, LayerMask b)
            => a & b;
        public static LayerMask OR(this LayerMask a, LayerMask b)
            => a | b;
    }
}
