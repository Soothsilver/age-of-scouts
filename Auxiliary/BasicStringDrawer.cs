using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using static Auxiliary.Primitives;

namespace Auxiliary
{
    public static partial class BasicStringDrawer
    {
        private static readonly List<MultilineString> multilineStringCache = new List<MultilineString>();
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
            SpriteFont font = null,
            TextAlignment alignment = TextAlignment.TopLeft,
            bool shadowed = false)
        {
            if (font == null) font = Library.FontVerdana;
            MultilineString ms = new MultilineString(text, rectangle, alignment, font, Vector2.Zero, true, null);
            foreach (MultilineString msCache in multilineStringCache)
            {
                if (ms.Equals(msCache))
                {
                    foreach (MultilineLine line in msCache.CachedLines)
                    {
                        if (shadowed)
                        {
                            Color shadowColor = Color.White;/*
                            if (color.IsLight())
                            {
                                shadowColor = Color.Black;
                            }*/
                            Primitives.spriteBatch.DrawString(font, line.Text, new Vector2(rectangle.X + line.PositionOffset.X + 1, rectangle.Y + line.PositionOffset.Y -1), shadowColor);
                            Primitives.spriteBatch.DrawString(font, line.Text, new Vector2(rectangle.X + line.PositionOffset.X - 1, rectangle.Y + line.PositionOffset.Y - 1), shadowColor);
                            Primitives.spriteBatch.DrawString(font, line.Text, new Vector2(rectangle.X + line.PositionOffset.X + 1, rectangle.Y + line.PositionOffset.Y + 1), shadowColor);
                            Primitives.spriteBatch.DrawString(font, line.Text, new Vector2(rectangle.X + line.PositionOffset.X - 1, rectangle.Y + line.PositionOffset.Y + 1), shadowColor);
                        }
                        Primitives.spriteBatch.DrawString(font, line.Text, new Vector2(rectangle.X + line.PositionOffset.X, rectangle.Y + line.PositionOffset.Y), color);
                    }
                    return;
                }
            }
            DrawMultiLineTextDetailedParameters(Primitives.spriteBatch, font, text, rectangle, color, alignment, true, new Vector2(0, 0), out Rectangle empty, out List<MultilineLine> cachedLines, shadowed: true);
            multilineStringCache.Add(new MultilineString(text, rectangle, alignment, font, Vector2.Zero, true, cachedLines));
            
        }
        /// <summary>
        /// If the text were written to the specified rectangle, how much width and height would it actually use?
        /// This method ignores the rectangle's X and Y properties.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="rectangle">Rectangle bounding the text.</param>
        /// <param name="font">Font to use.</param>
        public static Rectangle GetMultiLineTextBounds(string text, Rectangle rectangle, SpriteFont font = null)
        {
            if (font == null) font = Library.FontVerdana;
            DrawMultiLineTextDetailedParameters(Primitives.spriteBatch, font, text, rectangle, Color.Black, TextAlignment.TopLeft, true, new Vector2(0, 0), out Rectangle bounds, out List<MultilineLine> empty, shadowed: false, onlyGetBounds: true);
            return bounds;
        }
     

        /// <summary>
        /// Draws a multi-string. 
        /// WARNING! This grows more CPU-intensive as the number of words grow (only if word wrap enabled). It is recommended to use the DrawMultiLineText method instead - it uses caching.
        /// </summary>
        /// <param name="sb">A reference to a SpriteBatch object that will draw the text.</param>
        /// <param name="fnt">A reference to a SpriteFont object.</param>
        /// <param name="text">The text to be drawn. <remarks>If the text contains \n it
        /// will be treated as a new line marker and the text will drawn acordingy.</remarks></param>
        /// <param name="r">The screen rectangle that the rext should be drawn inside of.</param>
        /// <param name="col">The color of the text that will be drawn.</param>
        /// <param name="align">Specified the alignment within the specified screen rectangle.</param>
        /// <param name="performWordWrap">If true the words within the text will be aranged to rey and
        /// fit within the bounds of the specified screen rectangle.</param>
        /// <param name="offsett">Draws the text at a specified offset relative to the screen
        /// rectangles top left position. </param>
        /// <param name="textBounds">Returns a rectangle representing the size of the bouds of
        /// the text that was drawn.</param>
        /// <param name="cachedLines">This parameter is internal. Do not use it, merely throw away the variable.</param>
        /// <param name="onlyGetBounds">Do not actually draw the text. Instead, merely return (in textBounds) the bounds.</param>
        public static void DrawMultiLineTextDetailedParameters(SpriteBatch sb, SpriteFont fnt, string text, Rectangle r,
        Color col, TextAlignment align, bool performWordWrap, Vector2 offsett, out Rectangle textBounds, out List<MultilineLine> cachedLines, bool shadowed, bool onlyGetBounds = false)
        {
            // check if there is text to draw
            textBounds = r;
            cachedLines = new List<MultilineLine>();
            if (text == null) return;
            if (text == string.Empty) return;

            StringCollection lines = new StringCollection();
            lines.AddRange(text.Split(new string[] { "\\n", Environment.NewLine, "\n" }, StringSplitOptions.None));

            // calc the size of the rectangle for all the text
            Rectangle tmprect = ProcessLines(fnt, r, performWordWrap, lines);
            if (onlyGetBounds) { textBounds = tmprect; return; }

            // setup the position where drawing will start
            Vector2 pos = new Vector2(r.X, r.Y);
            int aStyle = 0;

            switch (align)
            {
                case TextAlignment.Bottom:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 1;
                    break;
                case TextAlignment.BottomLeft:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 0;
                    break;
                case TextAlignment.BottomRight:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 2;
                    break;
                case TextAlignment.Left:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 0;
                    break;
                case TextAlignment.Middle:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 1;
                    break;
                case TextAlignment.Right:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 2;
                    break;
                case TextAlignment.Top:
                    aStyle = 1;
                    break;
                case TextAlignment.TopLeft:
                    aStyle = 0;
                    break;
                case TextAlignment.TopRight:
                    aStyle = 2;
                    break;
            }

            // draw text
            foreach (string txt in lines)
            {
                Vector2 size = fnt.MeasureString(txt);
                switch (aStyle)
                {
                    case 0:
                        pos.X = r.X;
                        break;
                    case 1:
                        pos.X = r.X + (r.Width / 2 - (size.X / 2));
                        break;
                    case 2:
                        pos.X = r.Right - size.X;
                        break;
                }
                if (pos.Y + fnt.LineSpacing > r.Y + r.Height) { break; }
                // draw the line of text
                pos = new Vector2((int)pos.X, (int)pos.Y);
                if (shadowed)
                {
                    sb.DrawString(fnt, txt, pos + offsett + new Vector2(1, -1), Color.Black);
                }
                sb.DrawString(fnt, txt, pos + offsett, col);
                cachedLines.Add(new MultilineLine(txt, pos + offsett - new Vector2(r.X, r.Y)));
                pos.Y += fnt.LineSpacing;
            }

            textBounds = tmprect;
        }

        internal static Rectangle ProcessLines(SpriteFont fnt, Rectangle r, bool performWordWrap, StringCollection lines)
        {
            // loop through each line in the collection
            Rectangle bounds = r;
            bounds.Width = 0;
            bounds.Height = 0;
            int index = 0;
            bool lineInserted = false;
            while (index < lines.Count)
            {
                // get a line of text
                string linetext = lines[index];
                //measure the line of text
                Vector2 size = fnt.MeasureString(linetext);

                // check if the line of text is geater then then the rectangle we want to draw it inside of
                if (performWordWrap && size.X > r.Width)
                {
                    // find last space character in line
                    string endspace = string.Empty;
                    // deal with trailing spaces
                    if (linetext.EndsWith(" "))
                    {
                        endspace = " ";
                        linetext = linetext.TrimEnd();
                    }

                    // get the index of the last space character
                    int i = linetext.LastIndexOf(" ", StringComparison.Ordinal);
                    if (i != -1)
                    {

                        // if there was a space grab the last word in the line
                        string lastword = linetext.Substring(i + 1);
                        // move word to next line
                        if (index == lines.Count - 1)
                        {
                            lines.Add(lastword);
                            lineInserted = true;
                        }
                        else
                        {
                            // prepend last word to begining of next line
                            if (lineInserted)
                            {
                                lines[index + 1] = lastword + endspace + lines[index + 1];
                            }
                            else
                            {
                                lines.Insert(index + 1, lastword);
                                lineInserted = true;
                            }
                        }

                        // crop last word from the line that is being processed

                        lines[index] = linetext.Substring(0, i + 1);

                    }
                    else
                    {

                        // there appear to be no space characters on this line s move to the next line

                        lineInserted = false;
                        // size = fnt.MeasureString(lines[index]);
                        bounds.Height += fnt.LineSpacing;// size.Y - 1;
                        index++;
                    }
                }
                else
                {
                    // this line will fit so we can skip to the next line
                    lineInserted = false;

                    size = fnt.MeasureString(lines[index]);
                    if (size.X > bounds.Width) bounds.Width = (int)size.X;
                    bounds.Height += fnt.LineSpacing;//size.Y - 1;
                    index++;
                }
            }

            // returns the size of the text
            return bounds;
        }


        /// <summary>
        /// Do not use this class outside Auxiliary 3.
        /// </summary>
        public class MultilineLine
        {
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public string Text;
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public Vector2 PositionOffset;
            /// <summary>
            /// Do not use this class outside Auxiliary 3.
            /// </summary>
            public MultilineLine(string text, Vector2 position)
            {
                Text = text;
                PositionOffset = position;
            }
        }
        private class MultilineString
        {
            private readonly string text;
            private readonly Rectangle rectangle;
            private readonly TextAlignment textAlignment;
            private readonly SpriteFont font;
            private readonly Vector2 offset;
            private readonly bool wordWrap;
            public readonly List<MultilineLine> CachedLines;
            
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                MultilineString ms = (MultilineString)obj;
                return offset == ms.offset && wordWrap == ms.wordWrap && text == ms.text &&
                    rectangle.Width == ms.rectangle.Width &&
                    rectangle.Height == ms.rectangle.Height
                    && textAlignment == ms.textAlignment && font == ms.font;
            }

            public override int GetHashCode()
            {
                // Does this hash really work?
                // Also, we are using implicit modulo.
                return (
                    33 * 33 * 33 * 33 * 33 * 33 * text.GetHashCode() +
                    33 * 33 * 33 * 33 * 33 *  rectangle.GetHashCode() +
                    33 * 33 * 33 * 33 *  textAlignment.GetHashCode() +
                    33 * 33 * 33 * font.GetHashCode() +
                    33 * 33 * offset.GetHashCode() +
                    33 * wordWrap.GetHashCode());
            }
            public MultilineString(string text, Rectangle rect, TextAlignment alignment, SpriteFont font, Vector2 offset, bool wordWrap, List<MultilineLine> cachedLines)
            {
                this.CachedLines = cachedLines;
                this.text = text;
                rectangle = rect;
                textAlignment = alignment;
                this.font = font;
                this.offset = offset;
                this.wordWrap = wordWrap;
                CachedLines = cachedLines;
            }
        }
    }
}
