using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniKit
{
    public static class AStar
    {
        public delegate bool IsTraversable(Vector2Int pos);
        public delegate bool HasReachedTarget(Vector2Int targetPosition, Vector2Int currentPosition, GridPathfindingSettings settings);
        public readonly struct GridPathfindingSettings
        {
            public readonly IsTraversable IsTraversable;
            public readonly HasReachedTarget HasReachedTarget;
            public readonly bool canMoveDiagonaly;
            public readonly bool canMoveThroughCorners;

            public int StepsBetween(Vector2Int from, Vector2Int to)
                => from.TileSteps(to, canMoveDiagonaly);
            public GridPathfindingSettings(IsTraversable isTraversable, bool canMoveDiagonaly = true, bool canMoveThroughCorners = false, HasReachedTarget hasReachedTarget = null)
            {
                IsTraversable = isTraversable ?? throw new ArgumentNullException(nameof(isTraversable));
                HasReachedTarget = hasReachedTarget ?? ((target, current, settings) => target == current);
                this.canMoveDiagonaly = canMoveDiagonaly;
                this.canMoveThroughCorners = canMoveThroughCorners;
            }
        }
        public readonly struct GridPathfindResult
        {
            public readonly bool HasPath;
            public readonly IReadOnlyList<Vector2Int> Path;
            public int PathLength => Path.Count;
            public readonly Vector2Int From;
            public readonly Vector2Int To;
            public readonly GridPathfindingSettings Settings;

            public GridPathfindResult(bool hasPath, IReadOnlyList<Vector2Int> path, Vector2Int from, Vector2Int to, GridPathfindingSettings settings)
            {
                HasPath = hasPath;
                Path = path ?? throw new ArgumentNullException(nameof(path));
                From = from;
                To = to;
                Settings = settings;
            }
        }
        public static GridPathfindResult GetStepsToTarget(this Vector2Int from, Vector2Int to, GridPathfindingSettings settings)
        {
            if (settings.IsTraversable == null)
                throw new NullReferenceException(nameof(settings.IsTraversable));

            var tiles = new Dictionary<Vector2Int, TileInformation>();
            if (from == to)
                return CalculateResult(from);

            Vector2Int lastTile = from;
            var undiscoveredTiles = new List<KeyValuePair<Vector2Int, TileInformation>>();

            undiscoveredTiles.Add(new KeyValuePair<Vector2Int, TileInformation>(from, new TileInformation(from, 0, GetStepsToTarget(from))));


            while (true)
            {
                if (!BestUndiscoveredTile(out var bestTile))
                    return CalculateResult(lastTile);

                if (DiscoverTile(bestTile))
                    return CalculateResult(to);

                lastTile = bestTile.Key;
            }


            //Returns false if there's no more tiles to uncover
            bool BestUndiscoveredTile(out KeyValuePair<Vector2Int, TileInformation> bestTile)
            {
                if (undiscoveredTiles == null || undiscoveredTiles.Count == 0)
                {
                    bestTile = default;
                    return false;
                }

                undiscoveredTiles.Sort((a, b) => a.Value.F.CompareTo(b.Value.F));

                bestTile = undiscoveredTiles[0];
                return true;
            }
            //Returns true if reached end
            bool DiscoverTile(KeyValuePair<Vector2Int, TileInformation> tile)
            {
                tiles.Add(tile.Key, tile.Value);
                undiscoveredTiles.Remove(tile);

                foreach (var neighbor in ValidUndiscoveredNeighbors(tile.Key))
                {
                    if (neighbor == to)
                        return true;

                    AddNewUndiscoveredTileInfo(neighbor, tile.Key);
                }

                return false;
            }

            GridPathfindResult CalculateResult(Vector2Int endTile)
                => new GridPathfindResult(endTile == to, CalculatePath(endTile), from, to, settings);

            IReadOnlyList<Vector2Int> CalculatePath(Vector2Int endTile)
            {
                var steps = new List<Vector2Int>();
                Vector2Int currentTile = endTile;
                while (currentTile != from)
                {
                    steps.Add(currentTile);
                    currentTile = tiles[currentTile].Parent;
                }
                return steps;
            }

            void AddNewUndiscoveredTileInfo(Vector2Int atTile, Vector2Int fromTile)
                => undiscoveredTiles.Add(new KeyValuePair<Vector2Int, TileInformation>(atTile, NewTileInfo(atTile, fromTile)));
            TileInformation NewTileInfo(Vector2Int atTile, Vector2Int fromTile)
                => new TileInformation(fromTile, tiles[fromTile].G + 1, GetStepsToTarget(fromTile));

            int GetStepsToTarget(Vector2Int fromTile)
                => settings.StepsBetween(fromTile, to);

            IEnumerable<Vector2Int> ValidUndiscoveredNeighbors(Vector2Int tilePos)
            {
                foreach (var neighbor in tilePos.GetStraightNeighbors())
                {
                    if (undiscoveredTiles.FindIndex((x) => x.Key == neighbor) != -1)
                        continue;

                    if (tiles.ContainsKey(neighbor))
                        continue;

                    if (!settings.IsTraversable.Invoke(neighbor))
                        continue;

                    yield return neighbor;
                }

                foreach (var neighbor in tilePos.GetDiagonalNeighbors())
                {
                    if (undiscoveredTiles.FindIndex((x) => x.Key == neighbor) != -1)
                        continue;

                    if (tiles.ContainsKey(neighbor))
                        continue;

                    if (!settings.IsTraversable.Invoke(neighbor))
                        continue;

                    if (!settings.canMoveThroughCorners)
                    {
                        var corner1 = new Vector2Int(tilePos.x, neighbor.y);
                        if (!settings.IsTraversable(corner1))
                            continue;

                        var corner2 = new Vector2Int(neighbor.x, tilePos.y);
                        if (!settings.IsTraversable(corner2))
                            continue;
                    }

                    yield return neighbor;
                }
            }
        }

        private readonly struct TileInformation
        {
            /// <summary>
            /// Distance from start
            /// </summary>
            public readonly int G;
            /// <summary>
            /// Estimated distance from end point
            /// </summary>
            public readonly int H;
            /// <summary>
            /// 
            /// </summary>
            public readonly Vector2Int Parent;

            public TileInformation(Vector2Int parent, int G, int H) : this()
            {
                Parent = parent;
                this.G = G;
                this.H = H;
            }

            /// <summary>
            /// Tile score (distance from start + approx distance from end)
            /// </summary>
            public int F => G + H;


        }
    }
}
