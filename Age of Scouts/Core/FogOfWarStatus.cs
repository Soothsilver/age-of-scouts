﻿using System;
using System.Linq;
using Age.World;
using Auxiliary;
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
        public bool FromAir;
        public bool OnceOnly;
        public float ClearTime;
        public bool UsedUp;
        public FogRevealer(Vector2 coordinates, float tileRange, float cleartime = 2, bool fromAir = false, bool onceOnly = false)
        {
            OnceOnly = onceOnly;
            ClearTime = cleartime;
            FromAir = fromAir;
            StandardCoordinates = coordinates;
            TileRange = tileRange;
        }
        public void Update(Map map)
        {
            if (!UsedUp)
            {
                FogOfWarMechanics.RevealFogOfWar(StandardCoordinates, (int) (Tile.HEIGHT * TileRange), map, ClearTime,
                    FromAir);

                if (OnceOnly)
                {
                    UsedUp = true;
                }
            }
        }
    }
    static class FogOfWarMechanics
    {
        /// <summary>
        /// Reveals fog of war from the specified point as a circle. WARNING! This method is not yet optimized!
        /// </summary>
        /// <param name="source">The center of the circle of clear fog of war.</param>
        /// <param name="pixelRange">The radius, in standard coordinates.</param>
        /// <param name="map">The map.</param>
        /// <param name="cleartime">The number of seconds for which fog should remain revealed.</param>
        /// <param name="fromAir">If set to <c>true</c>, then this is a bird's eye view reveal that can see past trees.</param>
        internal static void RevealFogOfWar(Vector2 source, int pixelRange, Map map, float cleartime = 2, bool fromAir = false)
        {
            Tile tile = map.GetTileFromStandardCoordinates(source);
            SetClearFogStatus(tile, cleartime, revealSecondsLeftMap);
            for (float angle = 0; angle <= 2 * Math.PI; angle += MathHelper.Pi / 40)
            {
                float dx = (float)Math.Cos(angle) * 10;
                float dy = (float)Math.Sin(angle) * 10;
                float x = source.X;
                float y = source.Y;
                for (int i = 0; i < 200; i++)
                {
                    x += dx; y += dy;
                    if ((new Vector2(x, y) - source).LengthSquared() >= pixelRange * pixelRange) break;
                    if (Settings.Instance.ShowDebugPoints)
                    {
                        Debug.DebugPoints.Coordinates.Add(new Vector2(x, y));
                    }
                    Tile tl = map.GetTileFromStandardCoordinates(new Vector2(x,y));
                    if (tl == null)
                    {
                        break;
                    }
                    SetClearFogStatus(tl, cleartime, revealSecondsLeftMap);
                    if (tl.NaturalObjectOccupant?.EntityKind == EntityKind.UntraversableTree && !fromAir)
                    {
                        break;
                    }
                }
            }
           
        }

        private static object lockRevealChangesSwap = new object();
        private static FogOfWarStatus[,] trueBehindFogOfWarStatus;
        private static FogOfWarStatus[,] revealChangesMap;
        private static float[,] revealSecondsLeftMap;
        private static int mapWidth;
        private static int mapHeight;



        private static void SetClearFogStatus(Tile tile, float cleartime, float[,] revealMap)
        {
            if (trueBehindFogOfWarStatus[tile.X, tile.Y] == FogOfWarStatus.Clear && revealSecondsLeftMap[tile.X, tile.Y] > cleartime)
            {
                return;
            }
            trueBehindFogOfWarStatus[tile.X, tile.Y] = FogOfWarStatus.Clear;
            revealSecondsLeftMap[tile.X, tile.Y] = cleartime;
        }

        public static void PerformFogOfWarReveal(Session sessionUsedInOtherThreads, float elapsedSeconds, bool firstTimeInMap)
        {
            // Reveal.
            if (firstTimeInMap)
            {
                trueBehindFogOfWarStatus = new FogOfWarStatus[sessionUsedInOtherThreads.Map.Width, sessionUsedInOtherThreads.Map.Height];
                revealSecondsLeftMap = new float[sessionUsedInOtherThreads.Map.Width, sessionUsedInOtherThreads.Map.Height];
                for (int x = 0; x < sessionUsedInOtherThreads.Map.Width; x++)
                {
                    for (int y = 0; y < sessionUsedInOtherThreads.Map.Height; y++)
                    {
                        trueBehindFogOfWarStatus[x, y] = FogOfWarStatus.Black;
                    }
                }                
            }
            PerformanceCounter.StartMeasurement(PerformanceGroup.FogOfWarReveal);
            if (Settings.Instance.EnableFogOfWar)
            {
                for (int y = 0; y < sessionUsedInOtherThreads.Map.Height; y++)
                {
                    for (int x = 0; x < sessionUsedInOtherThreads.Map.Width; x++)
                    {
                        revealSecondsLeftMap[x, y] -= elapsedSeconds;
                        if (trueBehindFogOfWarStatus[x, y] == FogOfWarStatus.Clear && revealSecondsLeftMap[x, y] <= 0)
                        {
                            trueBehindFogOfWarStatus[x, y] = FogOfWarStatus.Grey;
                        }
                    }
                };

                foreach (var unit in sessionUsedInOtherThreads.AllUnits.Where(unt => unt.Controller == sessionUsedInOtherThreads.PlayerTroop || Settings.Instance.EnemyUnitsRevealFogOfWar || sessionUsedInOtherThreads.PlayerTroop.Omniscience))
                {
                    FogOfWarMechanics.RevealFogOfWar(unit.FeetStdPosition, Tile.HEIGHT * 5, sessionUsedInOtherThreads.Map);
                }
                foreach (var building in sessionUsedInOtherThreads.AllBuildings.Where(unt => unt.Controller == sessionUsedInOtherThreads.PlayerTroop || Settings.Instance.EnemyUnitsRevealFogOfWar || sessionUsedInOtherThreads.PlayerTroop.Omniscience))
                {
                    if (!building.SelfConstructionInProgress)
                    {
                        FogOfWarMechanics.RevealFogOfWar(building.FeetStdPosition, Tile.HEIGHT * building.Template.LineOfSightInTiles, sessionUsedInOtherThreads.Map, fromAir: true);
                    }
                }
            }

            foreach (FogRevealer revealer in sessionUsedInOtherThreads.Revealers)
            {
                revealer.Update(sessionUsedInOtherThreads.Map);
            }
            PerformanceCounter.EndMeasurement(PerformanceGroup.FogOfWarReveal);

            // Swap.
            lock (lockRevealChangesSwap)
            {
                if (revealChangesMap == null || sessionUsedInOtherThreads.Map.Width != mapWidth || sessionUsedInOtherThreads.Map.Height != mapHeight)
                {
                    mapWidth = sessionUsedInOtherThreads.Map.Width;
                    mapHeight = sessionUsedInOtherThreads.Map.Height;
                    revealChangesMap = new FogOfWarStatus[mapWidth, mapHeight];
                }

                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        revealChangesMap[x, y] = trueBehindFogOfWarStatus[x, y];
                    }
                }
            }
        }

        public static void AcceptRevealChanges(Session session)
        {
            lock (lockRevealChangesSwap)
            {
                if (revealChangesMap == null || session.Map.Width != mapWidth || session.Map.Height != mapHeight)
                {
                    return;
                }
                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        session.Map.Tiles[x, y].Fog = revealChangesMap[x, y];
                    }
                }
            }
        }
    }
}