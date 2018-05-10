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
        public Unit Source;
        public Projectile(Vector2 startingPosition, Vector2 target, Unit source)
        {
            float scalarSpeed = Tile.WIDTH * 3;
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
            Primitives.FillCircle(Isomath.StandardToScreen(new Vector2(Position.X, Position.Y - Height * 80), screen), (int)(4 * screen.ZoomLevel), Color.Gray);
        }
        public void DrawShadow(IScreenInformation screen)
        {
            Primitives.FillCircle(Isomath.StandardToScreen(Position, screen), (int)(4 * screen.ZoomLevel), Color.Black.Alpha(150));
        }
        public void Update(Session session, float elapsedSeconds)
        {
            this.YSpeed -= GRAVITY * elapsedSeconds;
            this.Height += this.YSpeed * elapsedSeconds;
            this.Position += this.Speed * elapsedSeconds;           
            if (!this.Lost)
            {
                Tile whereAmI = session.Map.GetTileFromStandardCoordinates(this.Position);
                if (whereAmI.PreventsProjectiles)
                {
                    this.Lost = true;
                    return;
                }
                if (this.Height >= 1)
                {
                    // too high
                    return;
                }
                foreach (var target in session.AllUnits)
                {
                    if (session.AreEnemies(Source, target) && target.Hitbox.Contains((int)this.Position.X, (int)this.Position.Y)) 
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