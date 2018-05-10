using Age.Core;
using Age.HUD;
using Age.Phases;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledSharp;

namespace Age.World
{
    class Map
    {
        public Tile[,] Tiles;
        internal int Width;
        internal int Height;

        internal void Draw(Session session, float elapsedSeconds, Selection selection)
        {
            Vector2 centerOfScreenInStandardPixels = session.CenterOfScreenInStandardPixels;
            float zoomLevel = session.ZoomLevel;
            // Layer 1: Tiles
            this.ForEachTile((x, y, tile) =>
            {
                Rectangle rectTile = Isomath.TileToScreen(tile, session);
                Primitives.DrawImage(Library.Get(tile.Icon), rectTile);
           
               
            });
            this.ForEachTile((x, y, tile) =>
            {
                Rectangle rectTile = Isomath.TileToScreen(tile, session);

                if (Settings.Instance.EnableFogOfWar)
                {                    
                    if (tile.Fog == FogOfWarStatus.Black)
                    {
                        Primitives.DrawImage(Library.Get(TextureName.FogOfWar), rectTile);
                    }
                }
            });
            this.ForEachTile((x, y, tile) =>
            {
                Rectangle rectTile = Isomath.TileToScreen(tile, session);

                if (Settings.Instance.EnableFogOfWar)
                {
                    if (tile.Fog == FogOfWarStatus.Grey)
                    {
                        Primitives.DrawImage(Library.Get(TextureName.WhiteTile), rectTile, Color.Black.Alpha(150));
                    }
                }
            });


            // Layer 1b: Projectiles' shadow
            foreach (Projectile p in session.Projectiles)
            {
                p.DrawShadow(session);
            }

            // Layer 1c: Pathing
            foreach (var unit in session.AllUnits)
            {
                Rectangle rectTile = Isomath.TileToScreen(unit.Occupies, session);
                if (Settings.Instance.ShowDebugPoints && selection.SelectedUnits.Contains(unit))
                {
                    Primitives.DrawImage(Library.Get(TextureName.WhiteTile), rectTile, Color.Red.Alpha(150));
                    Primitives.FillCircle(Isomath.StandardToScreen(unit.FeetStdPosition, session), 5, Color.DarkRed);
                }
                if (unit.Activity.AttackingInProgress)
                {
                    Primitives.FillCircleQuick(Isomath.StandardToScreen(unit.FeetStdPosition, session), (int)(R.Flicker * 15) + 5, Color.Red.Alpha(150));
                }
                if (unit.Activity.HasAGoal && unit.Activity.PathingCoordinates != null && selection.SelectedUnits.Contains(unit))
                {
                    Vector2 previousWaypoint = unit.FeetStdPosition;
                    foreach (var waypoint in unit.Activity.PathingCoordinates)
                    {
                        Vector2 screenFrom = Isomath.StandardToScreen(previousWaypoint, session);
                        Vector2 screenTo = Isomath.StandardToScreen(waypoint, session);
                        Primitives.DrawLine(screenFrom, screenTo, unit.Controller.StrongColor, 2);
                        previousWaypoint = waypoint;
                    }
                }
            }

            // Layer 2: Units and structures
            this.ForEachTile((x, y, tile) =>
            {
                Rectangle rectTile = Isomath.TileToScreen(tile, session);
                Tooltip tooltip = null;
                if (!Settings.Instance.EnableFogOfWar || tile.Fog == FogOfWarStatus.Clear)
                {
                    foreach (Corpse corpse in tile.BrokenOccupants)
                    {
                        corpse.Draw(elapsedSeconds, session);
                    }
                    tile.BrokenOccupants.RemoveAll(crps => crps.Lost);
                    foreach (Unit unit in tile.Occupants)
                    {
                        Texture2D icon = unit.AnimationTick(elapsedSeconds);
                        Rectangle rectUnit = Isomath.StandardPersonToScreen(unit.FeetStdPosition, unit.Sprite.Sprite.Width, unit.Sprite.Sprite.Height, session);
                        Primitives.DrawImage(icon, rectUnit);
                        Rectangle rewHitbox = Isomath.StandardToScreen(unit.Hitbox, session);
                        if (selection.SelectedUnits.Contains(unit))
                        {
                            Primitives.DrawRectangle(rewHitbox.Extend(1, 1), unit.Controller.StrongColor);
                        }
                        if (unit.HP < unit.MaxHP)
                        {
                            Primitives.DrawHealthbar(new Rectangle(rewHitbox.X, rewHitbox.Y - 5, rewHitbox.Width, 5), unit.Controller.StrongColor, unit.HP, unit.MaxHP);
                        }
                        if (Root.IsMouseOver(rewHitbox))
                        {
                            tooltip = unit.GetTooltip();
                        }
                    }
                }
                if (Settings.Instance.EnableFogOfWar && tile.Fog == FogOfWarStatus.Black)
                {
                    return;
                }

                if (Root.IsMouseOver(rectTile) && tooltip == null)
                {
                    UI.MajorTooltip = tile.GetTooltip();
                }
                if (tile.NaturalObjectOccupant != null)
                {
                    Rectangle rectObject = Isomath.StandardPersonToScreen(tile.NaturalObjectOccupant.FeetStdPosition, tile.NaturalObjectOccupant.PixelWidth, tile.NaturalObjectOccupant.PixelHeight, session);
                    Primitives.DrawImage(Library.Get(tile.NaturalObjectOccupant.Icon), new Rectangle(rectObject.X, rectObject.Y, rectObject.Width, rectObject.Height));
                }
            });

            // Layer 3: Projectiles
            foreach(Projectile p in session.Projectiles)
            {
                p.Draw(session);
            }

            // Layer 4: Debug
            if (Settings.Instance.ShowDebugPoints)
            {
                foreach (Vector2 coordinate in Debug.DebugPoints.Coordinates)
                {
                    Primitives.FillCircle(Isomath.StandardToScreen(coordinate, session), 5, Color.Violet);
                }
                if (selection.SelectedUnits.Count > 0)
                {
                    Unit unit = selection.SelectedUnits[0];
                    Primitives.DrawSingleLineText("Feet standard: " + unit.FeetStdPosition + "\nTile: " + unit.Occupies.ToString() + "\nTile standard: " + Isomath.TileToStandard(unit.Occupies.X, unit.Occupies.Y),
                        new Vector2(50, 300), Color.White, Library.FontTinyBold);
                }
            }
            Debug.DebugPoints.Coordinates.Clear();

        }
    
        

        internal Tile GetTileFromStandardCoordinates(Vector2 standard)
        {
            IntVector point = Isomath.StandardToTile(standard);
            if (point.X >= 0 && point.Y >= 0 && point.X < Width && point.Y < Height)
            {
                return this.Tiles[point.X, point.Y];
            } 
            else
            {
                return null;
            }
        }

        internal void ForEachTile(Action<int, int, Tile> x_y_tile_action)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    x_y_tile_action(x, y, Tiles[x, y]);
                }
            }
        }
    }
}
