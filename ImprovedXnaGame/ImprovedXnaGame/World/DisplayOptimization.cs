using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Core;
using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.World
{
    class DisplayOptimization
    {
        internal static HashSet<Tile> GetTilesVisibleOnScreen(Session session)
        {
            PerformanceCounter.StartMeasurement(PerformanceGroup.GetTilesVisibleOnScreen);
            HashSet<Tile> visibleTiles = new HashSet<Tile>();
            Vector2 standardTopLeft = Isomath.ScreenToStandard(Vector2.Zero, session);
            Vector2 standardTopRight = Isomath.ScreenToStandard(new Vector2(Root.ScreenWidth, 0), session);
            Vector2 standardBottomLeft = Isomath.ScreenToStandard(new Vector2(0, Root.ScreenHeight), session);
            Vector2 standardBottomRight = Isomath.ScreenToStandard(new Vector2(Root.ScreenWidth, Root.ScreenHeight), session);
            Rectangle standardRect = new Rectangle((int)standardTopLeft.X, (int)standardTopLeft.Y, (int)(standardBottomRight.X - standardTopLeft.X), (int)(standardBottomRight.Y - standardTopLeft.Y));

            foreach(IntVector tilePosition in MiaAlgorithm.GetTilesInsideRectangle(standardRect))
            {
                if (tilePosition.X >= 0 && tilePosition.Y >= 0 && tilePosition.X < session.Map.Width && tilePosition.Y < session.Map.Height)
                {
                    visibleTiles.Add(session.Map.Tiles[tilePosition.X, tilePosition.Y]);
                }
            }
            PerformanceCounter.EndMeasurement(PerformanceGroup.GetTilesVisibleOnScreen);
            return visibleTiles;
        }
    }
}
