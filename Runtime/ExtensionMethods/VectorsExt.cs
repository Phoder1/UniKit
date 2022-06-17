using System;
using UnityEngine;

namespace Phoder1.Core
{
    public static class VectorsExt
    {
        public static bool Approximately(this Vector3 a, Vector3 b)
            => a == b || (Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z));
        public static bool Approximately(this Vector2 a, Vector2 b)
            => Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        public static Vector2 ToTopDown(this Vector3 vector) => new Vector2(vector.x, vector.z);
        public static Vector3 FromTopDown(this Vector2 vector) => new Vector3(vector.x, 0, vector.y);
        public static Vector3 Absolute(this Vector3 vector) => new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        public static Vector3 Opposite(this Vector3 direction) => -direction;
        public static Vector2 Absolute(this Vector2 vector) => new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        public static Vector3 Divide(this Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        public static Vector2 Divide(this Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);
        public static Vector3Int Sign(this Vector3 vector) => new Vector3Int(Math.Sign(vector.x), Math.Sign(vector.y), Math.Sign(vector.z));
        public static Vector2Int Sign(this Vector2 vector) => new Vector2Int(Math.Sign(vector.x), Math.Sign(vector.y));
    }
}
