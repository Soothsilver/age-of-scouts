using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using System.Text;
using System.Linq;

namespace Auxiliary
{
    public enum FontFamily
    {
        Tiny,
        Mid,
        Normal
    }
    public class FontGroup
    {
        public readonly SpriteFont Regular;
        public readonly SpriteFont Italics;
        public readonly SpriteFont Bold;
        public readonly SpriteFont BoldItalics;
        public FontGroup(SpriteFont regular, SpriteFont italics, SpriteFont bold, SpriteFont boldItalics)
        {
            Regular = regular; Italics = italics; Bold = bold; BoldItalics = boldItalics;
        }
    }
    public static partial class Primitives
    {
        public static readonly Dictionary<FontFamily, FontGroup> Fonts = new Dictionary<FontFamily, FontGroup>();
        private static readonly List<MultilineString> multilineStringCache = new List<MultilineString>();

        public static void FillRectangleGradient(Rectangle rectangle, params Color[] colors)
        {
            if (colors.Length == 1)
            {
                FillRectangle(rectangle, colors[0]);
                return;
            }
            int spanWidth = rectangle.Width / (colors.Length - 1);
            for (int ci = 0; ci < colors.Length - 1; ci++)
            {
                Color first = colors[ci];
                Color second = colors[ci + 1];
                int COLS = 20;
                int colwidth = spanWidth / COLS;
                for (int i = 0; i < COLS; i++)
                {
                    Rectangle r = new Rectangle(
                         rectangle.X + spanWidth * ci + i * colwidth,
                         rectangle.Y,
                         i == COLS - 1 ? spanWidth - (COLS - 1) * colwidth : colwidth,
                         rectangle.Height
                        );
                    float p = ((float)(COLS - 1 - i)) / (COLS - 1);
                    float rp = 1 - p;
                    FillRectangle(r, Color.Lerp(first, second, p));/*
                        new Color(
                            (int)(first.R * p + second.R * rp),
                            (int)(first.G * p + second.G * rp),
                           (int)(first.B * p + second.B * rp),
                          (int)(first.A * p + second.A * rp)));*/

                }
            }
        }

        /// <summary>
        /// Clears the cache of multiline string. Then runs garbage collector. (Searching the cache is faster than drawing a multiline string from cache, but if the cache has too many unused entries, searching it is slow.)
        /// </summary>
        public static void ClearStringCache()
        {
            multilineStringCache.Clear();
            GC.Collect();
        }

        /// <summary>
        /// Draws a multiline text using the Primitives spritebatch.
        /// </summary>
        /// <param name="text">Text to be drawn.</param>
        /// <param name="rectangle">The rectangle bounding the text.</param>
        /// <param name="color">Text color.</param>
        /// <param name="font">Text font (Verdana 14, if null).</param>
        /// <param name="alignment">Text alignment.</param>
        public static void DrawMultiLineText(
            string text,
            Rectangle rectangle,
            Color color,
            FontFamily font = FontFamily.Tiny,
            TextAlignment alignment = TextAlignment.TopLeft,
            bool shadowed = false)
        {
            MultilineString ms = new MultilineString(text, rectangle, alignment, font, Vector2.Zero, true, null, color);
            foreach (MultilineString msCache in multilineStringCache)
            {
                if (ms.Equals(msCache))
                {
                    foreach (MultilineFragment line in msCache.CachedLines)
                    {
                        if (line.Icon != null)
                        {
                            spriteBatch.Draw(line.Icon,
                                new Rectangle(rectangle.X + (int)line.PositionOffset.X, rectangle.Y + (int)line.PositionOffset.Y,
                                line.IconWidth, line.IconWidth), Color.White);
                        }
                        else
                        {
                            if (line.Color != color || shadowed)
                            {
                                Color shadowColor = line.Color.Alpha(120);
                                if (shadowed)
                                {
                                    shadowColor = Color.White;
                                }
                                spriteBatch.DrawString(line.Font, line.Text,
                                    new Vector2(rectangle.X + (int)line.PositionOffset.X - 1,
                                    rectangle.Y + (int)line.PositionOffset.Y - 1), shadowColor);
                                spriteBatch.DrawString(line.Font, line.Text,
                                    new Vector2(rectangle.X + (int)line.PositionOffset.X + 1,
                                    rectangle.Y + (int)line.PositionOffset.Y - 1), shadowColor);
                                spriteBatch.DrawString(line.Font, line.Text,
                                    new Vector2(rectangle.X + (int)line.PositionOffset.X - 1,
                                    rectangle.Y + (int)line.PositionOffset.Y + 1), shadowColor);
                                spriteBatch.DrawString(line.Font, line.Text,
                                    new Vector2(rectangle.X + (int)line.PositionOffset.X + 1,
                                    rectangle.Y + (int)line.PositionOffset.Y + 1), shadowColor);

                            }
                            spriteBatch.DrawString(line.Font, line.Text,
                                new Vector2(rectangle.X + (int)line.PositionOffset.X,
                                rectangle.Y + (int)line.PositionOffset.Y), color);

                        }
                    }
                    return;
                }
            }
            Primitives.SetupNewMultilineString(font, text, rectangle, color, alignment, out Rectangle empty, out List<MultilineFragment> cachedLines);
            multilineStringCache.Add(new MultilineString(text, rectangle, alignment, font, Vector2.Zero, true, cachedLines, color));
            Primitives.DrawMultiLineText(text, rectangle, color, font, alignment);
        }


        /// <summary>
        /// If the text were written to the specified rectangle, how much width and height would it actually use?
        /// This method ignores the rectangle's X and Y properties.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="rectangle">Rectangle bounding the text.</param>
        /// <param name="font">Font to use.</param>
        public static Rectangle GetMultiLineTextBounds(string text, Rectangle rectangle, FontFamily font = FontFamily.Mid)
        {
            Primitives.SetupNewMultilineString(font, text, rectangle, Color.Black, TextAlignment.TopLeft, out Rectangle bounds, out List<MultilineFragment> empty);
            return bounds;
        }
        /// <summary>
        /// Specifies text alignment.
        /// </summary>
        public enum TextAlignment
        {
            /// <summary>
            /// Align to top.
            /// </summary>
            Top,
            /// <summary>
            /// Align to left.
            /// </summary>
            Left,
            /// <summary>
            /// Align to middle.
            /// </summary>
            Middle,
            /// <summary>
            /// Align to right.
            /// </summary>
            Right,
            /// <summary>
            /// Align to bottom.
            /// </summary>
            Bottom,
            /// <summary>
            /// Align to top left.
            /// </summary>
            TopLeft,
            /// <summary>
            /// Allign to top right.
            /// </summary>
            TopRight,
            /// <summary>
            /// Align to bottom left.
            /// </summary>
            BottomLeft,
            /// <summary>
            /// Align to bottom right.
            /// </summary>
            BottomRight
        }

        /// <summary>
        /// This class is used only internally when calculating how to print out a multiline string.
        /// </summary>
        private class Data
        {
            public StringBuilder ReadyFragment;
            public StringBuilder Builder;
            /// <summary>
            /// Font that will be used to print text now,maybe bold or italic.
            /// </summary>
            public SpriteFont CurrentFont;
            /// <summary>
            /// Total width of the fragments that were already committed to the line that's being constructed.
            /// </summary>
            public float CurrentX;
            /// <summary>
            /// Vertical coordinates of the line that's being constructed, starting at 0, incrementing by LineHeight.
            /// </summary>
            public int CurrentY;
            /// <summary>
            /// Number of lines that are already finalized.
            /// </summary>
            public int TotalNumberOfLines;
            /// <summary>
            /// Maximum permissible width of this multiline string,
            /// </summary>
            public int Width;
            /// <summary>
            /// Maximum permissible height of this multiline text.
            /// </summary>
            public int Height;
            /// <summary>
            /// Constant. Height of a single line.
            /// </summary>
            public int LineHeight;
            /// <summary>
            /// Fonts used in this multiline string.
            /// </summary>
            public FontGroup FontGroup;
            /// <summary>
            /// If true, then we have already overreached our vertical bounds and must stop constructing additional text.
            /// </summary>
            public bool End;
            /// <summary>
            /// List of already constructed lines
            /// </summary>
            public List<List<MultilineFragment>> Lines;
            /// <summary>
            /// Commmitted fragments of the line that's being constructed
            /// </summary>
            public List<MultilineFragment> ThisLine;
            /// <summary>
            /// What color are we currently using to write.
            /// </summary>
            public Color Color;
            /// <summary>
            /// The width of the widest line committed so far
            /// </summary>
            public float Maximumwidthencountered;
            /// <summary>
            /// Whether we are now writing bold text.
            /// </summary>
            internal bool IsBold;
            /// <summary>
            /// Whether we are now writing italics text.
            /// </summary>
            internal bool IsItalics;

            internal void UpdateFont()
            {
                if (this.IsBold && this.IsItalics)
                    this.CurrentFont = FontGroup.BoldItalics;
                else if (this.IsBold)
                    this.CurrentFont = FontGroup.Bold;
                else if (this.IsItalics)
                    this.CurrentFont = FontGroup.Italics;
                else
                    this.CurrentFont = FontGroup.Regular;
            }
        }

        /// <summary>
        /// Draws a multi-string. 
        /// WARNING! This grows more CPU-intensive as the number of words grow (only if word wrap enabled). It is recommended to use the DrawMultiLineText method instead - it uses caching.
        /// </summary>
        /// <param name="fnt">A reference to a SpriteFont object.</param>
        /// <param name="text">The text to be drawn. <remarks>If the text contains \n it
        /// will be treated as a new line marker and the text will drawn acordingy.</remarks></param>
        /// <param name="r">The screen rectangle that the rext should be drawn inside of.</param>
        /// <param name="col">The color of the text that will be drawn.</param>
        /// <param name="align">Specified the alignment within the specified screen rectangle.</param>
        /// <param name="textBounds">Returns a rectangle representing the size of the bouds of
        /// the text that was drawn.</param>
        /// <param name="cachedLines">This parameter is internal. Do not use it, merely throw away the variable.</param>
        private static void SetupNewMultilineString(FontFamily fnt, string text, Rectangle r,
        Color col, TextAlignment align, out Rectangle textBounds, out List<MultilineFragment> cachedLines)
        {
            textBounds = r;
            cachedLines = new List<MultilineFragment>();
            if (text == string.Empty) return;

            Data d = new Data
            {
                FontGroup = Fonts[fnt],
                ReadyFragment = new StringBuilder(),
                Builder = new StringBuilder()
            };
            d.CurrentFont = d.FontGroup.Regular;
            d.CurrentX = 0;
            d.CurrentY = 0;
            d.TotalNumberOfLines = 0;
            d.Width = r.Width;
            d.Height = r.Height;
            d.IsBold = false;
            d.IsItalics = false;
            d.LineHeight = d.FontGroup.Regular.LineSpacing;
            d.Color = col;
            d.End = false;
            d.Lines = new List<List<MultilineFragment>>();
            d.ThisLine = new List<MultilineFragment>();
            foreach (char t in text)
            {
                if (t == '{')
                {
                    // Flush and write.
                    Primitives.FlushAndWrite(d);
                    // Ready to change mode.
                }
                else if (t == '}')
                {
                    // Change mode.
                    switch (d.Builder.ToString())
                    {
                        case "b": d.IsBold = !d.IsBold; d.UpdateFont(); break;
                        case "i": d.IsItalics = !d.IsItalics; d.UpdateFont(); break;
                        case "/b": d.IsBold = !d.IsBold; d.UpdateFont(); break;
                        case "/i":
                            d.IsItalics = !d.IsItalics; d.UpdateFont(); break;
                        default:
                            string tag = d.Builder.ToString();
                            if (tag.StartsWith("icon:"))
                            {
                                Texture2D icon = Library.Icons[tag.Substring(5)];
                                d.Builder.Clear();
                                // Now add icon.
                                int iconwidth = d.LineHeight;
                                int remainingSpace = (int)(d.Width - d.CurrentX);
                                if (remainingSpace > iconwidth + 3)
                                {
                                    d.ThisLine.Add(new MultilineFragment(icon,
                                        new Vector2(d.CurrentX, d.CurrentY), iconwidth));
                                    d.CurrentX += iconwidth + 3;
                                }
                                else
                                {
                                    Primitives.FlushAndWrite(d);
                                    Primitives.GoToNextLine(d);

                                    d.ThisLine.Add(new MultilineFragment(icon,
                                        new Vector2(d.CurrentX, d.CurrentY), iconwidth));
                                    d.CurrentX += iconwidth + 3;
                                }
                                break;
                            }
                            d.Color = Primitives.ColorFromString(tag);
                            if (tag[0] == '/')
                            {
                                d.Color = col;
                            }
                            break;
                    }
                    d.Builder.Clear();
                }
                else if (t == ' ')
                {
                    // Flush. 
                    // Add builder to ready.
                    string without = d.ReadyFragment.ToString();
                    d.ReadyFragment.Append(d.Builder);
                    if (d.CurrentFont.MeasureString(d.ReadyFragment.ToString()).X <= d.Width - d.CurrentX)
                    {
                        // It will fit.
                        d.Builder.Clear();
                        d.Builder.Append(' ');
                    }
                    else
                    {
                        // It would not fit.
                        d.ReadyFragment = new StringBuilder(without);
                        string newone = d.Builder.ToString();
                        d.Builder = new StringBuilder();
                        Primitives.FlushAndWrite(d);
                        Primitives.GoToNextLine(d);
                        d.Builder.Append(newone);
                        Primitives.FlushAndWrite(d);
                        d.Builder.Clear();
                        d.Builder.Append(' ');
                    }
                    // Write if overflowing.
                }
                else if (t == '\n')
                {
                    // Flush and write.
                    Primitives.FlushAndWrite(d);
                    // Skip to new line automatically.
                    Primitives.GoToNextLine(d);
                }
                else
                {
                    d.Builder.Append(t);

                }
                if (d.End) break;
            }
            // Flush and write.
            FlushAndWrite(d);
            FinishLine(d);
            d.TotalNumberOfLines += 1;
            // Modify for non-top-left.

            // Output.
            textBounds = new Rectangle(r.X, r.Y, (int)d.Maximumwidthencountered, d.TotalNumberOfLines * d.LineHeight);
            int yoffset = 0;
            switch (align)
            {
                case TextAlignment.TopLeft:
                case TextAlignment.Top:
                case TextAlignment.TopRight:
                    break;
                case TextAlignment.Bottom:
                case TextAlignment.BottomLeft:
                case TextAlignment.BottomRight:
                    yoffset = r.Height - d.TotalNumberOfLines * d.LineHeight;
                    break;
                case TextAlignment.Middle:
                case TextAlignment.Left:
                case TextAlignment.Right:
                    yoffset = (r.Height - (d.TotalNumberOfLines * d.LineHeight)) / 2;
                    break;
            }
            foreach (var line in d.Lines)
            {
                foreach (var fragment in line)
                {
                    if (fragment.Text == "") continue;
                    float xoffset = 0;
                    float lineWidth = line.Sum(frg => frg.Width);
                    switch (align)
                    {
                        case TextAlignment.Right:
                        case TextAlignment.TopRight:
                        case TextAlignment.BottomRight:
                            xoffset = r.Width - lineWidth;
                            break;
                        case TextAlignment.Middle:
                        case TextAlignment.Top:
                        case TextAlignment.Bottom:
                            xoffset = (r.Width - lineWidth) / 2;
                            break;
                    }
                    fragment.PositionOffset += new Vector2(xoffset, yoffset);
                    fragment.PositionOffset = new Vector2((int)fragment.PositionOffset.X, (int)fragment.PositionOffset.Y);
                    cachedLines.Add(fragment);
                }
            }

        }

        private static Color ColorFromString(string nameOfColor)
        {
            var prop = typeof(Color).GetProperty(nameOfColor);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return Color.Black;
        }

        /// <summary>
        /// Draws whatever is present in the readyFragment on the line. 
        ///  If the current buildfragment can also fit, it is drawn as well.
        ///  If the current buildfragment cannot fit, we go to the next line and it is drawn as well. 
        ///  At the end of this procedure, both readyfragment and buildfragment are empty and something is definitely in thisline (though it can be "").
        /// </summary>
        /// <param name="d"></param>
        private static void FlushAndWrite(Data d)
        {
            string total = d.ReadyFragment.ToString() + d.Builder;
            if (total == "") return;
            if (d.CurrentFont.MeasureString(total).X <= (d.Width - d.CurrentX))
            {
                // Write it and move X.
                d.ReadyFragment.Append(d.Builder);
                MultilineFragment frag = new MultilineFragment(d.ReadyFragment.ToString(), new Vector2(d.CurrentX, d.CurrentY), d.Color, d.CurrentFont);
                d.ThisLine.Add(frag);
                d.CurrentX += d.CurrentFont.MeasureString(total).X;
                d.Builder.Clear();
                d.ReadyFragment.Clear();
            }
            else
            {
                // Write what is there and clear it.
                MultilineFragment frag = new MultilineFragment(d.ReadyFragment.ToString(), new Vector2(d.CurrentX, d.CurrentY), d.Color, d.CurrentFont);
                d.ThisLine.Add(frag);
                d.ReadyFragment.Clear();
                // Flush the line.
                GoToNextLine(d);
                d.ReadyFragment.Append(d.Builder);
                MultilineFragment frag2 = new MultilineFragment(d.ReadyFragment.ToString(), new Vector2(d.CurrentX, d.CurrentY), d.Color, d.CurrentFont);
                d.ThisLine.Add(frag2);
                d.ReadyFragment.Clear();
                d.CurrentX += d.CurrentFont.MeasureString(d.Builder.ToString()).X;
            }
            d.Builder.Clear();
        }

        private static void GoToNextLine(Data d)
        {
            FinishLine(d);
            d.ThisLine = new List<MultilineFragment>();
            // Move one line down.
            d.CurrentY += d.LineHeight;
            d.TotalNumberOfLines += 1;
            d.CurrentX = 0;
            if (d.CurrentY + d.LineHeight > d.Height)
            {
                // End.
                d.End = true;
            }
        }

        private static void FinishLine(Data d)
        {
            d.Maximumwidthencountered = Math.Max(d.Maximumwidthencountered, d.CurrentX);
            d.Lines.Add(d.ThisLine);
        }




        /// <summary>
        /// Do not use this class outside Auxiliary 3.
        /// </summary>
        private class MultilineFragment
        {
            public Color Color;
            public readonly SpriteFont Font;
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public readonly string Text;
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public Vector2 PositionOffset;
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public MultilineFragment(string text, Vector2 position, Color clr, SpriteFont font)
            {
                Text = text;
                PositionOffset = position;
                Font = font;
                Color = clr;
            }
            public readonly Texture2D Icon;
            public readonly int IconWidth;
            public float Width => Icon == null ? Font.MeasureString(Text).X : IconWidth + 1;
            public MultilineFragment(Texture2D icon, Vector2 position, int width)
            {
                Icon = icon;
                PositionOffset = position;
                IconWidth = width;
            }
        }
        private class MultilineString
        {
            private readonly string text;
            private readonly Rectangle rectangle;
            private readonly TextAlignment textAlignment;
            private readonly FontFamily font;
            private readonly Vector2 offset;
            private readonly Color color;
            private readonly bool wordWrap;

            public readonly List<MultilineFragment> CachedLines;

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                MultilineString ms = (MultilineString)obj;
                return offset == ms.offset && wordWrap == ms.wordWrap && text == ms.text &&
                    rectangle.Width == ms.rectangle.Width &&
                    color == ms.color &&
                    rectangle.Height == ms.rectangle.Height
                    && textAlignment == ms.textAlignment && font == ms.font;
            }

            public override int GetHashCode()
            {
                // Does this hash really work?
                // Also, we are using implicit modulo.
                return (
                     color.GetHashCode() +
                    33 * 33 * 33 * 33 * 33 * 33 * text.GetHashCode() +
                    33 * 33 * 33 * 33 * 33 * rectangle.GetHashCode() +
                    33 * 33 * 33 * 33 * textAlignment.GetHashCode() +
                    33 * 33 * 33 * font.GetHashCode() +
                    33 * 33 * offset.GetHashCode() +
                    33 * wordWrap.GetHashCode());
            }
            public MultilineString(string text, Rectangle rect, TextAlignment alignment, FontFamily font, Vector2 offset, bool wordWrap, List<MultilineFragment> cachedLines, Color color)
            {
                this.CachedLines = cachedLines;
                this.text = text;
                rectangle = rect;
                textAlignment = alignment;
                this.font = font;
                this.offset = offset;
                this.wordWrap = wordWrap;
                this.color = color;
                CachedLines = cachedLines;
            }
        }
    }
}
