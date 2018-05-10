using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Auxiliary.GUI
{
    /// <summary>
    /// Represents an Auxiliary button.
    /// </summary>
    public class Button : UIElement
    {
        /// <summary>
        /// Text on the button.
        /// </summary>
        public string Caption {get; set;}
        /// <summary>
        /// Called whenever the user left-clicks the button.
        /// </summary>
        public event Action<Button> Click;

        /// <summary>
        /// Calls the Click event.
        /// </summary>
        protected virtual void OnClick(Button button)
        {
            if (Click != null)
            {
                Click(button);
            }
        }
        /// <summary>
        /// Calls OnClick when the button is clicked.
        /// </summary>
        public override void Update()
        {
            if (Root.WasMouseLeftClick && Root.IsMouseOver(Rectangle))
            {
                Root.ConsumeLeftClick();
                OnClick(this);
            }
            base.Update();
        }
        private bool isMouseOverThis;
        /// <summary>
        /// Creates an Auxiliary button.
        /// </summary>
        /// <param name="text">Text on the button.</param>
        /// <param name="rect">Space of the button.</param>
        public Button(string text, Rectangle rect)
        {
            Caption = text;
            Rectangle = rect;
        }
        /// <summary>
        /// Draws the button.
        /// </summary>
        public override void Draw()
        {
            isMouseOverThis = Root.IsMouseOver(Rectangle);
            bool pressed = isMouseOverThis && Root.Mouse_NewState.LeftButton == ButtonState.Pressed;
            Color outerBorderColor = isMouseOverThis ? Skin.OuterBorderColorMouseOver : Skin.OuterBorderColor;
            Color innerBorderColor = pressed ? Skin.InnerBorderColorMousePressed : (isMouseOverThis ? Skin.InnerBorderColorMouseOver : Skin.InnerBorderColor);
            Color innerButtonColor = isMouseOverThis ? Skin.GreyBackgroundColorMouseOver: Skin.GreyBackgroundColor;
            Color textColor = isMouseOverThis ? Skin.TextColorMouseOver : Skin.TextColor;
            Primitives.FillRectangle(Rectangle, innerBorderColor);
            Primitives.DrawRectangle(Rectangle, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(InnerRectangleWithBorder, innerButtonColor, outerBorderColor, Skin.OuterBorderThickness);
            BasicStringDrawer.DrawMultiLineText(Caption, InnerRectangle, textColor, Skin.Font, Primitives.TextAlignment.Middle);
        }
    }
 
}
