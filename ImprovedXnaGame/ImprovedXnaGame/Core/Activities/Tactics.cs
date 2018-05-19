using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Animation;
using Age.Pathfinding;
using Age.World;
using Auxiliary;
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
        /// <summary>
        /// The unit will try to move as close as possible to this building, and then, if possible: construct it, repair it and drop off resources at it.
        /// </summary>
        public Building BuildTarget;
        private int FinalPointMovements = 0;
        /// <summary>
        /// The unit will try to gather resources from this until the unit is full or the resource object is exhausted.
        /// </summary>
        internal NaturalObject GatherTarget;

        public bool Idle => AttackTarget == null && MovementTarget == IntVector.Zero && BuildTarget == null && GatherTarget == null;

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
            GatherTarget = null;
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
            if (this.Idle && owner.Strategy.DoesStrategyExist)
            {
                owner.Strategy.DetermineNextTactics();
            }
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
                if (!BuildTarget.SelfConstructionInProgress && !BuildTarget.CanAcceptResourcesFrom(owner))
                {
                    BuildTarget = null;
                }
                else
                {
                    // Can you build ? If yes, start building and stop speed.
                    foreach (var tile in owner.Occupies.Neighbours.All)
                    {
                        if (tile.BuildingOccupant == BuildTarget)
                        {
                            owner.Activity.BuildingWhat = BuildTarget;
                            owner.Activity.SecondsUntilNextRecalculation = 0.5f;
                        }
                    }
                    // Otherwise pick movement targets
                    if (owner.Activity.BuildingWhat == null)
                    {
                        targetTiles = BuildTarget.GetFreeSpotsForBuilders();
                    }
                }
            }
            if (GatherTarget != null)
            {
                if (!owner.CanNowGatherFrom(GatherTarget))
                {
                    GatherTarget = null;
                }
                else
                {
                    // Can you gather ? If yes, start gather and stop speed.
                    foreach (var tile in owner.Occupies.Neighbours.All)
                    {
                        if (tile.NaturalObjectOccupant == GatherTarget)
                        {
                            owner.Activity.GatheringFrom = GatherTarget;
                            owner.Activity.SecondsUntilNextRecalculation = 0.5f;
                        }
                    }
                    // Otherwise pick movement targets
                    if (owner.Activity.GatheringFrom == null)
                    {
                        targetTiles = GatherTarget.GetFreeSpotsForGatherers();
                    }
                }
            }
            if (MovementTarget != IntVector.Zero || AttackTarget != null || targetTiles != null)
            {
                // Recalculate.
                LinkedList<Vector2> path;

                PerformanceCounter.StartMeasurement(PerformanceGroup.Pathfinding);
                if (targetTiles != null) {
                    path  = Pathfinding.Pathfinding.DijkstraMultiple(owner, targetTiles, owner.Session.Map);
                } else {
                    path  = Pathfinding.Pathfinding.AStar(owner, AttackTarget?.FeetStdPosition ?? MovementTarget, owner.Session.Map, PathfindingMode.FindClosestIfDirectIsImpossible);
                }
                PerformanceCounter.EndMeasurement(PerformanceGroup.Pathfinding);
            
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
            else if (GatherTarget != null)
            {
                Reset();
                owner.Strategy.GatherTarget = null;
                owner.Activity.Invalidate();
            }
            else if (BuildTarget != null)
            {
                Reset();
                owner.Activity.Invalidate();
            }

        }

        public override string ToString()
        {
            if (AttackTarget != null)
            {
                return "Attack " + AttackTarget.ToString();
            }
            else if (MovementTarget != IntVector.Zero)
            {
                return "Move to " + MovementTarget;
            }
            else if (GatherTarget != null)
            {
                return "Gather from " + GatherTarget;
            }
            else if (BuildTarget != null)
            {
                return "Attend building: " + BuildTarget;
            }
            else if (Idle)
            {
                return "Idle";
            }
            else
            {
                return "UNIDENTIFIED TACTICS";
            }
        }
    }
}
