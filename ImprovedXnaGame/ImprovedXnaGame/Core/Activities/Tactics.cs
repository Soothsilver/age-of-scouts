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
    class Tactics : Goals
    {
        private bool standMyGround;
        private Unit owner;
        public LinkedList<Vector2> PathingCoordinates;

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

        public override void Reset()
        {
            base.Reset();
            PathingCoordinates = null;
        }

        private void ResetBoth()
        {
            this.Reset();
            owner.Activity.Reset();
        }

        public void RecalculateAndDetermineActivity()
        {
            if (this.Idle)
            {
                owner.Strategy.RecalculateAndDetermineTactics();
            }
            if (this.Idle) // check again, because recalculating strategy may have caused us to stop being idle
            {
                // And we're done.
                owner.Activity.MakeIdle();
                return;
            }
            if (AttackTarget != null)
            {
                // no attacks for now
            }

            if (MovementTarget != IntVector.Zero)
            {
                // Recalculate.
                LinkedList<Vector2> path = Pathfinding.Pathfinding.AStar(owner, AttackTarget?.FeetStdPosition ?? MovementTarget, owner.Session.Map, PathfindingMode.FindClosestIfDirectIsImpossible);
                PathingCoordinates = path;
                if (path == null || path.Count == 0)
                {
                    owner.Activity.MakeIdle();
                    this.MovementTarget = IntVector.Zero;
                    owner.Activity.Invalidate();
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
    }
}
