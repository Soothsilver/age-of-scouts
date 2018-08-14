using System;
using System.Collections.Generic;
using Age.Core;
using Age.World;

namespace Age.Phases
{
    internal class WallPlacement
    {
        internal static List<Tile> DetermineWhereToPlaceWalls(Tile startedBuildingOnThisTile, Tile mouseOverTile, Session session)
        {
            Map map = session.Map;
            int xdif = Math.Abs(startedBuildingOnThisTile.X - mouseOverTile.X);
            int ydif = Math.Abs(startedBuildingOnThisTile.Y - mouseOverTile.Y);
            int max = Math.Max(xdif, ydif);
            int xd = (xdif >= ydif ? 1 : 0);
            int yd = (xdif >= ydif ? 0 : 1);
            if (mouseOverTile.X < startedBuildingOnThisTile.X) xd *= -1;
            if (mouseOverTile.Y < startedBuildingOnThisTile.Y) yd *= -1;
            List<Tile> tiles = new List<Tile>();
            for (int i  =0; i <= max; i++)
            {
                Tile tl = map.GetTileFromTileCoordinates(startedBuildingOnThisTile.X + xd * i, startedBuildingOnThisTile.Y + yd * i);
                if (tl != null && BuildingTemplate.Wall.PlaceableOn(session, tl, !Settings.Instance.EnableFogOfWar))
                {
                    tiles.Add(tl);
                }
            }
            return tiles;
        }
    }
}