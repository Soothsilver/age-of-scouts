using Age.Core;
using Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.World
{
    class Waterflow
    {
        private static float secondsUntilNextChange = 0.2f;
        private static TextureName[] waterTextures = new[] { TextureName.IsoWater1, TextureName.IsoWater2, TextureName.IsoWater3 }; 
        internal static void Flow(float elapsedSeconds, Map map)
        {
            secondsUntilNextChange -= elapsedSeconds;
            if (secondsUntilNextChange <= 0)
            {
                secondsUntilNextChange += 0.2f;
                for (int y = map.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        Tile tile = map.Tiles[x, y];
                        if (tile.Type == TileType.Water)
                        {
                            if (tile.Neighbours.TopRight != null && tile.Neighbours.TopRight.Type == TileType.Water
                                 && tile.Neighbours.TopRight.Icon != TextureName.IsoWater)
                            {
                                tile.Icon = tile.Neighbours.TopRight.Icon;
                            }
                            else
                            {
                                tile.Icon = waterTextures[R.Next(waterTextures.Length)];
                            }
                        }
                    }
                }
            }
        }
    }
}
