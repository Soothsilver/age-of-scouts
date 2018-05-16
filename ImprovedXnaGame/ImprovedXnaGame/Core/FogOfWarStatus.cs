using System;
using Age.World;
using Microsoft.Xna.Framework;

namespace Age.Core
{
    enum FogOfWarStatus
    {
        Clear,
        Grey,
        Black
    }
    class FogRevealer
    {
        public Vector2 StandardCoordinates;
        public float TileRange;
        public FogRevealer(Vector2 coordinates, float range)
        {
            StandardCoordinates = coordinates;
            TileRange = range;
        }
        public void Update(Map map)
        {
            FogOfWarMechanics.RevealFogOfWar(StandardCoordinates, (int)(Tile.HEIGHT * TileRange), map, true, 2);
        }
    }
    public static class FogOfWarMechanics
    {

        internal static void RevealFogOfWar(Vector2 source, int pixelRange, Map map, bool createDebugPoints, float cleartime = 2, bool fromAir = false)
        {
            Tile tile = map.GetTileFromStandardCoordinates(source);
            tile.SetClearFogStatus(cleartime);
            for (float angle = 0; angle <= 2 * Math.PI; angle += Microsoft.Xna.Framework.MathHelper.Pi / 40)
            {
                float dx = (float)Math.Cos(angle) * 10;
                float dy = (float)Math.Sin(angle) * 10;
                float x = source.X;
                float y = source.Y;
                for (int i = 0; i < 30; i++)
                {
                    x += dx; y += dy;
                    if ((new Vector2(x, y) - source).LengthSquared() >= pixelRange * pixelRange) break;
                    if (createDebugPoints && Settings.Instance.ShowDebugPoints)
                    {
                        Debug.DebugPoints.Coordinates.Add(new Vector2(x, y));
                    }
                    Tile tl = map.GetTileFromStandardCoordinates(new Vector2(x,y));
                    if (tl == null)
                    {
                        break;
                    }
                    tl.SetClearFogStatus(cleartime);
                    if (tl.NaturalObjectOccupant?.EntityKind == EntityKind.UntraversableTree && !fromAir)
                    {
                        break;
                    }
                }
            }
           
        }
    }
}