using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Animation;
using Age.HUD;
using Age.Pathfinding;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Core
{
    class Unit : Entity
    {
        public SpriteInstance Sprite;
        public UnitTemplate UnitTemplate;
        public override string Name { get; }
        public Activity Activity = new Activity();
        public Tile Occupies;
        public bool Broken;
        public float Speed = Tile.WIDTH;
        public int HP = 50;
        internal int MaxHP = 50;
        public Stance Stance = Stance.Aggressive;

        public Unit(string name, Troop controller, UnitTemplate unitTemplate, Vector2 feetPosition) : base(TextureName.None, feetPosition)
        {
            Name = name;
            Controller = controller;
            Session = Controller.Session;
            UnitTemplate = unitTemplate;
            Sprite = unitTemplate.Sprite.CreateInstance(controller.LightColor);
        }

        public Rectangle Hitbox
        {
            get
            {
                return GetHitboxFromFeet(this.FeetStdPosition);
            }
        }
        
        public override Texture2D BottomBarTexture => SpriteCache.GetColoredTexture(UnitTemplate.Icon, Controller.LightColor);

        public Rectangle GetHitboxFromFeet(Vector2 feet)
        {
            return new Rectangle((int)feet.X - 10, (int)feet.Y - 40, 20, 40);

        }

        internal void TakeDamage(int dmg, Unit source)
        {
            this.HP -= dmg;
            if (this.HP <= 0)
            {
                this.Broken = true;
                this.Occupies.BrokenOccupants.Add(new Corpse(this));
                this.Occupies.Occupants.Remove(this);
                this.Occupies = null;
            }

            // Aggressive
            if (Stance == Stance.Aggressive)
            {
                if (!Activity.HasAGoal)
                {
                    Activity.AttackTarget = source;
                    Activity.SecondsUntilNextRecalculation = 0;
                }
            }
        }

        public override List<ConstructionOption> ConstructionOptions => this.UnitTemplate.CanBuildStuff ? ConstructionOption.PracantOptions : ConstructionOption.None;

        public Texture2D AnimationTick(float elapsedSeconds)
        {
            return Sprite.AnimationTick(elapsedSeconds);
        }

        public void Autoaction(Session session, float elapsedSeconds)
        {
            // All stances          
            if (!this.Activity.HasAGoal && Stance != Stance.Stealthy)
            {
                foreach (var unit in session.AllUnits)
                {
                    if (this.CanRangeAttack(session, unit) && !this.Activity.IsMoving)
                    {
                        if (unit.Occupies.NaturalObjectOccupant?.EntityKind == EntityKind.TallGrass)
                        {
                            // Don't autoattack into tall grass.
                        }
                        else
                        {
                            if (this.Stance == Stance.Aggressive)
                            {
                                this.Activity.AttackTarget = unit;
                            }
                            this.AttackIfAble(session, unit, elapsedSeconds);
                        }
                    }
                    if (unit.Controller == this.Controller && unit.Activity.AttackTarget != null
                        && (this.Stance == Stance.Aggressive) && unit.FeetStdPosition.WithinDistance(this.FeetStdPosition, 5*Tile.HEIGHT))
                    {
                        this.Activity.AttackTarget = unit.Activity.AttackTarget;
                        this.Activity.SecondsUntilNextRecalculation = 0.2f;
                    }
                }
            }
            
        }

        private void AttackIfAble(Session session, Unit target, float elapsedSeconds)
        {
            this.Activity.AttackingInProgress = true;
            if (this.Activity.SecondsUntilRecharge <= 0)
            {
                // Fire
                session.SpawnProjectile(new Projectile(this.FeetStdPosition + new Vector2(0, -10), target.FeetStdPosition + new Vector2(0, -10), this));
                // Recharge
                this.Activity.SecondsUntilRecharge = 4; // recharge in four seconds
            }
            else
            {
                this.Activity.SecondsUntilRecharge -= elapsedSeconds;
                bool facingRight = (target.FeetStdPosition.X - this.FeetStdPosition.X) > 0;
                if (this.Activity.SecondsUntilRecharge < 2)
                {
                    if (facingRight)
                    {
                        this.Sprite.CurrentAnimation = AnimationListKey.ReadyToAttackRight;
                    }
                    else
                    {
                        this.Sprite.CurrentAnimation = AnimationListKey.ReadyToAttackLeft;
                    }
                }
                else
                {
                    if (facingRight)
                    {
                        this.Sprite.CurrentAnimation = AnimationListKey.AfterAttackRight;
                    }
                    else
                    {
                        this.Sprite.CurrentAnimation = AnimationListKey.AfterAttackLeft;
                    }
                }
            }
        }

        internal void Movement(Session session, float elapsedSeconds)
        {

            if (Activity.AttackTarget != null)
            {
                if (Activity.AttackTarget.Broken)
                {
                    Activity.AttackTarget = null;
                }
            }
            if (Activity.SecondsUntilNextRecalculation <= 0)
            {
                if (Activity.HasAGoal)
                {
                    if (Activity.AttackTarget != null && this.CanRangeAttack(session, Activity.AttackTarget))
                    {
                        Activity.Speed = Vector2.Zero;
                    }
                    else
                    {
                        // Recalculate.
                        LinkedList<Vector2> path = Pathfinding.Pathfinding.AStar(this, Activity.AttackTarget == null ? Activity.MovementTarget
                            : Activity.AttackTarget.FeetStdPosition, session.Map, PathfindingMode.FindClosestIfDirectIsImpossible);
                        Activity.PathingCoordinates = path;
                        if (path == null)
                        {
                            Activity.MovementTarget = Vector2.Zero;
                            Activity.AttackTarget = null;
                            Activity.Speed = Vector2.Zero;
                            Sprite.CurrentAnimation = AnimationListKey.Idle;
                        }
                        else if (path.Count > 0)
                        {
                            Vector2 toNearestPoint = path.First.Value - this.FeetStdPosition;
                            Vector2 speed = new Vector2(toNearestPoint.X, toNearestPoint.Y);
                            speed.Normalize();
                            speed *= this.Speed * (this.Occupies.NaturalObjectOccupant?.SpeedMultiplier ?? 1);
                            Activity.SecondsUntilNextRecalculation = toNearestPoint.X / speed.X;
                            if (float.IsNaN(Activity.SecondsUntilNextRecalculation))
                            {
                                Activity.SecondsUntilNextRecalculation = toNearestPoint.Y / speed.Y;
                            }
                            Activity.Speed = speed;
                        }
                        else
                        {
                            Activity.MovementTarget = Vector2.Zero;
                            Activity.Speed = Vector2.Zero;
                            Activity.AttackTarget = null;
                            Sprite.CurrentAnimation = AnimationListKey.Idle;
                        }
                    }
                }
                else
                {
                    Activity.Speed = Vector2.Zero;
                    Sprite.CurrentAnimation = AnimationListKey.Idle;
                }
            }
            else
            {
                if (Activity.AttackTarget != null && this.CanRangeAttack(session, Activity.AttackTarget))
                {
                    this.AttackIfAble(session, Activity.AttackTarget, elapsedSeconds);
                }
                else if (Activity.Speed != Vector2.Zero)
                {
                    Activity.SecondsUntilNextRecalculation -= elapsedSeconds;
                    Vector2 newPosition = FeetStdPosition + Activity.Speed * elapsedSeconds;
                    Tile targetTile = session.Map.GetTileFromStandardCoordinates(newPosition);
                    MoveLegality moveLegality = MovementRules.IsMoveLegal(this, FeetStdPosition, newPosition, targetTile, session);
                    if (moveLegality.IsLegal)
                    {
                        Sprite.CurrentAnimation = Sprite.DetermineAnimationKeyFromMovement(newPosition, FeetStdPosition);
                        this.UpdatePositionTo(targetTile, newPosition);
                    }
                    else
                    {
                        // Target moves
                        Unit obstacle = moveLegality.ObstacleUnit;
                        if (obstacle != null && obstacle.Controller == this.Controller && !obstacle.Activity.IsMoving)
                        {
                            obstacle.Activity.Speed = R.RandomUnitVector() * Speed * 1.8f;
                            obstacle.Activity.SecondsUntilNextRecalculation = 0.1f;
                        }
                        else
                        {
                            // Random movement
                            Activity.Speed = R.RandomUnitVector() * Speed * 1.8f;
                            Activity.SecondsUntilNextRecalculation = 0.1f;
                        }
                    }
                }

            }
        }

        internal Tooltip GetTooltip()
        {
            return new Tooltip(this.Name + " (" + this.UnitTemplate.Name + ")", this.UnitTemplate.Description);
        }

        private bool CanRangeAttack(Session session, Unit attackTarget)
        {
            return 
                this.UnitTemplate.CanAttack &&
                attackTarget != null && session.AreEnemies(this, attackTarget)
                && !attackTarget.Broken &&
                !this.Broken 
                && this.FeetStdPosition.WithinDistance(attackTarget.FeetStdPosition, 5 * Tile.HEIGHT);
                // && line of effect
        }

        internal void SwitchControllerTo(Troop controller)
        {
            this.Controller = controller;
            this.Sprite.Color = controller.LightColor;
        }

        private void UpdatePositionTo(Tile goingTo, Vector2 newPosition)
        {
            if (goingTo != this.Occupies)
            {
                this.Occupies.Occupants.Remove(this);
                this.Occupies = null;
                goingTo.Occupants.Add(this);
                this.Occupies = goingTo;
            }
            this.FeetStdPosition = newPosition;
        }
    }
}
