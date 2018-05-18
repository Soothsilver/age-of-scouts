using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Animation;
using Age.Pathfinding;
using Age.World;
using Microsoft.Xna.Framework;

namespace Age.Core.Activities
{
    /// <summary>
    /// Represents a short-term goal of this unit. For example, a gatherer's short term goal is to reach the drop site or to mine until they're full.
    /// For many actions, a unit's short-term goal might be the same as its strategy.
    /// A unit's tactics are reevaluated when its activity timeout elapses.
    /// A unit's strategy is not reevaluated until its short-term goal is completed or invalidated.
    /// </summary>
    class Tactics
    {
        private bool standMyGround;
        private Unit owner;
        public LinkedList<Vector2> PathingCoordinates;        
        public IntVector MovementTarget;
        public Unit AttackTarget;
        public Building BuildTarget;
        private int FinalPointMovements = 0;

        public bool Idle => AttackTarget == null && MovementTarget == IntVector.Zero && BuildTarget == null;

        public Tactics(Unit owner)
        {
            this.owner = owner;
        }

        public void ResetTo(Unit target, bool standGround)
        {
            ResetBoth();
            this.AttackTarget = target;
            this.standMyGround = standGround;
        }

        public void Reset()
        {
            MovementTarget = IntVector.Zero;
            AttackTarget = null;
            BuildTarget = null;
            standMyGround = false;
            PathingCoordinates = null;
            FinalPointMovements = 0;
        }

        private void ResetBoth()
        {
            this.Reset();
            owner.Activity.ResetActions();
            owner.Activity.Invalidate();
        }

        public void RecalculateAndDetermineActivity()
        {
            owner.Activity.ResetActions();
            if (this.Idle)
            {
                // And we're done.
                owner.Activity.StopForever();
                return;
            }
            if (AttackTarget != null)
            {
                if (AttackTarget.Broken)
                {
                    ResetBoth();
                    RecalculateAndDetermineActivity();
                    return;
                }
                if (owner.CanRangeAttack(AttackTarget))
                {
                    owner.Activity.Speed = Vector2.Zero;
                    owner.Activity.SecondsUntilNextRecalculation = 1;
                    owner.Activity.AttackTarget = AttackTarget;
                    return;
                }
            }
            HashSet<Vector2> targetTiles = null;
            if (BuildTarget != null)
            {
                // Can you build ? If yes, start building and stop speed.
                foreach (var tile in owner.Occupies.Neighbours.All)
                {
                    if (tile.BuildingOccupant == BuildTarget)
                    {
                        owner.Activity.BuildingWhat = BuildTarget;
                    }
                }
                // Otherwise pick movement targets
                if (owner.Activity.BuildingWhat == null)
                {
                    targetTiles = BuildTarget.GetFreeSpotsForBuilders();
                }
            }

            if (MovementTarget != IntVector.Zero || AttackTarget != null || targetTiles != null)
            {
                // Recalculate.
                LinkedList<Vector2> path;
                if (targetTiles != null) {
                    path  = Pathfinding.Pathfinding.DijkstraMultiple(owner, targetTiles, owner.Session.Map);
                } else {
                    path  = Pathfinding.Pathfinding.AStar(owner, AttackTarget?.FeetStdPosition ?? MovementTarget, owner.Session.Map, PathfindingMode.FindClosestIfDirectIsImpossible);
                }
                if (path != null && path.Count == 1)
                {
                    FinalPointMovements++;
                    if (FinalPointMovements >= 10)
                    {
                        path = null;
                    }
                }
                PathingCoordinates = path;
                if (path == null || path.Count == 0)
                {
                    MovementBlocked();
                }
                else if (path.Count > 0)
                {
                    Vector2 toNearestPoint = path.First.Value - owner.FeetStdPosition;
                    Vector2 speed = new Vector2(toNearestPoint.X, toNearestPoint.Y);
                    speed.Normalize();
                    speed *= owner.Speed * (owner.Occupies.NaturalObjectOccupant?.SpeedMultiplier ?? 1);
                    owner.Activity.SecondsUntilNextRecalculation = toNearestPoint.X / speed.X;
                    if (float.IsNaN(owner.Activity.SecondsUntilNextRecalculation))
                    {
                        owner.Activity.SecondsUntilNextRecalculation = toNearestPoint.Y / speed.Y;
                    }
                    owner.Activity.Speed = speed;
                }
            }

        }

        private void MovementBlocked()
        {
            if (MovementTarget != IntVector.Zero)
            {
                MovementTarget = IntVector.Zero;
                RecalculateAndDetermineActivity();
            }

        }
    }
}
