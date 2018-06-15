using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Animation;
using Age.Core.Activities;
using Age.HUD;
using Age.Pathfinding;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Core
{
    class Unit : AttackableEntity
    {
        public SpriteInstance Sprite;
        public UnitTemplate UnitTemplate;
        public override string Name { get; }
        public Strategy Strategy;
        public Tactics Tactics;
        public Activity Activity;
        public Tile Occupies;
        public float Speed => UnitTemplate.Speed;
        public Stance Stance = Stance.Aggressive;
        public Resource CarryingResource;
        public int CarryingHowMuchResource;
        public override string EncyclopediaFilename => UnitTemplate.EncyclopediaFilename;

        public Unit(string name, Troop controller, UnitTemplate unitTemplate, Vector2 feetPosition) : base(TextureName.None, feetPosition)
        {
            Name = name;
            Controller = controller;
            Session = Controller.Session;
            UnitTemplate = unitTemplate;
            Sprite = unitTemplate.Sprite.CreateInstance(controller.LightColor);
            Strategy = new Strategy(this);
            Tactics = new Tactics(this);
            Activity = new Activity(this);
        }

        /// <summary>
        /// Returns true if the unit can construct or repair the given building.
        /// </summary>
        /// <param name="construction">The building that needs to be constructed or repaired.</param>
        internal bool CanContributeToBuilding(Building construction)
        {
            return this.UnitTemplate.CanBuildStuff &&
                (construction.SelfConstructionInProgress || construction.HP < construction.MaxHP)
                && construction.Controller == this.Controller;
        }

        public Rectangle Hitbox
        {
            get
            {
                return GetHitboxFromFeet(this.FeetStdPosition);
            }
        }

        public string DebugActivityDescription => "Strategy: " + Strategy + "\nTactics: " + Tactics + "\nActivity: " + Activity;
        public override Texture2D BottomBarTexture => SpriteCache.GetColoredTexture(UnitTemplate.Icon, false, Controller.LightColor);

        public Rectangle GetHitboxFromFeet(Vector2 feet)
        {
            if (this.UnitTemplate.Id == UnitId.Katapult)
            {
                return new Rectangle((int)feet.X - 27, (int)feet.Y - 55, 54, 55);
            }
            else
            {
                return new Rectangle((int)feet.X - 10, (int)feet.Y - 40, 20, 40);
            }
        }

        public override void TakeDamage(int dmg, Entity source)
        {
            this.HP -= dmg;
            if (this.HP <= 0 && !this.Broken)
            {
                this.Broken = true;
            }

            // Aggressive
            if (Stance == Stance.Aggressive)
            {
                if (this.FullyIdle && this.CanAttack)
                {
                    if (source is Unit sourceUnit)
                    {
                        Tactics.ResetTo(sourceUnit, false);
                    }
                }
            }
        }

        public override List<ConstructionOption> ConstructionOptions
        {
            get
            {
                if (this.UnitTemplate.CanBuildStuff)
                {
                    if (this.Session.Flags.RadeninConstructionOnly)
                    {
                        return ConstructionOption.RadeninOptions;
                    }
                    else
                    {
                        return ConstructionOption.PracantOptions;
                    }
                }
                else
                {
                    return ConstructionOption.None;
                }
            }
        }

        public Texture2D AnimationTick(float elapsedSeconds)
        {
            return Sprite.AnimationTick(elapsedSeconds);
        }

        /// <summary>
        /// If this unit is fully idle, it may decide to start doing something spontaneously. This something is usually beginning attacking another unit,
        /// based on its stance. 
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="elapsedSeconds">Seconds since last update.</param>
        public void Autoaction(Session session, float elapsedSeconds)
        {
            // All stances          
            if (this.CanAttack && this.FullyIdle && Stance != Stance.Stealthy)
            {
                foreach (var unit in session.AllUnits)
                {
                    if (this.CanRangeAttack(unit, this.UnitTemplate.AttackRange))
                    {
                        if (unit.Occupies.NaturalObjectOccupant?.EntityKind == EntityKind.TallGrass)
                        {
                            // Don't autoattack into tall grass.
                        }
                        else
                        {
                            this.Tactics.ResetTo(unit, this.Stance == Stance.StandYourGround);
                        }
                    }
                    if (unit.Controller == this.Controller && 
                        unit.Tactics.AttackTarget != null &&
                        this.Stance == Stance.Aggressive && 
                        unit.FeetStdPosition.WithinDistance(this.FeetStdPosition, 5*Tile.HEIGHT))
                    {
                        this.Tactics.ResetTo(unit.Tactics.AttackTarget, false);
                    }
                }
            }
            
        }

        public bool FullyIdle => Tactics.Idle && Activity.Idle;

        public void AttackIfAble(Session session, AttackableEntity target, float elapsedSeconds)
        {
            this.Activity.AttackingInProgress = true;
            if (this.Activity.SecondsUntilRecharge <= 0)
            {
                // Fire
                if (this.UnitTemplate.Id == UnitId.Katapult)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        session.SpawnProjectile(new Projectile(this.FeetStdPosition + new Vector2(0, -20), target.FeetStdPosition + new Vector2(0, -20)
                            + R.RandomUnitVector() * R.Float() * 50
                            , this)
                        {
                            ProjectileKind = ProjectileKind.HeavyMud,
                            TimeDilation = 0.4f
                        });
                    }
                }
                else
                {
                    session.SpawnProjectile(new Projectile(this.FeetStdPosition + new Vector2(0, -20), target.FeetStdPosition + new Vector2(0, -20), this));
                }
                // Recharge
                this.Activity.SecondsUntilRecharge = 4; // recharge in four seconds
            }
            else
            {
                this.Activity.SecondsUntilRecharge -= elapsedSeconds;
                bool facingRight = (target.FeetStdPosition.X - this.FeetStdPosition.X) > 0;
                if (this.Activity.SecondsUntilRecharge < 2)
                {
                    this.Sprite.SetCurrentAnimation(AnimationListKey.ReadyToAttackRight, !facingRight);
                }
                else
                {
                    this.Sprite.SetCurrentAnimation(AnimationListKey.AfterAttackRight, !facingRight);
                }
            }
        }

        /// <summary>
        /// Returns true if this unit can be ordered to gather from this tile.
        /// </summary>
        internal bool CanBeOrderedToGatherFrom(NaturalObject naturalObject)
        {
            return this.UnitTemplate.CanGatherStuff &&
                naturalObject.ResourcesLeft > 0 &&
                naturalObject.ProvidesResource != 0;
        }
        public bool CanNowGatherFrom(NaturalObject naturalObject)
        {
             return this.CanBeOrderedToGatherFrom(naturalObject) &&
                (naturalObject.ProvidesResource != this.CarryingResource ||
                this.CarryingHowMuchResource < StaticData.CarryingCapacity);
        }

        /// <summary>
        /// Performs the unit's <see cref="Activity"/>, potentially reevaluating its <see cref="Tactics"/> and <see cref="Strategy"/>.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="elapsedSeconds">Seconds since last update.</param>
        public void DoAssignedActivity(Session session, float elapsedSeconds)
        {
            // Do invalidation first, so that we don't skip frames.
            if (Activity.AttackTarget?.Broken ?? false)
            {
                Activity.Invalidate();
            }

            if (Activity.SecondsUntilNextRecalculation <= 0)
            {
                Tactics.RecalculateAndDetermineActivity();
            }
                       
            Activity.Execute(elapsedSeconds);            
        }

        internal Tooltip GetTooltip()
        {
            return new Tooltip(this.Name + " (" + this.UnitTemplate.Name + ")", this.UnitTemplate.Description);
        }


        public override bool CanAttack => UnitTemplate.CanAttack;

        internal void SwitchControllerTo(Troop controller)
        {
            this.Controller = controller;
            this.Sprite.Color = controller.LightColor;
        }

        public void UpdatePositionTo(Tile goingTo, Vector2 newPosition)
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

        public void AttemptToExitConstructionSite(Building outsideOfBuilding)
        {
            if (this.Activity.Speed == Vector2.Zero)
            {
                List<Tile> options = new List<Tile>();
                Tile gotoWhere = null;
                foreach (var tile in this.Occupies.Neighbours.All)
                {
                    if (tile.PreventsMovement || (tile.BuildingOccupant == outsideOfBuilding))
                    {

                    }
                    else
                    {
                        options.Add(tile);
                    }
                }
                if (options.Count > 0)
                {
                    gotoWhere = options[R.Next(options.Count)];
                }
                else
                {
                    options.AddRange(this.Occupies.Neighbours.All.Where(tl => !tl.PreventsMovement));
                    if (options.Count > 0)
                    {
                        gotoWhere = options[R.Next(options.Count)];
                    }
                }
                if (gotoWhere != null)
                {
                    Vector2 m = Isomath.TileToStandard(gotoWhere.X + 0.5f, gotoWhere.Y + 0.5f);
                    this.Activity.QuicklyWalkTo(m);
                }
            }
        }

        public override string ToString()
        {
            return Name + " (" + UnitTemplate.Name + ", " + Occupies + ")";
        }
    }
}
