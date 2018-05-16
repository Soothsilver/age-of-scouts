using System;
using Age.Core;
using Age.Phases;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.HUD
{
    internal class Minimap
    {
        private IntVector mouseOverStandard = new IntVector(0, 0);
        private Texture2D minimapTexture = null;
        private Color[] minimapData = null;
        private static int BASE_TERRAIN_ALPHA = 120;
        private static int TERRAIN_ALPHA = 150;
        private static Color grass = Color.LimeGreen.Alpha(BASE_TERRAIN_ALPHA).OverlayOnto(Color.White);
        private static Color water = Color.CornflowerBlue.Alpha(TERRAIN_ALPHA).OverlayOnto(Color.White);
        private static Color tallGrass = Color.GreenYellow.Alpha(TERRAIN_ALPHA).OverlayOnto(Color.White);
        private static Color forest = Color.DarkGreen.Alpha(TERRAIN_ALPHA).OverlayOnto(Color.White);
        private static Color obstacle = Color.Black;
        private int cyclesUntilRedraw = 0;

        public void Draw(Session session, Rectangle rectangle)
        {
            Map map = session.Map;
            int tileWidth = rectangle.Width / map.Width;
            if (minimapTexture == null)
            {
                Initialize(session.Map, rectangle);
            }

            if (cyclesUntilRedraw <= 0)
            {
                int minimapHeight = rectangle.Height;
                int minimapWidth = rectangle.Width;
                int tileHeight = rectangle.Height / map.Height;
                mouseOverStandard = IntVector.Zero;
                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        int screenX = (x - y) * tileWidth / 2 + rectangle.Width / 2;
                        for (int j = 0; j < tileHeight; j++)
                        {
                            int screenY = (x + y) * tileHeight / 2;
                            Tile t = map.Tiles[x, y];
                            Color clr = GetMinimapColor(t);
                            if (t.Fog == FogOfWarStatus.Grey && Settings.Instance.EnableFogOfWar)
                            {
                                clr = Color.Black.Alpha(150).OverlayOnto(clr);
                            }
                            for (int i = 0; i < tileWidth; i++)
                            {
                                minimapData[(screenY + j) * minimapHeight + (screenX + i)] = clr;
                            }
                        }
                    }
                }

                cyclesUntilRedraw = 10;
                minimapTexture.SetData<Color>(minimapData);
            }
            else
            {
                cyclesUntilRedraw--;
            }

            // Blit
            Primitives.DrawImage(minimapTexture, rectangle);
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
            int borderWidth = 4;
            Primitives.DrawLine(top, right, borderColor, borderWidth);
            Primitives.DrawLine(bottom, right, borderColor, borderWidth);
            Primitives.DrawLine(top, left, borderColor, borderWidth);
            Primitives.DrawLine(bottom, left, borderColor, borderWidth);
        }

        private void Initialize(Map sessionMap, Rectangle rectangle)
        {
            this.minimapTexture = new Texture2D(Root.GraphicsDevice, rectangle.Width, rectangle.Height);
            this.minimapData = new Color[rectangle.Width * rectangle.Height];
            for (int y = 0; y < rectangle.Height; y++)
            {
                for (int x = 0; x < rectangle.Width; x++)
                {
                    minimapData[y * rectangle.Height + x] = Color.Transparent;
                }
            }
        }

        private static Color GetMinimapColor(Tile tile)
        {
            if (tile.Fog == FogOfWarStatus.Black && Settings.Instance.EnableFogOfWar)
            {
                return Color.Black;
            }
          
            if (tile.Occupants.Count > 0)
            {
                return tile.Occupants[0].Controller.StrongColor;
            }
            if (tile.NaturalObjectOccupant != null)
            {
                switch (tile.NaturalObjectOccupant.EntityKind)
                {
                    case EntityKind.TallGrass:
                        return tallGrass;
                    case EntityKind.TutorialFlag:
                        return obstacle;
                    case EntityKind.UnalignedTent:
                        return obstacle;
                    case EntityKind.UntraversableTree:
                        return forest;
                    case EntityKind.BerryBush:
                        return Color.Violet;
                    case EntityKind.Corn:
                        return Color.Yellow;
                }
            }
            switch (tile.Type)
            {
                case TileType.Grass:
                    return grass;
                case TileType.Water: return water;
                default: return Color.Red;
            }
        }

        public void Update(Selection selection, Session session)
        {
            /*
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
            */
        }
    }
}