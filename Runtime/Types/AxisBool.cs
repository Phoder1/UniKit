#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System;
using UnityEngine;

namespace Phoder1.Core.Types
{
    //Todo: probably possible to create this attribute with a classic editor drawer
#if ODIN_INSPECTOR
    [InlineProperty]
#endif
    [Serializable]
    public struct AxisBool
    {
        public AxisBool(bool x = default, bool y = default, bool z = default)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

#if ODIN_INSPECTOR
        [HorizontalGroup, LabelWidth(15)]
#endif
        public bool x;
#if ODIN_INSPECTOR
        [HorizontalGroup, LabelWidth(15)]
#endif
        public bool y;
#if ODIN_INSPECTOR
        [HorizontalGroup, LabelWidth(15)]
#endif
        public bool z;

        public void Freeze(ref Vector3 vector)
            => vector = Freeze(vector);
        public Vector3 Freeze(Vector3 vector)
            => Freeze(vector, this);
        public static Vector3 Freeze(Vector3 vector, AxisBool axisBool)
        {
            if (axisBool.x)
                vector.x = 0;
            if (axisBool.y)
                vector.y = 0;
            if (axisBool.z)
                vector.z = 0;

            return vector;
        }
        public static AxisBool True => new(true, true, true);
        public static AxisBool False => new(false, false, false);
        public static AxisBool Up => new(false, true, false);
        public static AxisBool Right => new(true, false, false);
        public static AxisBool Forward => new(false, false, true);
        public static AxisBool operator !(AxisBool a) => new(!a.x, !a.y, !a.z);
        public static AxisBool operator |(AxisBool a, AxisBool b) => new(a.x || b.x, a.y || b.y, a.z || b.z);
        public static AxisBool operator &(AxisBool a, AxisBool b) => new(a.x && b.x, a.y && b.y, a.z && b.z);
        public static AxisBool operator ^(AxisBool a, AxisBool b) => new(a.x ^ b.x, a.y ^ b.y, a.z ^ b.z);
        public static Vector3 operator *(Vector3 a, AxisBool b) => Vector3.Scale(a, (Vector3)b);
        public static explicit operator Vector3Int(AxisBool a) => new(a.x ? 1 : 0, a.y ? 1 : 0, a.z ? 1 : 0);
        public static explicit operator Vector3(AxisBool a) => new(a.x ? 1 : 0, a.y ? 1 : 0, a.z ? 1 : 0);
    }
}
