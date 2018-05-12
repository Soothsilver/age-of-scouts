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
        public static List<Vector2> AStar(Unit who, Vector2 targetPrecise, Map map)
        {
            PathfindingSearchId++;
            Tile start = who.Occupies;
            Tile target = map.GetTileFromStandardCoordinates(targetPrecise);
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
                    var l = ReconstructPath(who, current);
                    if (l.Count == 1 && l.First.Value == start)
                    {
                        float distance = (targetPrecise - who.FeetStdPosition).LengthSquared();
                        if (distance <= 9)
                        {
                            return null;
                        }
                    }
                    List<Vector2> result = new List<Vector2>();
                    foreach(Tile tl in l)
                    {
                        result.Add(Isomath.TileToStandard(tl.X + 0.5f, tl.Y + 0.5f));
                    }
                    result.RemoveAt(result.Count - 1);
                    result.Add(targetPrecise);
                    return result;
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

                    int tentativeGScore = current.Pathfinding_G + edge.Difficulty; // TODO diagonal (neighbour.Diagonal ? 14 : 10);
                    if (tentativeGScore >= neighbour.Pathfinding_G) continue;

                    neighbour.Pathfinding_Parent = current;
                    neighbour.Pathfinding_G = tentativeGScore;
                    neighbour.Pathfinding_F = neighbour.Pathfinding_G + Heuristic(neighbour, target);

                }
            }
            return null;
        }

        private static LinkedList<Tile> ReconstructPath(Unit who, Tile current)
        {
            LinkedList<Tile> path = new LinkedList<Tile>();
            path.AddLast(current);
            while (current.Pathfinding_Parent != null &&
                current.Pathfinding_Parent != who.Occupies)
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
}
        /*
            public static Tile GetNextTileInLine(Unit u, Tile target)
            {
                var line = AStar(u, target);
                u.PlannedPath = line;
                if (line == null) return null;
                foreach (var l in line)
                {
                    if (l != u.Tile)
                    {
                        return l;
                    }
                }
                return null;
            }*/
       /*
        internal class PathfindingTask : UnitTask
        {
            private Tile target;
            private Tile immediateTarget;
            private float timeRemaining;
            private Vector2 speed;


            public PathfindingTask(Tile target)
            {
                this.target = target;
            }

            internal override string Describe()
            {
                return "Někam jde.";
            }

            internal override bool Execute(Unit u, float elapsedSeconds)
            {
                if (immediateTarget == null)
                {
                    if (u.Tile == target)
                    {
                        return true;
                    }
                    immediateTarget = Pathfinding.GetNextTileInLine(u, target);
                    if (immediateTarget == null)
                    {
                        u.TaskQueue.Prepend(new PathfindingTask(target));
                        u.TaskQueue.Prepend(new WaitTask(0.5f));
                        return true;
                    }
                    Vector2 distt = new Vector2(immediateTarget.X + 0.5f, immediateTarget.Y + 0.5f) - u.Position;
                    timeRemaining = distt.Length() / u.Speed;
                    distt.Normalize();
                    distt *= u.Speed;
                    speed = distt;
                    u.Tile.PathfindingOccupants.Remove(u);
                    immediateTarget.PathfindingOccupants.Add(u);
                }
                timeRemaining -= elapsedSeconds;
                u.Tile.Occupants.Remove(u);
                u.Position += speed * elapsedSeconds;
                u.Tile.Occupants.Add(u);
                if (timeRemaining <= 0)
                {
                    immediateTarget = null;
                }


                return false;
            }
        }
    */
    

