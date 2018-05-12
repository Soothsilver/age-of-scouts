using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.World
{
    static class MiaAlgorithm
    {
        public static IEnumerable<IntVector> GetTilesInsideRectangle(Rectangle standardCoordinates)
        {
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
                            yield return tile;
                        }
                        else if (IsTileCornerInStandardRectangle(tile.X, tile.Y, standardCoordinates)
                            || IsTileCornerInStandardRectangle(tile.X + 1, tile.Y, standardCoordinates)
                            || IsTileCornerInStandardRectangle(tile.X, tile.Y + 1, standardCoordinates)
                            || IsTileCornerInStandardRectangle(tile.X +1 , tile.Y + 1, standardCoordinates))
                        {
                            yield return tile;
                        }
                    }
                }
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
