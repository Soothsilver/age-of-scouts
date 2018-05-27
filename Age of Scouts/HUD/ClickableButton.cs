using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.HUD
{
    class UI
    {
        public static Action MouseOverOnClickAction = null;
        public static Tooltip MajorTooltip = null;

        public static void DrawButton(Rectangle rectangle, bool interactible, string caption, Action onClick, string tooltip = null)
        {
            bool mo = Root.IsMouseOver(rectangle);
            bool held = Root.Mouse_NewState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            if (held && mo)
            {
                Primitives.DrawImage(Library.Get(TextureName.ButtonHoverBackground), rectangle, Color.White);
            }
            else
            {
                Primitives.DrawImage(Library.Get(TextureName.ButtonBackground), rectangle, Color.White.Alpha(mo & interactible ? 255 : 150));
            }
            Primitives.DrawSingleLineText(caption, new Vector2(rectangle.X + 10, rectangle.Y + 8), Color.Black, Library.FontNormal);
            if (mo && interactible)
            {
                MouseOverOnClickAction = onClick;
                if (tooltip != null)
                {
                    UI.MajorTooltip = new Tooltip(caption, tooltip);
                }
            }
        }
        internal static void DrawRadioButton(Rectangle rectangle, bool interactible, string caption, Action onClick, Func<bool> isChecked, string tooltip = null)
        {
            DrawCheckThing(rectangle, interactible, caption, onClick, isChecked, tooltip, true);
        }

        internal static void DrawCheckbox(Rectangle rectangle, bool interactible, string caption, Action onClick, Func<bool> isChecked, string tooltip = null)
        {
            DrawCheckThing(rectangle, interactible, caption, onClick, isChecked, tooltip, false);
        }
        private static void DrawCheckThing(Rectangle rectangle, bool interactible, string caption, Action onClick, Func<bool> isChecked, string tooltip, bool isRound)
        { 
            bool mo = Root.IsMouseOver(rectangle);
            Rectangle rectBox = new Rectangle(rectangle.X + 2, rectangle.Y + rectangle.Height / 4, rectangle.Height /2, rectangle.Height /2);
            Color backgroundColor = mo ? Color.Yellow : Color.White;
            if (isRound)
            {
                Vector2 center = new Vector2(rectBox.X + rectBox.Width / 2, rectBox.Y + rectBox.Height / 2);
                Primitives.FillCircle(center, rectBox.Width / 2, backgroundColor);
                Primitives.DrawCircle(center, rectBox.Width / 2, Color.Black, 2);
                if (isChecked())
                {
                    Primitives.FillCircle(center, rectBox.Width / 2 - 4, Color.Black);
                }
            }
            else
            {
                Primitives.DrawAndFillRectangle(rectBox, backgroundColor, Color.Black);
                if (isChecked())
                {
                    Primitives.FillRectangle(rectBox.Extend(-3, -3), Color.Black);
                }
            }
            Primitives.DrawSingleLineText(caption, new Vector2(rectBox.Right + 10, rectangle.Y + 8), Color.Black, Library.FontNormal);
            if (mo && interactible)
            {
                MouseOverOnClickAction = onClick;
                if (tooltip != null)
                {
                    UI.MajorTooltip = new Tooltip(caption, tooltip);
                }
            }
        }

        internal static void DrawSlider(Rectangle rectangle, bool interactible, string caption, Action<float> onSetValue, Func<float> getValue, string tooltip = null)
        {
            bool mo = Root.IsMouseOver(rectangle);
            Vector2 start = new Vector2(rectangle.X, rectangle.Y + rectangle.Height / 2);
            Vector2 end = new Vector2(rectangle.Right, rectangle.Y + rectangle.Height / 2);
            float percentage = getValue();
            float percentageOfMouse = (float)(Root.Mouse_NewState.X - rectangle.X) / rectangle.Width;
            Primitives.DrawLine(start, end, Color.Black, 2);
            float xMidPoint = start.X + rectangle.Width * percentage;
            Primitives.DrawLine(new Vector2(xMidPoint, rectangle.Y), new Vector2(xMidPoint, rectangle.Bottom), Color.Black, 2);
            Primitives.DrawSingleLineText(caption, new Vector2(rectangle.Right + 10, rectangle.Y + 8), Color.Black, Library.FontNormal);
            if (mo && interactible)
            {
                if (Root.Mouse_NewState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    onSetValue(percentageOfMouse);
                }
                if (tooltip != null)
                {
                    UI.MajorTooltip = new Tooltip(caption, tooltip);
                }
            }
        }

        internal static void DrawImageButton(Rectangle rectangle, bool interactible, string tooltipCaption, TextureName image, Action onClick, string tooltipDescription)
        {
            bool mo = Root.IsMouseOver(rectangle);
            Primitives.DrawImage(Library.Get(image), rectangle, Color.White.Alpha(mo & interactible ? 255 : 210));
            if (mo && interactible)
            {
                MouseOverOnClickAction = onClick;
                if (tooltipCaption != null)
                {
                    UI.MajorTooltip = new Tooltip(tooltipCaption, tooltipDescription);
                }
            }
        }

       

        internal static void DrawIconButton(Rectangle rectangle, bool interactible, Texture2D texture, string tooltipCaption, string tooltipDescription, Action onClick)
        {
            bool mo = Root.IsMouseOver(rectangle);
            Primitives.DrawImage(Library.Get(TextureName.Tile64x64), rectangle);
            Primitives.DrawImage(texture, rectangle, Color.White.Alpha(mo & interactible ? 255 : 210));
            if (mo && interactible)
            {
                MouseOverOnClickAction = onClick;
                UI.MajorTooltip = new Tooltip(tooltipCaption, tooltipDescription);
            }
        }
    }
}
