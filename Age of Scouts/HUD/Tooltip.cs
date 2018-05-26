using System;
using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.HUD
{
    public class Tooltip
    {
        public string Caption;
        public string Text;

        public Tooltip(string caption, string text)
        {
            Caption = caption;
            Text = text;
        }

        internal void Draw(Rectangle rectangle)
        {
            Primitives.FillRectangle(rectangle, Color.Brown.Alpha(190));
            Primitives.DrawSingleLineText(Caption, new Vector2(rectangle.X + 2, rectangle.Y + 2), Color.White, Library.FontTinyBold);
            Primitives.DrawMultiLineText(Text, new Rectangle(rectangle.X + 2, rectangle.Y + 22, rectangle.Width - 4, rectangle.Height - 30), Color.White, FontFamily.Tiny);
        }
    }
}