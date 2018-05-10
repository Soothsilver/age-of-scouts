using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Auxiliary.GUI
{
    /// <summary>
    /// Represents an Auxiliary text box. This class utilizes Win32 API and will not work on Mono.
    /// </summary>
    public class Textbox : UIElement
    {  
        /// <summary>
        /// Called when the user presses Enter while this text box is active. Is called only if the text box is not multiline.
        /// </summary>
        public event Action<Textbox> EnterPress;
        /// <summary>
        /// Calls the EnterPress event.
        /// </summary>
        /// <param name="textbox"></param>
        protected virtual void OnEnterPress(Textbox textbox)
        {
            if (EnterPress != null)
                EnterPress(textbox);
        }
        /// <summary>
        /// Text in the text box.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Determines whether the text is displayed as a series of asterisks instead.
        /// </summary>
        public bool IsPassword { get; set; }
        /// <summary>
        /// Determines whether the text box is instead a text area.
        /// </summary>
        public bool IsMultiline { get; set; }
        /// <summary>
        /// Activates the textbox, clearing the keyboard buffer.
        /// </summary>
        public override void Activate()
        {
            Root.KeyboardInput.ClearBuffer();
            base.Activate();
        }
        /// <summary>
        /// Handles key presses while the text box is the active control.
        /// </summary>
        public override void Update()
        {
            if (this.IsActive)
            {
                if (Root.KeyboardInput.Buffer != "")
                {
                    string sbuffer = "";
                    foreach (char c in Root.KeyboardInput.Buffer)
                    {
                        if (!Char.IsControl(c))
                        {
                            sbuffer += c;
                        }
                    }
                    Text += sbuffer;
                    Root.KeyboardInput.ClearBuffer();
                }
                if (Root.KeyboardInput.BackSpace)
                {
                    if (Text.Length > 0) Text = Text.Substring(0, Text.Length - 1);
                }
                if (Root.WasKeyPressed(Keys.Enter, ModifierKey.Ctrl))
                {
                        OnEnterPress(this);
                }
                else if (Root.WasKeyPressed(Keys.Enter))
                {
                    if (IsMultiline) Text += Environment.NewLine;
                    else
                        OnEnterPress(this);
                }
                
            }
            base.Update();
        }

        /// <summary>
        /// Draws the textbox.
        /// </summary>
        public override void Draw()
        {
            string txt = Text;
            if (IsPassword)
            {
                txt = "";
                for (int i = 0; i < Text.Length; i++)
                    txt += "*";

            }
            Color outerBorderColor = Skin.OuterBorderColor;
            Color innerBorderColor = Skin.InnerBorderColor;
            Color innerButtonColor = Skin.WhiteBackgroundColor;
            Color textColor = Skin.TextColor;
            Primitives.FillRectangle(Rectangle, innerBorderColor);
            Primitives.DrawRectangle(Rectangle, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(InnerRectangleWithBorder, innerButtonColor, outerBorderColor, Skin.OuterBorderThickness);
            BasicStringDrawer.DrawMultiLineText(txt + (Root.SecondsSinceStartInt % 2 == 0 && IsActive ? "|" : ""), new Rectangle(InnerRectangle.X + 8, InnerRectangle.Y + 3, InnerRectangle.Width - 10, InnerRectangle.Height - 4), textColor, Skin.Font);
        }
        /// <summary>
        /// Creates a single-line text box with 40px height.
        /// </summary>
        /// <param name="text">Initial text.</param>
        /// <param name="x">The X coordinate of the top left corner.</param>
        /// <param name="y">The Y coordinate of the top left corner.</param>
        /// <param name="width">Width of the textbox in pixels.</param>
        public Textbox(string text, int x, int y, int width)
        {
            const int defaultheight = 40;
            Rectangle = new Rectangle(x, y, width, defaultheight);
            Text = text;
        }
        /// <summary>
        /// Creates a general text box.
        /// </summary>
        /// <param name="text">Initial text.</param>
        /// <param name="rect">Space occupied by the textbox.</param>
        /// <param name="multiline">Determines whether the textbox is multiline.</param>
        public Textbox(string text, Rectangle rect, bool multiline = false)
        {
            Rectangle = rect;
            IsMultiline = multiline;
            Text = text;
        }
    }
  
}
