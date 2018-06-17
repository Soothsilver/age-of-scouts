using Age.Core;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Priority_Queue;


namespace Age.Pathfinding
{
    class Pathfinding
    {
        private static int mapTotalSize;
        private static FastPriorityQueue<Tile> openSet;
        private static int pathfindingSearchId = 0;
        public static LinkedList<Vector2> AStar(Unit who, Vector2 targetPrecise, Map map, PathfindingMode mode)
        {
            InitializeOpenSetIfNecessary(map);
            pathfindingSearchId++;
            Tile start = who.Occupies;
            Tile target = map.GetTileFromStandardCoordinates(targetPrecise);
            if (target == null)
            {
                return null;
            }
            Tile closestToTargetSoFar = start;
            openSet.Clear();
            int initH = Pathfinding.Heuristic(start, target);
            SetPathfindingInformation(start, pathfindingSearchId, false, 0, initH, null);
            openSet.Enqueue(start, initH);
            int closestToTargetHeuristicSoFar = initH;

            while (openSet.Count > 0)
            {
                Tile current = openSet.Dequeue();
                if (current == target)
                {
                    return ReconstructPrettyPath(start, target, who.FeetStdPosition, targetPrecise);
                }
                current.Pathfinding_Closed = true;
                foreach (var edge in current.Neighbours.Traversable)
                {
                    Tile neighbour = edge.Destination;
                    if (neighbour.Pathfinding_EncounteredDuringSearch == pathfindingSearchId &&
                        neighbour.Pathfinding_Closed) continue;
                    if (neighbour.PreventsMovement) continue;

                    if (neighbour.Pathfinding_EncounteredDuringSearch < pathfindingSearchId)
                    {
                        SetPathfindingInformation(neighbour, pathfindingSearchId, false, int.MaxValue, int.MaxValue, current);
                        openSet.Enqueue(neighbour, int.MaxValue);
                    }

                    int tentativeGScore = current.Pathfinding_G + edge.Difficulty; 
                    if (tentativeGScore >= neighbour.Pathfinding_G) continue;

                    neighbour.Pathfinding_Parent = current;
                    neighbour.Pathfinding_G = tentativeGScore;
                    int heuristic = Heuristic(neighbour, target);
                    neighbour.Pathfinding_F = neighbour.Pathfinding_G + heuristic;
                    openSet.UpdatePriority(neighbour, neighbour.Pathfinding_F);
                    if (heuristic < closestToTargetHeuristicSoFar)
                    {
                        closestToTargetSoFar = neighbour;
                        closestToTargetHeuristicSoFar = heuristic;
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
            InitializeOpenSetIfNecessary(map);
            pathfindingSearchId++;
            Tile start = who.Occupies;
            foreach(Vector2 vector in targetTiles)
            {
                Tile target = map.GetTileFromStandardCoordinates(vector);
                target.Pathfinding_IsTargetDuringThisSearch = pathfindingSearchId;
                target.Pathfinding_TargetPreciseLocation = vector;
            }

            openSet.Clear();
            SetPathfindingInformation(start, pathfindingSearchId, false, 0, 0, null);
            openSet.Enqueue(start, 0);

            while (openSet.Count > 0)
            {
                Tile current = openSet.Dequeue();
                if (current.Pathfinding_IsTargetDuringThisSearch == pathfindingSearchId)
                {
                    return ReconstructPrettyPath(start, current, who.FeetStdPosition, current.Pathfinding_TargetPreciseLocation);
                }
                current.Pathfinding_Closed = true;
                foreach (var edge in current.Neighbours.Traversable)
                {
                    Tile neighbour = edge.Destination;
                    if (neighbour.Pathfinding_EncounteredDuringSearch == pathfindingSearchId &&
                        neighbour.Pathfinding_Closed) continue;
                    if (neighbour.PreventsMovement) continue;

                    if (neighbour.Pathfinding_EncounteredDuringSearch < pathfindingSearchId)
                    {
                        SetPathfindingInformation(neighbour, pathfindingSearchId, false, int.MaxValue, int.MaxValue, current);
                        openSet.Enqueue(neighbour, int.MaxValue);
                    }

                    int tentativeGScore = current.Pathfinding_G + edge.Difficulty;
                    if (tentativeGScore >= neighbour.Pathfinding_G) continue;

                    neighbour.Pathfinding_Parent = current;
                    neighbour.Pathfinding_G = tentativeGScore;
                    neighbour.Pathfinding_F = neighbour.Pathfinding_G;
                    openSet.UpdatePriority(neighbour, neighbour.Pathfinding_F);
                }
            }
            return null;
        }

        private static void InitializeOpenSetIfNecessary(Map map)
        {
            int totalSize = map.Width * map.Height;
            if (mapTotalSize < totalSize)
            {
                openSet = new FastPriorityQueue<Tile>(totalSize);
                mapTotalSize = totalSize;
            }
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
            return Math.Max(Math.Abs(start.X - target.X), Math.Abs(start.Y - target.Y)) * 64;
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
       