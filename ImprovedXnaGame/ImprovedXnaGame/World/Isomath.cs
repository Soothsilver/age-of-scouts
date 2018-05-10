using Age.Core;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.World
{
    /// <summary>
    /// There are three coordinate systems:
    /// <para></para>
    /// TILE marks isometric tiles: it starts at (0,0) in the top corner of the topmost tile, then the X axis increases to bottom right and the Y axis increases to bottom left, by 1 per tile.
    /// A tile's coordinates are the integer tile coordinates of its top corner.
    /// <para></para>
    /// 
    /// STANDARD is the canonical position of an object in the world: it starts at (0,0) in the top corner of the topmost tile, then the X axis increases to the right, by <see cref="Tile.WIDTH"/> per tile,
    /// and the Y axis increases to the bottom, by <see cref="Tile.HEIGHT"/> per tile. The (0,0) coordinates of TILE and STANDARD are the same.
    /// <para></para>
    /// SCREEN is displayed on the user's monitor: it starts at (0,0) in the top left corner of the user's screen, then the X increases by 1 per pixel to the right, and the Y axis by 1 per pixel to the bottom.
    /// Screen coordinates are affected by the monitor offset (where the viewport is focused) and by zoom, unlike the other two coordinate systems.
    /// 
    /// </summary>
    class Isomath
    {
        // Tile => Standard
        public static Vector2 TileToStandard(Vector2 tile)
        {
            return TileToStandard(tile.X, tile.Y);
        }
        public static Vector2 TileToStandard(float x, float y)
        {
            return new Vector2((x - y) * Tile.HALF_WIDTH, (x + y) * Tile.HALF_HEIGHT);
        }
        public static IntVector TileToStandard(int x, int y)
        {
            return new IntVector((x - y) * Tile.HALF_WIDTH, (x + y) * Tile.HALF_HEIGHT);
        }
        // Tile => Screen
        public static Rectangle TileToScreen(Tile tile, IScreenInformation session)
        {
            IntVector top = TileToStandard(tile.X, tile.Y);
            IntVector topLeft = new IntVector(top.X - Tile.HALF_WIDTH, top.Y);
            Rectangle standard = new Rectangle(topLeft.X, topLeft.Y, Tile.WIDTH, Tile.HEIGHT);
            return StandardToScreen(standard, session);
        }

        // Screen => Standard
        public static Vector2 ScreenToStandard(int x, int y, IScreenInformation session)
        {
            return ScreenToStandard(new Vector2(x, y), session);
        }
        public static Vector2 ScreenToStandard(Vector2 screen, IScreenInformation session) { 
            Vector2 topLeftScreen = session.CenterOfScreenInStandardPixels * session.ZoomLevel - new Vector2(Root.ScreenWidth / 2, Root.ScreenHeight / 2);

            float standardX = (screen.X + topLeftScreen.X) / session.ZoomLevel;
            float standardY = (screen.Y + topLeftScreen.Y) / session.ZoomLevel;
            return new Vector2(standardX, standardY);
        }

        // Standard => Screen
        public static Rectangle StandardToScreen(Rectangle standard, IScreenInformation session)
        {
           return StandardToScreen(standard.X, standard.Y, standard.X + standard.Width, standard.Y + standard.Height, session);
        }
        public static Rectangle StandardPersonToScreen(Vector2 feetStandard, int width, int height, IScreenInformation session)
        {
            float zoom = session.ZoomLevel;
            Vector2 feetAsScreen = StandardToScreen(feetStandard, session);
            return new Rectangle(
                (int)(feetAsScreen.X - width/2 * zoom),
                (int)(feetAsScreen.Y - height * zoom),
                (int)(width * zoom),
                (int)(height * zoom));
        }
        public static Vector2 StandardToScreen(Vector2 standard, IScreenInformation session)
        {
            Vector2 topLeftOfScreenInStdPixels = session.CenterOfScreenInStandardPixels * session.ZoomLevel - new Vector2(Root.ScreenWidth / 2, Root.ScreenHeight / 2);

            float feetScreenX = standard.X * session.ZoomLevel - (int)topLeftOfScreenInStdPixels.X;
            float feetScreenY = standard.Y * session.ZoomLevel - (int)topLeftOfScreenInStdPixels.Y;
            return new Vector2(feetScreenX, feetScreenY);
        }
        public static Rectangle StandardToScreen(float x1, float y1, float x2, float y2, IScreenInformation session)
        {
            Vector2 topLeft = StandardToScreen(new Vector2(x1, y1), session);
            Vector2 bottomRight = StandardToScreen(new Vector2(x2, y2), session);
            return new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(bottomRight.X - topLeft.X), (int)(bottomRight.Y - topLeft.Y));
        }

        // Standard => Tile
        internal static IntVector StandardToTile(Vector2 standardFloat)
        {
            IntVector standard = new IntVector((int)standardFloat.X, (int)standardFloat.Y);
            int mapx = (int)Math.Floor( ((float)standard.X / Tile.HALF_WIDTH + (float)standard.Y / Tile.HALF_HEIGHT) / 2   );
            int mapy = (int)Math.Floor( ((float)standard.Y / Tile.HALF_HEIGHT - ((float)standard.X / Tile.HALF_WIDTH)) / 2 );
            return new IntVector(mapx, mapy);
        }
    }
}
