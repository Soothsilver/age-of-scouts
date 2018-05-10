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
        static float SecondsUntilNextChange = 0.2f;
        static TextureName[] WaterTextures = new[] { TextureName.IsoWater1, TextureName.IsoWater2, TextureName.IsoWater3 }; 
        internal static void Flow(float elapsedSeconds, Map map)
        {
            SecondsUntilNextChange -= elapsedSeconds;
            if (SecondsUntilNextChange <= 0)
            {
                SecondsUntilNextChange += 0.2f;
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
                                tile.Icon = WaterTextures[R.Integer(WaterTextures.Length)];
                            }
                        }
                    }
                }
            }
        }
    }
}
