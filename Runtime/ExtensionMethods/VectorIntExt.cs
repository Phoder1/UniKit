using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.Core
{
    public static class VectorIntExt
    {
        public static int TileSteps(this Vector2Int from, Vector2Int to, bool canMoveDiagonally)
        {
            int xDistance = Mathf.Abs(to.x - from.x);
            int yDistance = Mathf.Abs(to.y - from.y);

            if (canMoveDiagonally)
                return xDistance + yDistance - Mathf.Min(xDistance, yDistance);
            else
                return xDistance + yDistance;
        }
    }
}
