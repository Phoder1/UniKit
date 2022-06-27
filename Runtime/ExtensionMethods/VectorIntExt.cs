using System.Collections.Generic;
using UnityEngine;

namespace UniKit
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
        public static IEnumerable<Vector2Int> GetStraightNeighbors(this Vector2Int tilePos)
        {
            yield return tilePos + Vector2Int.up;
            yield return tilePos + Vector2Int.down;
            yield return tilePos + Vector2Int.left;
            yield return tilePos + Vector2Int.right;
        }
        public static IEnumerable<Vector2Int> GetDiagonalNeighbors(this Vector2Int tilePos)
        {
            yield return tilePos + Vector2Int.one;
            yield return tilePos - Vector2Int.one;
            yield return tilePos + new Vector2Int(1, -1);
            yield return tilePos + new Vector2Int(-1, 1);
        }
        public static bool IsNeighborOf(this Vector2Int tile, Vector2Int possibleNeighbor, bool includeDiagonal)
            => tile.TileSteps(possibleNeighbor, includeDiagonal) <= 1;
    }
}
