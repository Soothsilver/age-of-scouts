using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.Core
{
    internal class Projectile
    {
        private const float GRAVITY = 9.81f;

        public Vector2 Position;
        public float YSpeed;
        public Vector2 Speed;
        public bool Lost;
        public float Height;
        public Entity Source;
        public float TimeDilation = 1;
        public ProjectileKind ProjectileKind = ProjectileKind.Hadrak;

        public Projectile(Vector2 startingPosition, Vector2 target, Entity source)
        {
            float scalarSpeed = Tile.WIDTH * 4.5f;
            this.Position = startingPosition;
            this.Speed = target - startingPosition;
            this.Speed.Normalize();
            this.Source = source;
            this.Speed *= scalarSpeed;
            float time = (target - startingPosition).Length() / scalarSpeed;
            this.YSpeed = time * GRAVITY / 2;
        }

        public void Draw(IScreenInformation screen)
        {
            if (this.Height > 0)
            {
                Primitives.FillCircle(
                    Isomath.StandardToScreen(new Vector2(Position.X, Position.Y - Height * 300), screen),
                    (int) (4 * screen.ZoomLevel), ProjectileKind == ProjectileKind.HeavyMud ? Color.Brown : Color.White);
            }
        }

        public void DrawShadow(IScreenInformation screen)
        {
            if (this.Height > 0)
            {
                Primitives.FillCircle(Isomath.StandardToScreen(Position, screen), (int) (4 * screen.ZoomLevel),
                    Color.Black.Alpha(150));
            }
        }

        public void Update(Session session, float elapsedSeconds)
        {
            elapsedSeconds = elapsedSeconds * TimeDilation;
            this.YSpeed -= GRAVITY * elapsedSeconds;
            this.Height += this.YSpeed * elapsedSeconds;
            this.Position += this.Speed * elapsedSeconds;
            if (!this.Lost)
            {
                Tile whereAmI = session.Map.GetTileFromStandardCoordinates(this.Position);
                if (this.Height < -2f)
                {
                    this.Lost = true;
                    // Hit the ground.
                }
                else if (whereAmI == null)
                {
                    // Do nothing
                }
                else if (whereAmI.BuildingOccupant != null && session.AreEnemies(Source, whereAmI.BuildingOccupant))
                {
                    int modifiedDamage = 10;
                    if (ProjectileKind == ProjectileKind.HeavyMud)
                    {
                        modifiedDamage *= 3;
                    }
                    whereAmI.BuildingOccupant.TakeDamage(modifiedDamage, Source);
                    this.Lost = true;
                }
             // It is too harsh to prevent projectiles this way because it results in bad gameplay.
             //   else if (whereAmI.PreventsProjectiles)
             //   {
             //       this.Lost = true;
             //   }
                else if (this.Height >= 1)
                {
                    // Do nothing.
                }
                else
                {
                    foreach (var target in session.AllUnits)
                    {
                        if (session.AreEnemies(Source, target) &&
                            target.Hitbox.Contains((int) this.Position.X, (int) this.Position.Y))
                        {
                            target.TakeDamage(10, Source);
                            this.Lost = true;
                            break;
                        }
                    }
                }
            }
        }

    }

    enum ProjectileKind
    {
        /// <summary>
        /// Deals standard damage, with no special bonuses.
        /// </summary>
        Hadrak,
        /// <summary>
        /// Deals crushing damage, effective against buildings.
        /// </summary>
        HeavyMud
    }
}