using System;
using Age.Core;
using Age.Phases;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.HUD
{
    internal class Minimap
    {
        private static Tile mouseOverTile;
        internal static void Draw(Session session, Rectangle rectangle)
        {
            Map map = session.Map;
            int tileWidth = rectangle.Width / map.Width;
            int tileHeight = rectangle.Height / map.Height;
            mouseOverTile = null;
            for (int y = 0; y < map.Height; y++)
            {
                for (int x =0; x< map.Width; x++)
                {
                    int screenX = (x - y) * tileWidth / 2  + rectangle.X + rectangle.Width / 2;
                    int screenY = (x + y) * tileHeight / 2 + rectangle.Y;
                    Rectangle r = new Rectangle(screenX, screenY, tileWidth, tileHeight);
                    Tile t = map.Tiles[x, y];
                    if (Root.IsMouseOver(r))
                    {
                        mouseOverTile = t;
                    }
                    Primitives.FillRectangle(r, Color.White);
                    Primitives.FillRectangle(r, GetMinimapColor(t));
                    if (Settings.Instance.EnableFogOfWar)
                    {
                        if (t.Fog == FogOfWarStatus.Black)
                        {
                            Primitives.FillRectangle(r, Color.Black);
                        }
                        else if (t.Fog == FogOfWarStatus.Grey)
                        {
                            Primitives.FillRectangle(r, Color.Black.Alpha(150));
                        }
                    }
                }
            }

            // Viewport
            /*
            var centerTile = Isomath.StandardToTile(session.CenterOfScreenInStandardPixels);
            int viewportTilesX = (int)(Root.ScreenWidth / (session.ZoomLevel * Tile.WIDTH));
            int viewportTilesY = (int)(Root.ScreenHeight / (session.ZoomLevel * Tile.HEIGHT));
            IntVector topLeftTile = new IntVector(centerTile.X - viewportTilesX / 2, centerTile.Y - viewportTilesY / 2);
            Rectangle rectViewport = new Rectangle(
                    (topLeftTile.X - topLeftTile.Y) * tileWidth / 2 + rectangle.X + rectangle.Width / 2,
                    (topLeftTile.X + topLeftTile.Y) * tileHeight / 2 + rectangle.Y,
                    tileWidth * viewportTilesX,
                    tileHeight * viewportTilesY                
                );
            Primitives.DrawRectangle(rectViewport, Color.White, 1);
            Primitives.DrawRectangle(rectViewport.Extend(1,1), Color.Black, 1);
            */

            // Borders
            int movright = tileWidth;
            int movall = -2;
            int prolongBottomLeftX = -4;
            int prolongBottomLeftY = 4;
            Vector2 top = new Vector2(rectangle.X + movright + rectangle.Width / 2 + movall, rectangle.Y + movall);
            Vector2 right = new Vector2(rectangle.Right + movright + movall, rectangle.Y + rectangle.Height / 2 + movall);
            Vector2 bottom = new Vector2(rectangle.X + movright + rectangle.Width / 2 + movall + prolongBottomLeftX, rectangle.Bottom + movall + prolongBottomLeftY);
            Vector2 left = new Vector2(rectangle.X + movright + movall + prolongBottomLeftX, rectangle.Y + rectangle.Height / 2 + movall + prolongBottomLeftY);

            Color borderColor = ColorScheme.Foreground;
            Color alternateBorderColor = Color.White;
            int borderWidth = 4;
            Primitives.DrawLine(top, right, borderColor, borderWidth);
            Primitives.DrawLine(bottom, right, borderColor, borderWidth);
            Primitives.DrawLine(top, left, borderColor, borderWidth);
            Primitives.DrawLine(bottom, left, borderColor, borderWidth);
        }

        private static Color GetMinimapColor(Tile tile)
        {
            int BASE_TERRAIN_ALPHA = 120;
            int TERRAIN_ALPHA = 170;
          
            if (tile.Occupants.Count > 0)
            {
                return tile.Occupants[0].Controller.StrongColor;
            }
            if (tile.NaturalObjectOccupant != null)
            {
                switch (tile.NaturalObjectOccupant.EntityKind)
                {
                    case EntityKind.TallGrass:
                        return Color.Green.Alpha(BASE_TERRAIN_ALPHA);
                    case EntityKind.TutorialFlag:
                        return Color.White.Alpha(TERRAIN_ALPHA);
                    case EntityKind.UnalignedTent:
                        return Color.Black.Alpha(TERRAIN_ALPHA);
                    case EntityKind.UntraversableTree:
                        return Color.DarkGreen.Alpha(TERRAIN_ALPHA);
                    case EntityKind.BerryBush:
                        return Color.Violet.Alpha(TERRAIN_ALPHA);
                    case EntityKind.Corn:
                        return Color.Yellow.Alpha(TERRAIN_ALPHA);
                }
            }
            switch (tile.Type)
            {
                case TileType.Grass: return Color.LimeGreen.Alpha(BASE_TERRAIN_ALPHA);
                case TileType.Water: return Color.Blue.Alpha(TERRAIN_ALPHA);
                default: return Color.Red;
            }
        }

        internal static void Update(Selection selection, Session session)
        {
            if (mouseOverTile != null && !selection.SelectionInProgress)
            {
                Root.WasMouseLeftClick = false;
                if (Root.Mouse_NewState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    session.CenterOfScreenInStandardPixels = Isomath.TileToStandard(mouseOverTile.X + 0.5f, mouseOverTile.Y + 0.5f);
                    selection.StandardCoordinatesSelectionStart = Vector2.Zero;
                    selection.SelectionInProgress = false;
                }
                if (Root.WasMouseRightClick)
                {
                    session.RightClickOn(selection, Isomath.TileToStandard(mouseOverTile.X + 0.5f, mouseOverTile.Y + 0.5f));
                    Root.WasMouseRightClick = false;
                }

            } 
        }
    }
}