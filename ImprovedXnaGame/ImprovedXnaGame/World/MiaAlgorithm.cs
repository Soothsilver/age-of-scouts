using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Age.World
{
    static class MiaAlgorithm
    {
        public static LinkedList<IntVector> GetTilesInsideRectangle(Rectangle standardCoordinates, Map map)
        {
            LinkedList<IntVector> values = new LinkedList<IntVector>();
            IntVector topLeftTile = Isomath.StandardToTile(standardCoordinates.X, standardCoordinates.Y);
            IntVector topRightTile = Isomath.StandardToTile(standardCoordinates.Right, standardCoordinates.Y);
            IntVector bottomLeftTile = Isomath.StandardToTile(standardCoordinates.X, standardCoordinates.Bottom);
            IntVector bottomRightTile = Isomath.StandardToTile(standardCoordinates.Right, standardCoordinates.Bottom);

            MiaCoordinates topLeft_1 = TileToMia(topLeftTile);
            MiaCoordinates topRight_2 = TileToMia(topRightTile);
            MiaCoordinates bottomLeft_3 = TileToMia(bottomLeftTile);
            MiaCoordinates bottomRight_4 = TileToMia(bottomRightTile);

            int minimumT = Math.Min(topLeft_1.T, bottomLeft_3.T) - 1;
            int minimumS = Math.Min(topLeft_1.S, topRight_2.S) - 1;
            int maximumT = Math.Max(topRight_2.T, bottomRight_4.T) + 1;
            int maximumS = Math.Max(bottomLeft_3.S, bottomRight_4.S) + 1;

            int maxT1T3 = Math.Max(topLeft_1.T, bottomLeft_3.T);
            int minT2T4 = Math.Min(topRight_2.T, bottomRight_4.T);
            int maxS1S2 = Math.Max(topLeft_1.S, topRight_2.S);
            int minS3S4 = Math.Min(bottomLeft_3.S, bottomRight_4.S);

            for (int s = minimumS; s <= maximumS; s++)
            {
                for (int t = minimumT; t <= maximumT; t++)
                {
                    if ((s + t) % 2 == 0)
                    {
                        IntVector tile = MiaToTile(t, s);
                        if (tile == topLeftTile ||
                            tile == topRightTile ||
                            tile == bottomLeftTile ||
                            tile == bottomRightTile)
                        {
                            AddAsLast(values, tile, map);
                        }
                        else if (maxT1T3 < t && t < minT2T4 && maxS1S2 < s && s < minS3S4)
                        {
                            AddAsLast(values, tile, map);
                        }
                        else if (IsTileCornerInStandardRectangle(tile.X, tile.Y, standardCoordinates)
                            || IsTileCornerInStandardRectangle(tile.X + 1, tile.Y, standardCoordinates)
                            || IsTileCornerInStandardRectangle(tile.X, tile.Y + 1, standardCoordinates)
                            || IsTileCornerInStandardRectangle(tile.X +1 , tile.Y + 1, standardCoordinates))
                        {
                            AddAsLast(values, tile, map);
                        }
                    }
                }
            }
            return values;
        }

        private static void AddAsLast(LinkedList<IntVector> values, IntVector tile, Map map)
        {
            if (tile.X < 0 || tile.Y < 0 || tile.X >= map.Width || tile.Y >= map.Height)
            {
                return;
            }
            else
            {
                values.AddLast(tile);
            }
        }

        private static bool IsTileCornerInStandardRectangle(int tileX, int tileY, Rectangle standardCoordinates)
        {
            var standardOfTileCorner = Isomath.TileToStandard(tileX, tileY);
            return (standardCoordinates.Contains(standardOfTileCorner.X, standardOfTileCorner.Y)) ;
        }

        private static MiaCoordinates TileToMia(IntVector tileCoordinates)
        {
            return TileToMia(tileCoordinates.X, tileCoordinates.Y);
        }
        private static MiaCoordinates TileToMia(int x, int y)
        {
            return new MiaCoordinates(x - y, x + y);
        }
        private static IntVector MiaToTile(int t, int s)
        {
            return new IntVector((s + t) / 2, (s - t) / 2);
        }

        private struct MiaCoordinates
        {
            public int T;
            public int S;
            public MiaCoordinates(int t, int s)
            {
                T = t;
                S = s;
            }
        }
    }
}
