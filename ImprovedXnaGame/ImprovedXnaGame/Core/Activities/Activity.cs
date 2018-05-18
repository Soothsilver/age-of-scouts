using System;
using System.Collections.Generic;
using Age.Animation;
using Age.Pathfinding;
using Microsoft.Xna.Framework;

namespace Age.Core.Activities
{
    /// <summary>
    /// A unit's activity is the thing it's immediately doing right now, such as moving a few pixels in a direction or performing an attack. A unit's activity doesn't
    /// last very long and when it completes, the unit's tactics are reevaluated to determine its next activity.
    /// </summary>
    class Activity
    {
        public float SecondsUntilNextRecalculation;

        public Vector2 Speed;
        public float SecondsUntilRecharge;
        public Unit AttackTarget;
        public bool AttackingInProgress;
        private Unit owner;
        public Building BuildingWhat;

        public bool Idle => AttackTarget == null && Speed == Vector2.Zero;

        public Activity(Unit owner)
        {
            this.owner = owner;
        }

        public void ResetActions()
        {
            this.AttackingInProgress = false;
            this.AttackTarget = null;
            this.BuildingWhat = null;
            owner.Sprite.CurrentAnimation = AnimationListKey.Idle;
            this.Speed = Vector2.Zero;
        }
        public void Invalidate()
        {
            this.SecondsUntilNextRecalculation = 0;
        }
        
        public void Execute(float elapsedSeconds)
        {
            Session session = owner.Session;
            SecondsUntilNextRecalculation -= elapsedSeconds;
            if (AttackTarget != null && owner.CanRangeAttack(AttackTarget))
            {
                owner.AttackIfAble(session, AttackTarget, elapsedSeconds);
            }
            if (BuildingWhat != null)
            {
                if (BuildingWhat.SelfConstructionInProgress && BuildingWhat.NoUnitsOnThisBuilding)
                {
                    BuildingWhat.SelfConstructionProgress += elapsedSeconds / BuildingWhat.Template.SecondsToBuild;
                }
                if (!BuildingWhat.SelfConstructionInProgress)
                {
                    BuildingWhat = null;
                    SecondsUntilNextRecalculation = 0;
                }
            }

            if (Speed != Vector2.Zero)
            {
                Vector2 newPosition = owner.FeetStdPosition + Speed * elapsedSeconds;
                Tile targetTile = session.Map.GetTileFromStandardCoordinates(newPosition);
                MoveLegality moveLegality = MovementRules.IsMoveLegal(owner, owner.FeetStdPosition, newPosition, targetTile, session);
                if (moveLegality.IsLegal)
                {
                    owner.Sprite.CurrentAnimation = owner.Sprite.DetermineAnimationKeyFromMovement(newPosition, owner.FeetStdPosition);
                    owner.UpdatePositionTo(targetTile, newPosition);
                }
                else
                {
                    // Target moves
                    Unit obstacle = moveLegality.ObstacleUnit;
                    if (obstacle != null && obstacle.Controller == owner.Controller && obstacle.Activity.Speed == Vector2.Zero)
                    {
                        obstacle.Activity.Speed = R.RandomUnitVector() * obstacle.Speed * 1.8f;
                        obstacle.Activity.SecondsUntilNextRecalculation = 0.1f;
                        Speed = Vector2.Zero;
                        SecondsUntilNextRecalculation = 0.05f;
                    }
                    else
                    {
                        // Random movement
                        Speed = R.RandomUnitVector() * owner.Speed * 1.8f;
                        SecondsUntilNextRecalculation = 0.1f;
                    }
                }
            }
        }


        public void StopForever()
        {
            this.ResetActions();
            this.SecondsUntilNextRecalculation = Single.MaxValue;
            owner.Sprite.CurrentAnimation = AnimationListKey.Idle;
        }

        internal void QuicklyWalkTo(Vector2 whereToStandard)
        {
            this.ResetActions();
            var toNearestPoint = whereToStandard - owner.FeetStdPosition;
            var speed = whereToStandard - owner.FeetStdPosition;
            speed.Normalize();
            this.Speed = owner.Speed * 2 * speed;
            SecondsUntilNextRecalculation = toNearestPoint.X / speed.X;
            if (float.IsNaN(SecondsUntilNextRecalculation))
            {
                SecondsUntilNextRecalculation = toNearestPoint.Y / speed.Y;
            }
        }
    }
}