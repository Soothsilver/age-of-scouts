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
        public float SecondsUntilGatherRecharge;
        public AttackableEntity AttackTarget;
        public bool AttackingInProgress;
        private Unit owner;
        public Building BuildingWhat;
        public NaturalObject GatheringFrom;

        public bool Idle => AttackTarget == null && Speed == Vector2.Zero && BuildingWhat == null && GatheringFrom == null;

        public override string ToString()
        {
            string all = "";
            if (Idle)
            {
                all += "No immediate activity";
            }
            else
            {
                if (AttackTarget != null)
                {
                    all += "Attacking. ";
                }
                if (BuildingWhat != null)
                {
                    all += "Building. ";
                }
                if (GatheringFrom != null)
                {
                    all += "Gathering. ";
                }
                if (Speed != Vector2.Zero)
                {
                    all += "Moving. ";
                }
            }
            all += "\nRecalc in: " + SecondsUntilNextRecalculation;
            return all;
        }

        public Activity(Unit owner)
        {
            this.owner = owner;
        }

        public void ResetActions()
        {
            this.AttackingInProgress = false;
            this.AttackTarget = null;
            this.BuildingWhat = null;
            this.GatheringFrom = null;
            owner.Sprite.SetCurrentAnimation(AnimationListKey.Idle, false);
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
                    owner.Sprite.SetCurrentAnimation(AnimationListKey.BuildRight, owner.Sprite.ShouldFlip(owner.FeetStdPosition, BuildingWhat.FeetStdPosition));
                    if (Settings.Instance.Aegis)
                    {
                        BuildingWhat.SelfConstructionProgress = 1;
                    }
                }
                if (!BuildingWhat.SelfConstructionInProgress)
                {
                    if (BuildingWhat.CanAcceptResourcesFrom(owner))
                    {
                        BuildingWhat.TakeResourcesFrom(owner);
                    }
                    BuildingWhat = null;
                    SecondsUntilNextRecalculation = 0;
                }
            }
            if (GatheringFrom != null)
            {
                var tuple = owner.Sprite.DetermineAnimationKeyFromGathering(GatheringFrom.FeetStdPosition, owner.FeetStdPosition, GatheringFrom);
                owner.Sprite.SetCurrentAnimation(tuple.Item1, tuple.Item2);
                if (SecondsUntilGatherRecharge <= 0)
                {
                    GatheringFrom.TransferOneResourceTo(owner);
                    SecondsUntilNextRecalculation = 0;
                    SecondsUntilGatherRecharge = 0.5f;
                }
                else
                {
                    SecondsUntilGatherRecharge -= elapsedSeconds;
                }
            }

            if (Speed != Vector2.Zero)
            {
                Vector2 newPosition = owner.FeetStdPosition + Speed * (owner.Occupies.SpeedMultiplier) * elapsedSeconds;
                Tile targetTile = session.Map.GetTileFromStandardCoordinates(newPosition);
                MoveLegality moveLegality = MovementRules.IsMoveLegal(owner, owner.FeetStdPosition, newPosition, targetTile, session);
                if (moveLegality.IsLegal)
                {
                    var tuple = owner.Sprite.DetermineAnimationKeyFromMovement(newPosition, owner.FeetStdPosition);
                    owner.Sprite.SetCurrentAnimation(tuple.Item1, tuple.Item2);
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
            owner.Sprite.SetCurrentAnimation(AnimationListKey.Idle, false);
        }

        internal void QuicklyWalkTo(Vector2 whereToStandard)
        {
            this.ResetActions();
            var toNearestPoint = whereToStandard - owner.FeetStdPosition;
            var speed = whereToStandard - owner.FeetStdPosition;
            speed.Normalize();
            this.Speed = owner.Speed * 2 * speed;
            SecondsUntilNextRecalculation = toNearestPoint.X / Speed.X;
            if (float.IsNaN(SecondsUntilNextRecalculation))
            {
                SecondsUntilNextRecalculation = toNearestPoint.Y / Speed.Y;
            }
        }
    }
}