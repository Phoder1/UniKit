using UnityEngine;

namespace UniKit
{
    public static class RectExt
    {

        public static bool IsPointInside(this RectInt rect, Vector2Int vector)
            //X
            => vector.x <= rect.xMax
            && vector.x >= rect.xMin
            //Y
            && vector.y <= rect.yMax
            && vector.y >= rect.yMin;
    }
}
