using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Auxiliary
{
    public static partial class Root
    {
        internal static readonly List<Toast> Toasts = new List<Toast>();
        private static void UpdateToasts(float elapsedSeconds)
        {
            bool changemade = true;
            while (changemade)
            {
                changemade = false;
                // Deciding about conditions
                foreach (Toast t in Toasts)
                {
                    // If something pushes from down, go up
                    int toast_upside = (int)t.Y;
                    int toast_downside = (int)t.Y + Toast.TOAST_HEIGHT;
                    bool moveupneeded = false;
                    int moveupatleasthere = Int32.MaxValue;
                    foreach (Toast t2 in Toasts)
                    {
                        if (t2 == t) continue;

                        if (!t.Ascending && t2.TargetY < toast_downside && t2.Y > toast_upside)
                        {
                            // Move up
                            moveupneeded = true;
                            if (moveupatleasthere > t2.TargetY - Toast.TOAST_HEIGHT)
                                moveupatleasthere = t2.TargetY - Toast.TOAST_HEIGHT;
                        }
                    }
                    
                    if (moveupneeded)
                    {
                        changemade = true;
                        t.TargetY = moveupatleasthere;
                        t.Ascending = true;
                    }
                }
            }
            // Moving
            foreach (Toast t in Toasts)
                t.Update(elapsedSeconds);

            // Removal
            for (int ti = Toasts.Count - 1; ti >= 0; ti--)
            {
                if (Toasts[ti].ScheduledForElimination) Toasts.RemoveAt(ti);
            }
        }

        public static void GoToBorderlessWindow(Resolution resolution)
        {
            Form.FormBorderStyle = FormBorderStyle.None;
            Form.Width = 1920;
            Form.Height = 1080;
            Form.WindowState = FormWindowState.Maximized;
        }

        public static Form Form;

        private static void DrawToasts()
        {
            foreach (Toast t in Toasts)
                t.Draw();
        }
        /// <summary>
        /// Creates a new short message to be displayed at the bottom of the screen.
        /// </summary>
        /// <param name="text">Message text.</param>
        /// <param name="color">Text color.</param>
        /// <param name="duration">Message duration in seconds.</param>
        /// <param name="icon">Prepended icon to the message.</param>
        public static void SendToast(string text, Color? color = null, float duration = Toast.TOAST_DEFAULT_DURATION, GuiIcon icon = GuiIcon.None)
        {
            int y = Root.ScreenHeight;
            Color clr = color ?? Color.Black;
            Toasts.Add(new Toast(text, clr,duration, y, icon));
        }
    }
    /// <summary>
    /// A basic user interface icon for use in Toasts and MessageBox.
    /// </summary>
    public enum GuiIcon
    {
        /// <summary>
        /// No icon.
        /// </summary>
        None,
        /// <summary>
        /// An information bubble.
        /// </summary>
        Information,
        /// <summary>
        /// A yellow warning triangle.
        /// </summary>
        Warning,
        /// <summary>
        /// A question mark bubble.
        /// </summary>
        Question,
        /// <summary>
        /// A red error bubble.
        /// </summary>
        Error
    }
    class Toast
    {
        public GuiIcon Icon = GuiIcon.None;
        public const int TOAST_HEIGHT = 40;
        public const float ALPHA_FADE_SPEED = 1;
        public const float DESCENT_SPEED = 40;
        public const float ASCENT_SPEED = 40;
        public const float TOAST_DEFAULT_DURATION = 5;
        public string Text;
        public Color ForeColor;
        public float Y;
        public int TargetY;
        public float Alpha = 1;
        public int AlphaMax = 200;
        public bool Fading;
        public bool Descending;
        public bool ScheduledForElimination;
        public bool Ascending;
        public float Duration;
        public void Draw()
        {
            Rectangle rect = new Rectangle(0, (int)Y, Root.ScreenWidth, TOAST_HEIGHT);
            Primitives.FillRectangle(rect, Color.FromNonPremultiplied(170, 170, 170, (int)(AlphaMax * Alpha)));
            int text_x = rect.X + 7;
            if (Icon != GuiIcon.None)
            {
                const int iconheight = TOAST_HEIGHT - 6;
                Primitives.DrawImage(Library.GetTexture2DFromGuiIcon(Icon), new Rectangle(text_x, rect.Y + 2, iconheight, iconheight), Color.FromNonPremultiplied(255,255,255,(int)(AlphaMax * Alpha)));
                text_x += iconheight + 7;
            }
            Primitives.DrawSingleLineText(Text, new Vector2(text_x, rect.Y + 6), Color.FromNonPremultiplied(ForeColor.R, ForeColor.G, ForeColor.B, (int)(Alpha * AlphaMax)));
        }
        public void Update(float elapsedSeconds)
        {
            if (!Ascending && !Descending)
            {
                Duration -= elapsedSeconds;
            }
            if (Duration <= 0 && !Fading)
            {
                Fading = true;
            }
            if (Fading)
            {
                Alpha -= elapsedSeconds * ALPHA_FADE_SPEED;
                if (Alpha <= 0)
                {
                    Alpha = 0;
                    ScheduledForElimination = true;
                    Fading = false;
                }
            }
            if (Descending)
            {
                Y += elapsedSeconds * DESCENT_SPEED;
                if (Y >= TargetY)
                {
                    Y = TargetY;
                    Descending = false;
                }
            }
            if (Ascending)
            {
                bool donotascend = false;
                foreach (Toast t2 in Root.Toasts)
                {
                    if (t2.Ascending && t2.Y < this.Y && t2.Y + TOAST_HEIGHT > this.Y)
                    {
                        donotascend = true;
                        break;
                    }
                }
                if (!donotascend)
                {
                    Y -= elapsedSeconds * ASCENT_SPEED;
                    if (Y <= TargetY)
                    {
                        Y = TargetY;
                        Ascending = false;
                    }
                }
            }
        }
        public Toast(string text, Color color, float duration, int y, GuiIcon icon)
        {
            Text = text;
            ForeColor = color;
            Duration = duration;
            Y = y;
            Icon = icon;
            Ascending = true;
            TargetY = Root.ScreenHeight - TOAST_HEIGHT;
        }
    }
}
