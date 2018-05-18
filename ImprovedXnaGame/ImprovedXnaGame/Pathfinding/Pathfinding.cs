using Age.Core;
using Age.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Pathfinding
{
    class Pathfinding {
        private static int PathfindingSearchId = 0;
        public static LinkedList<Vector2> AStar(Unit who, Vector2 targetPrecise, Map map, PathfindingMode mode)
        {
            PathfindingSearchId++;
            Tile start = who.Occupies;
            Tile target = map.GetTileFromStandardCoordinates(targetPrecise);
            Tile closestToTargetSoFar = start;
            HashSet<Tile> openSet = new HashSet<Tile>
            {
                start
            };
            SetPathfindingInformation(start, PathfindingSearchId, false, 0, Pathfinding.Heuristic(start, target), null);

            while (openSet.Any())
            {
                Tile current = null;
                foreach (var tile in openSet)
                {
                    if (current == null || tile.Pathfinding_F < current.Pathfinding_F) current = tile;
                }
                if (current == target)
                {
                    return ReconstructPrettyPath(start, target, who.FeetStdPosition, targetPrecise);
                }
                openSet.Remove(current);
                current.Pathfinding_Closed = true;
                foreach (var edge in current.Neighbours.Traversable)
                {
                    Tile neighbour = edge.Destination;
                    if (neighbour.Pathfinding_EncounteredDuringSearch == PathfindingSearchId &&
                        neighbour.Pathfinding_Closed) continue;
                    if (neighbour.PreventsMovement) continue;
                    //if (neighTile.PathfindingOccupants.Any(un => un != who)) continue;

                    if (neighbour.Pathfinding_EncounteredDuringSearch < PathfindingSearchId)
                    {
                        SetPathfindingInformation(neighbour, PathfindingSearchId, false, int.MaxValue, int.MaxValue, current);
                        openSet.Add(neighbour);
                    }

                    int tentativeGScore = current.Pathfinding_G + edge.Difficulty; 
                    if (tentativeGScore >= neighbour.Pathfinding_G) continue;

                    neighbour.Pathfinding_Parent = current;
                    neighbour.Pathfinding_G = tentativeGScore;
                    neighbour.Pathfinding_F = neighbour.Pathfinding_G + Heuristic(neighbour, target);
                    if (neighbour.Pathfinding_F < closestToTargetSoFar.Pathfinding_F)
                    {
                        closestToTargetSoFar = neighbour;
                    }
                }
            }
            if (mode == PathfindingMode.FindClosestIfDirectIsImpossible)
            {
                return ReconstructPrettyPath(start, closestToTargetSoFar, who.FeetStdPosition, Isomath.TileToStandard(closestToTargetSoFar.X + 0.5f, closestToTargetSoFar.Y + 0.5f));
            }
            return null;
        }

        private static LinkedList<Vector2> ReconstructPrettyPath(Tile start, Tile reachableDestination, Vector2 trueStart, Vector2 trueEnd)
        {
            var l = ReconstructPath(start, reachableDestination);
            if (l.Count == 1 && l.First.Value == start)
            {
                float distance = (trueEnd - trueStart).LengthSquared();
                if (distance <= 9)
                {
                    return null;
                }
            }
            LinkedList<Vector2> result = new LinkedList<Vector2>();
            foreach (Tile tl in l)
            {
                result.AddLast(Isomath.TileToStandard(tl.X + 0.5f, tl.Y + 0.5f));
            }
            result.RemoveLast();
            result.AddLast(trueEnd);
            return result;
        }

        internal static LinkedList<Vector2> DijkstraMultiple(Unit who, HashSet<Vector2> targetTiles, Map map)
        {
            PathfindingSearchId++;
            Tile start = who.Occupies;
            foreach(Vector2 vector in targetTiles)
            {
                Tile target = map.GetTileFromStandardCoordinates(vector);
                target.Pathfinding_IsTargetDuringThisSearch = PathfindingSearchId;
                target.Pathfinding_TargetPreciseLocation = vector;
            }
            HashSet<Tile> openSet = new HashSet<Tile>
            {
                start
            };
            SetPathfindingInformation(start, PathfindingSearchId, false, 0, 0, null);

            while (openSet.Any())
            {
                Tile current = null;
                foreach (var tile in openSet)
                {
                    if (current == null || tile.Pathfinding_F < current.Pathfinding_F) current = tile;
                }
                if (current.Pathfinding_IsTargetDuringThisSearch == PathfindingSearchId)
                {
                    return ReconstructPrettyPath(start, current, who.FeetStdPosition, current.Pathfinding_TargetPreciseLocation);
                }
                openSet.Remove(current);
                current.Pathfinding_Closed = true;
                foreach (var edge in current.Neighbours.Traversable)
                {
                    Tile neighbour = edge.Destination;
                    if (neighbour.Pathfinding_EncounteredDuringSearch == PathfindingSearchId &&
                        neighbour.Pathfinding_Closed) continue;
                    if (neighbour.PreventsMovement) continue;

                    if (neighbour.Pathfinding_EncounteredDuringSearch < PathfindingSearchId)
                    {
                        SetPathfindingInformation(neighbour, PathfindingSearchId, false, int.MaxValue, int.MaxValue, current);
                        openSet.Add(neighbour);
                    }

                    int tentativeGScore = current.Pathfinding_G + edge.Difficulty;
                    if (tentativeGScore >= neighbour.Pathfinding_G) continue;

                    neighbour.Pathfinding_Parent = current;
                    neighbour.Pathfinding_G = tentativeGScore;
                    neighbour.Pathfinding_F = neighbour.Pathfinding_G;
                }
            }
            return null;
        }

        private static LinkedList<Tile> ReconstructPath(Tile start, Tile current)
        {
            LinkedList<Tile> path = new LinkedList<Tile>();
            path.AddLast(current);
            while (current.Pathfinding_Parent != null &&
                current.Pathfinding_Parent != start)
            {
                path.AddFirst(current.Pathfinding_Parent);
                current = current.Pathfinding_Parent;
            }
            return path;
        }

        private static int Heuristic(Tile start, Tile target)
        {
            return Math.Abs(start.X - target.X) * 130 + Math.Abs(start.Y - target.Y) * 130;
        }

        private static void SetPathfindingInformation(Tile t, int searchId, bool closed, int g, int f, Tile parent)
        {
            t.Pathfinding_EncounteredDuringSearch = searchId;
            t.Pathfinding_Closed = closed;
            t.Pathfinding_F = f;
            t.Pathfinding_G = g;
            t.Pathfinding_Parent = parent;
        }
    }

    enum PathfindingMode
    {
        Precise,
        FindClosestIfDirectIsImpossible,
        FindAnyPrecise
    }
}
       