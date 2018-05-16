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
