using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Auxiliary
{
    /// <summary>
    /// This class contains methods for drawing 2D primitives. 
    /// WARNING! Before using it, you must call the Primitives.Init() function. This method is called automatically by Root.Init(), however.
    /// </summary>
    public static partial class Primitives
    {
        public static SpriteBatch SpriteBatch;
        private static GraphicsDevice graphicsDevice;
        

        /// <summary>
        /// Draws a filled rectangle without borders. 
        /// </summary>
        public static void FillRectangle(Rectangle rectangle, Color color)
        {
            SpriteBatch.Draw(Library.Pixel, rectangle, color);
        }
        /// <summary>
        /// Draw the border of a rectangle, without filling it in.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="color">Color of the border.</param>
        /// <param name="thickness">Number of pixels (line width)</param>
        public static void DrawRectangle(Rectangle rectangle, Color color, int thickness = 1)
        {
            SpriteBatch.Draw(Library.Pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            SpriteBatch.Draw(Library.Pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            SpriteBatch.Draw(Library.Pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
            SpriteBatch.Draw(Library.Pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
        }
        /// <summary>
        /// Draw a filled rectangle with a border.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="innerColor">The fill-in color.</param>
        /// <param name="outerColor">The border color.</param>
        /// <param name="thickness">Border width.</param>
        public static void DrawAndFillRectangle(Rectangle rectangle, Color innerColor, Color outerColor, int thickness = 1)
        {
            FillRectangle(rectangle, innerColor);
            DrawRectangle(rectangle, outerColor, thickness);
        }
        /// <summary>
        /// Draws a square centered on the specified position.
        /// </summary>
        /// <param name="position">Center of the square.</param>
        /// <param name="color">Color of the square.</param>
        /// <param name="size">VideoWidth of the square.</param>
        public static void DrawPoint(Vector2 position, Color color, int size = 1)
        {
            FillRectangle(new Rectangle((int)position.X - size /2, (int)position.Y - size/2, size, size), color);
        }
        /// <summary>
        /// Draws a line between two points in 2D space.
        /// </summary>
        /// <param name="startPoint">Line starts at this point.</param>
        /// <param name="endPoint">Line ends at this point.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="width">VideoWidth of the line in pixels.</param>
        public static void DrawLine(Vector2 startPoint, Vector2 endPoint, Color color, int width = 1)
        {
            float angle = (float)Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
            float length = Vector2.Distance(startPoint, endPoint);
            SpriteBatch.Draw(Library.Pixel, startPoint, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }

        public static void DrawHealthbar(Rectangle rectangle, Color strongColor, int HP, int maxHP)
        {
            Primitives.FillRectangle(rectangle, Color.White);
            Primitives.FillRectangle(new Rectangle(rectangle.X, rectangle.Y, rectangle.Width * HP / maxHP, rectangle.Height), strongColor);
            Primitives.DrawRectangle(rectangle, Color.Black);
        }

        /// <summary>
        /// Draws string using the basic XNA method. Does not perform word-wrap (it is much faster, though).
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Position of the top-left corner.</param>
        /// <param name="color">Text color.</param>
        /// <param name="font">Font of the text.</param>
        /// <param name="scale">Text will be scaled down or up. A scale of 1 means normal size.</param>
        public static void DrawSingleLineText(string text, Vector2 position, Color color, SpriteFont font = null, float scale = 1)
        {
            if (font == null)
            {
                font = Library.FontVerdana;
            }   
            SpriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
        /// <summary>
        /// Draws a Texture2D, possibly preserving aspect-ratio (based on parameters).
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="rectangle">The rectangle to fill with the texture.</param>
        /// <param name="color">Tint the image with this color. A value of null (default) means no tinting (i.e. white).</param>
        /// <param name="scale">If true, the drawing will preserve aspect ratio.</param>
        /// <param name="scaleUp">If true, and we preserve aspect ratio, then the image will be scaled up if necessary.</param>
        /// <param name="scaleBgColor">The color that fills the background in case aspect-ratio is preserved. By default, this is null, i.e. no color.</param>
        public static void DrawImage(Texture2D texture, Rectangle rectangle, Color? color = null, bool scale = false, bool scaleUp = true, Color? scaleBgColor = null)
        {
            Color clr = color ?? Color.White;
            
            if (scale)
            {
                Color clrB = scaleBgColor ?? Color.Transparent;
                Primitives.FillRectangle(rectangle, clrB);
                SpriteBatch.Draw(texture, Utilities.ScaleRectangle(rectangle, texture.Width, texture.Height, scaleUp), clr);

            }
            else
            {
                if (rectangle.Width % texture.Width == 0 && rectangle.Height % texture.Height == 0)
                {
                    int xZoom = rectangle.Width / texture.Width;
                    int yZoom = rectangle.Height / texture.Height;
                    if (xZoom == yZoom && xZoom != 1)
                    {
                        SpriteBatch.Draw(texture, new Vector2(rectangle.X, rectangle.Y), null, clr, 0, Vector2.Zero, xZoom, SpriteEffects.None, 0);
                    }
                    else
                    {
                        SpriteBatch.Draw(texture, rectangle, clr);
                    }
                }
                else
                {
                    SpriteBatch.Draw(texture, rectangle, clr);
                }
            }
        }
        

        /* ROUNDED RECTANGLES */
        /// <summary>
        /// Draw the border of a rounded rectangle.
        /// WARNING! Methods for drawing rounded rectangles store rectangle mask textures in memory for performance reasons.
        /// If you draw multiple rounded rectangles of different sizes, you may have both performance and memory problems.
        /// You may, however, draw a rectangle of the same dimensions multiple times on different areas of the screen without problems.
        /// </summary>
        public static void DrawRoundedRectangle(Rectangle rectangle, Color color, int thickness = 1)
        {
            if (rectangle.Width > 1500 || rectangle.Height > 1500)
            {
                DrawRectangle(rectangle, color, thickness);
                return;
            }
            InnerDrawRoundedRectangle(rectangle, color, color, thickness, false, true);
        }
        /// <summary>
        /// Draws a filled rounded rectangle without a border.
        /// WARNING! Methods for drawing rounded rectangles store rectangle mask textures in memory for performance reasons.
        /// If you draw multiple rounded rectangles of different sizes, you may have both performance and memory problems.
        /// You may, however, draw a rectangle of the same dimensions multiple times on different areas of the screen without problems.
        /// </summary>
        public static void FillRoundedRectangle(Rectangle rectangle, Color color)
        {
            if (rectangle.Width > 1500 || rectangle.Height > 1500)
            {
                FillRectangle(rectangle, color);
                return;
            }
            InnerDrawRoundedRectangle(rectangle, color, Color.Transparent, 0, true, false);
        }
        /// <summary>
        /// Draws a filled rounded rectangle with a border.
        /// WARNING! Methods for drawing rounded rectangles store rectangle mask textures in memory for performance reasons.
        /// If you draw multiple rounded rectangles of different sizes, you may have both performance and memory problems.
        /// You may, however, draw a rectangle of the same dimensions multiple times on different areas of the screen without problems.
        /// </summary>
        public static void DrawAndFillRoundedRectangle(Rectangle rectangle, Color innerColor, Color outerColor, int thickness = 1)
        {
            FillRoundedRectangle(rectangle, innerColor);
            DrawRoundedRectangle(rectangle, outerColor, thickness);
        }
        private static void InnerDrawRoundedRectangle(Rectangle rectangle, Color innerColor, Color outerColor, int width, bool doFill, bool doDrawBorder)
        {
            // In case of insufficent size, fall back to full rectangle.
            if (rectangle.Width < 32 || rectangle.Height < 32)
            {
                if (doFill)
                    FillRectangle(rectangle, innerColor);
                if (doDrawBorder)
                    DrawRectangle(rectangle, outerColor, width);
                return;
            }
            // Check the cache.
            Vector2 rectDimensions = new Vector2(rectangle.Width, rectangle.Height);
            bool containsKeyDimension = roundedRectangleCache.ContainsKey(rectDimensions);
            if (containsKeyDimension)
            {
                foreach (var rr in roundedRectangleCache[rectDimensions])
                {
                    if (rr.Thickness == width)
                    {
                        if (doFill)
                            SpriteBatch.Draw(rr.FillInTexture, rectangle, innerColor);
                        else if (doDrawBorder)
                            SpriteBatch.Draw(rr.BorderTexture, rectangle, outerColor);
                        return;
                    }
                }
                
            }
            // Cache search failed. Create new texture.
            Texture2D textureFill = new Texture2D(Primitives.graphicsDevice, rectangle.Width, rectangle.Height);
            Texture2D textureDraw = new Texture2D(Primitives.graphicsDevice, rectangle.Width, rectangle.Height);
       
            Color[] dataFill = new Color[rectangle.Width * rectangle.Height];
            Color[] dataDraw = new Color[rectangle.Width * rectangle.Height];
            for (int i = 0; i < dataFill.Length; i++)
            {
                dataFill[i] = Color.Transparent;
                dataDraw[i] = Color.Transparent;
            }
            for (int x = 16; x < rectangle.Width - 16; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    dataDraw[y * rectangle.Width + x] = Color.White;
                    dataDraw[(rectangle.Height - 1 - y) * rectangle.Width + x] = Color.White;
                }
                for (int y = 0; y < rectangle.Height; y++)
                {
                    dataFill[y * rectangle.Width + x] = Color.White;
                    dataFill[(rectangle.Height - 1 -y) * rectangle.Width + x] = Color.White;
                }
            }
            for (int y = 16; y < rectangle.Height - 16; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    dataDraw[y * rectangle.Width + x] = Color.White;
                    dataDraw[y * rectangle.Width + (rectangle.Width - x - 1)] = Color.White;
                }
                for (int x = 0; x < rectangle.Width; x++)
                {
                    dataFill[y * rectangle.Width + x] = Color.White;
                    dataFill[y * rectangle.Width + (rectangle.Width - x - 1)] = Color.White;
                }
            }


            const double angleStep = 0.01f;
            Point[] centerPoints = new Point[] { new Point(rectangle.Width-17, rectangle.Height-17),
                                                 new Point(16, rectangle.Height-17),
                                                 new Point(16, 16),
                                                 new Point(rectangle.Width-17, 16)};
            double[] startAngles = new double[] { 0, Math.PI / 2, Math.PI, Math.PI * 3 / 2 };
            for (int i = 0; i < 4; i++)
            {
                for (double angle = startAngles[i]; angle < startAngles[i]+Math.PI / 2; angle += angleStep)
                {
                    for (int reach = 16; reach > 16 - width; reach--)
                    {
                        int y = (int)(Math.Round(Math.Sin(angle) * reach));
                        int x = (int)(Math.Round(Math.Cos(angle) * reach));
                        Point target = new Point(centerPoints[i].X + x, centerPoints[i].Y + y);
                        dataDraw[target.Y * rectangle.Width + target.X] = Color.White;
                    } 
                    for (int reach = 16; reach >= 0; reach--)
                    {
                        int y = (int)(Math.Round(Math.Sin(angle) * reach));
                        int x = (int)(Math.Round(Math.Cos(angle) * reach));
                        Point target = new Point(centerPoints[i].X + x, centerPoints[i].Y + y);
                        dataFill[target.Y * rectangle.Width + target.X] = Color.White;
                    }
                }
            }
            textureFill.SetData(dataFill);
            textureDraw.SetData(dataDraw);
            if (!containsKeyDimension)
            {
                roundedRectangleCache.Add(rectDimensions, new List<RoundedRectangle>());
            }
            roundedRectangleCache[rectDimensions].Add(new RoundedRectangle(textureFill, textureDraw, width));
            InnerDrawRoundedRectangle(rectangle, innerColor, outerColor, width, doFill, doDrawBorder);
        }
        private static readonly Dictionary<Vector2, List<RoundedRectangle>> roundedRectangleCache = new Dictionary<Vector2, List<RoundedRectangle>>();

        /* CIRCLES */
        /// <summary>
        /// Draws a filled circle. Unlike the non-quick method, this is much less CPU-intensive but may produce pixelated look
        /// on extremely small or extremely large circles. It is recommended you use this instead of the non-quick method.
        /// </summary>
        public static void FillCircleQuick(Vector2 center, int radius, Color color)
        {
            float scale = radius / 500f;
            SpriteBatch.Draw(Library.Circle1000X1000, center, null, color, 0, new Vector2(500, 500), scale, SpriteEffects.None, 0);
        }
        /// <summary>
        /// Draws an outline of a circle. Unlike the non-quick method, this is much less CPU-intensive, however, it does not allow you to 
        /// specify the width of the outline. If you need to specify that, you must use the non-quick method. In that case, however, it is recommended
        /// that you do not change the width or the radius often as whenever you do, the texture of the circle is redrawn which causes a CPU slowdown. 
        /// It may also miss some pixels at radii smaller than 20 pixels.
        /// (This class keeps a cache of circle textures and stores them in a dictionary based on radius and thickness)
        /// </summary>
        public static void DrawCircleQuick(Vector2 center, int radius, Color color)
        {
            float scale = radius / 500f;
            SpriteBatch.Draw(Library.EmptyCircle1000X1000, center, null, color, 0, new Vector2(500, 500), scale, SpriteEffects.None, 0);
        }
        /// <summary>
        /// Draws a filled circle. 
        /// WARNING! This and the DrawCircle method store circle textures in memory for performance reasons.
        /// If you draw multiple circles of different radii, you may have both performance and memory problems.
        /// </summary>
        public static void FillCircle(Vector2 center, int radius, Color color)
        {
            InnerDrawCircle(center, radius, color, true, 1);
        }    
        /// <summary>
        /// Draws an outline of a circle. 
        /// WARNING! This and the FillCircle method store circle textures in memory for performance reasons.
        /// If you draw multiple circles of different radii, you may have both performance and memory problems.
        /// </summary>
        public static void DrawCircle(Vector2 center, int radius, Color color, int thickness = 1)
        {
            InnerDrawCircle(center, radius, color, false, thickness);
        }
        /// <summary>
        /// Clears all cached Circle textures. This will clear space from memory, but drawing circles will take longer. Then runs garbage collector.
        /// </summary>
        public static void ClearCircleCache()
        {
            circlesCache.Clear();
            GC.Collect();
        }
        private static void InnerDrawCircle(Vector2 center, int radius, Color color, bool filled, int thickness)
        {
            bool containsKeyRadius = circlesCache.ContainsKey(radius);
            if (containsKeyRadius)
            {
                foreach(Circle c in circlesCache[radius])
                {
                    if (c.Filled == filled && (filled || c.Thickness == thickness) && c.Color == color)
                    {
                        SpriteBatch.Draw(c.Texture, center, null, color, 0, new Vector2(radius + 1, radius + 1), 1, SpriteEffects.None, 0);
                        return;
                    }
                }
            }
            int outerRadius = radius * 2 + 2;
            Texture2D texture = new Texture2D(Primitives.graphicsDevice, outerRadius, outerRadius);
            if (filled) thickness = radius + 1;

            Color[] data = new Color[outerRadius * outerRadius];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            double angleStep = 0.5f / radius;

            int lowpoint = radius - thickness;
            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                for (int i = radius; i > lowpoint; i--)
                {
                    int x = (int)Math.Round(radius + i * Math.Cos(angle));
                    int y = (int)Math.Round(radius + i * Math.Sin(angle));
                    data[y * outerRadius + x + 1] = color;
                }
            }
            texture.SetData(data);
            if (containsKeyRadius)
                circlesCache[radius].Add(new Circle(texture, filled, thickness, color));
            else
                circlesCache.Add(radius, new List<Circle>(new Circle[] { new Circle(texture, filled, thickness, color) }));

            SpriteBatch.Draw(texture, center, null, color, 0, new Vector2(radius + 1, radius + 1), 1, SpriteEffects.None, 0);
        }
        private static readonly Dictionary<int, List<Circle>> circlesCache = new Dictionary<int, List<Circle>>();

        /// <summary>
        /// This method is called automatically from Root.Init(). It will enable the use of this static class.
        /// </summary>
        public static void Init(SpriteBatch spriteBatchParameter, GraphicsDevice graphicsDeviceParameter)
        {
            Primitives.SpriteBatch = spriteBatchParameter;
            Primitives.graphicsDevice = graphicsDeviceParameter;
        }
        private class Circle
        {
            public readonly Texture2D Texture;
            public readonly bool Filled;
            public readonly int Thickness;
            internal Color Color;

            public Circle(Texture2D texture, bool filled, int thickness, Color color)
            {
                Texture = texture; Filled = filled; Thickness = thickness;
                Color = color;
            }
        }
        private class RoundedRectangle
        {
            public readonly Texture2D FillInTexture;
            public readonly Texture2D BorderTexture;
            public readonly int Thickness;
            public RoundedRectangle(Texture2D fillInTexture, Texture2D borderTexture, int thickness)
            {
                Thickness = thickness;
                FillInTexture = fillInTexture;
                BorderTexture = borderTexture;
            }
        }
    }
 
}
