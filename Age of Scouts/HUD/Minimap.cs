using System;
using System.Linq;
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
        private Texture2D minimapTexture = null;
        private Texture2D backBufferMinimapTexture = null;
        private Color[] minimapData = null;
        private Color[] copiableMinimapData = null;
        private Color[] backBufferminimapData = null;
        private static int BASE_TERRAIN_ALPHA = 120;
        private static int TERRAIN_ALPHA = 150;
        private static Color grass = Color.LimeGreen.Alpha(BASE_TERRAIN_ALPHA).OverlayOnto(Color.White);
        private static Color water = Color.CornflowerBlue.Alpha(TERRAIN_ALPHA).OverlayOnto(Color.White);
        private static Color tallGrass = Color.GreenYellow.Alpha(TERRAIN_ALPHA).OverlayOnto(Color.White);
        private static Color forest = Color.DarkGreen.Alpha(TERRAIN_ALPHA).OverlayOnto(Color.White);
        private static Color obstacle = Color.Black;
        private Rectangle rectangle;
        private volatile bool initialized = false;

        public void UpdateTexture(Map map)
        {
            if (initialized)
            {
                int tileWidth = rectangle.Width / map.Width;
                int minimapHeight = rectangle.Height;
                int tileHeight = rectangle.Height / map.Height;
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
                                backBufferminimapData[(screenY + j) * minimapHeight + (screenX + i)] = clr;
                            }
                        }
                    }
                }
                
                
                
                lock (this)
                {
                    int ln = copiableMinimapData.Length;
                    for (int i =0; i < ln; i++)
                    {
                        copiableMinimapData[i] = backBufferminimapData[i];
                    }
                }             
                
            }
        }

        public void Draw(Session session, Rectangle rectangle)
        {
            Map map = session.Map;
            int tileWidth = rectangle.Width / map.Width;

            
            if (minimapTexture == null)
            {
                Initialize(session.Map, rectangle);
            }

            // Blit
            lock (this)
            {

                int ln = copiableMinimapData.Length;
                for (int i = 0; i < ln; i++)
                {
                    minimapData[i] = copiableMinimapData[i];
                }
                minimapTexture.SetData(minimapData);
            }
            Primitives.DrawImage(minimapTexture, rectangle);
            if (session.MinimapWarnings.Warnings.Any())
            {
                int minimapHeight = rectangle.Height;
                int tileHeight = rectangle.Height / map.Height;
                for(int wi = session.MinimapWarnings.Warnings.Count - 1; wi >= 0; wi--)
                {
                    MinimapWarnings.MinimapWarning warning = session.MinimapWarnings.Warnings[wi];
                    if (warning.StopWarningAt > DateTime.Now)
                    {
                        int x = warning.TileX;
                        int y = warning.TileY;
                        int screenX = (x - y) * tileWidth / 2 + rectangle.Width / 2 + rectangle.X;
                        int screenY = (x + y) * tileHeight / 2 + rectangle.Y;
                        int radius = (int)(R.Flicker * 16) + 10;
                        Primitives.DrawRectangle(new Rectangle(screenX - radius, screenY - radius, radius * 2, radius * 2), Color.Red, 2);
                    }
                    else if (warning.StopPreventingWarningsAt < DateTime.Now)
                    {
                        session.MinimapWarnings.Warnings.RemoveAt(wi);
                    }
                   
                }
            }
            // Viewport
            IntVector minimapTopLeft = StandardToMinimap(Isomath.ScreenToStandard(Vector2.Zero, session), rectangle, session);
            IntVector minimapTopRight = StandardToMinimap(Isomath.ScreenToStandard(new Vector2(Root.ScreenWidth, 0), session), rectangle, session);
            IntVector minimapBottomLeft = StandardToMinimap(Isomath.ScreenToStandard(new Vector2(0, Root.ScreenHeight), session), rectangle, session);
            IntVector minimapBottomRight = StandardToMinimap(Isomath.ScreenToStandard(new Vector2(Root.ScreenWidth, Root.ScreenHeight), session), rectangle, session);
            Rectangle minimapRect = new Rectangle((int)minimapTopLeft.X, (int)minimapTopLeft.Y, (int)(minimapBottomRight.X - minimapTopLeft.X), (int)(minimapBottomRight.Y - minimapTopLeft.Y));
            Primitives.DrawRectangle(minimapRect, Color.White);
            

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
            this.backBufferMinimapTexture = new Texture2D(Root.GraphicsDevice, rectangle.Width, rectangle.Height);
            this.minimapData = new Color[rectangle.Width * rectangle.Height];
            this.backBufferminimapData = new Color[rectangle.Width * rectangle.Height];
            this.copiableMinimapData = new Color[rectangle.Width * rectangle.Height];
            this.rectangle = rectangle;
            for (int y = 0; y < rectangle.Height; y++)
            {
                for (int x = 0; x < rectangle.Width; x++)
                {
                    minimapData[y * rectangle.Height + x] = Color.Transparent;
                    backBufferminimapData[y * rectangle.Height + x] = Color.Transparent;
                    copiableMinimapData[y * rectangle.Height + x] = Color.Transparent;
                }
            }
            minimapTexture.SetData(minimapData);
            this.initialized = true;

        }

        private IntVector StandardToMinimap(Vector2 standard, Rectangle rectangle, Session session)
        {
            int minimapHeight = rectangle.Height;
            int tileWidth = rectangle.Width / session.Map.Width;
            int tileHeight = rectangle.Height / session.Map.Height;
            int x = (int)(standard.X * tileWidth / Tile.WIDTH);
            int y = (int)(standard.Y * tileHeight / Tile.HEIGHT);
            int screenX = x + rectangle.X + rectangle.Width /2;
            int screenY = y + rectangle.Y;
            return new IntVector(screenX, screenY);
        }

        private static Color GetMinimapColor(Tile tile)
        {
            if (tile.Fog == FogOfWarStatus.Black && Settings.Instance.EnableFogOfWar)
            {
                return Color.Black;
            }
          
            if (tile.Occupants.Count > 0 && (!Settings.Instance.EnableFogOfWar || tile.Fog == FogOfWarStatus.Clear))
            {
                return tile.Occupants[0].Controller.StrongColor;
            }
            if (tile.BuildingOccupant != null)
            {
                return tile.BuildingOccupant.Controller.StrongColor;
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
                    case EntityKind.MudMine:
                        return Color.Brown;
                    case EntityKind.CutDownTree:
                        return grass;
                }
            }
            switch (tile.Type)
            {
                case TileType.Grass:
                    return grass;
                case TileType.Water: return water;
                case TileType.Mud:
                    return tallGrass;
                case TileType.Road:
                    return Color.Yellow;
                default: return Color.Red;
            }
        }

        public void Update(Selection selection, Session session)
        {
            if (initialized)
            {
                Vector2 mouseOverStandardCoordinates = Vector2.Zero;
                int minimapMouseX = Root.Mouse_NewState.X - (rectangle.X + rectangle.Width / 2);
                int minimapMouseY = Root.Mouse_NewState.Y - rectangle.Y;
                int tileWidth = rectangle.Width / session.Map.Width;
                int tileHeight = rectangle.Height / session.Map.Height;

                int standardX = minimapMouseX * Tile.WIDTH / tileWidth;
                int standardY = minimapMouseY * Tile.HEIGHT / tileHeight;
                Vector2 pointerStandard = new Vector2(standardX, standardY);
                Tile tile = session.Map.GetTileFromStandardCoordinates(pointerStandard);
                if (tile != null)
                {
                    if (!selection.SelectionInProgress)
                    {
                        selection.DontStartSelectionBoxThisFrame = true;
                        Root.WasMouseLeftClick = false;
                        if (Root.Mouse_NewState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                        {
                            session.CenterOfScreenInStandardPixels = pointerStandard;
                            selection.StandardCoordinatesSelectionStart = Vector2.Zero;
                        }
                        if (Root.WasMouseRightClick)
                        {
                            session.RightClickOn(selection, pointerStandard);
                            Root.WasMouseRightClick = false;
                        }
                    }
                }
            }
        }
    }
}