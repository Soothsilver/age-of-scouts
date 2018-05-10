using System;
using System.Collections.Generic;
using System.Linq;
using Auxiliary.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Auxiliary
{
    /// <summary>
    /// A game phase you can use to bring up a simple MessageBox-like dialog. When the phase completes, it will put its result in the MessageBoxResult field of the phase under it on stack.
    /// </summary>
    public class MessageBoxPhase : GamePhase
    {
        private string Text { get; set; }
        private string Caption { get; set; }
        private GuiIcon Icon { get; set; }
        private MessageBoxButtons ButtonsType { get; set; }
        private readonly List<Button> buttons = new List<Button>();
        /// <summary>
        /// Skin used for the buttons of this message box and for the message box itself.
        /// </summary>
        public GuiSkin Skin = GuiSkin.DefaultSkin;
        /// <summary>
        /// VideoWidth of the message box.
        /// </summary>
        protected int Width = 800;
        /// <summary>
        /// Height of the message box.
        /// </summary>
        protected int Height = 200;
        /// <summary>
        /// The X coordinate of the topleft corner of the message box.
        /// </summary>
        protected int TopLeftX;
        /// <summary>
        /// The Y coordinate of the topleft corner of the messagebox.
        /// </summary>
        protected int TopLeftY;

        /// <summary>
        /// Initializes the MessageBoxPhase. Sets its bounds and creates its buttons.
        /// </summary>
        protected internal override void Initialize(Game game)
        {
            Root.ReturnedMessageBoxResult = MessageBoxResult.Awaiting;
                   
            // Total width and height and X and Y
            Rectangle bounds = BasicStringDrawer.GetMultiLineTextBounds(Text, new Rectangle(0,0,700, 400), Skin.Font);
            Height = bounds.Height + 106;
            Width = bounds.Width + 65;
            TopLeftX = Root.ScreenWidth / 2 - Width / 2;
            TopLeftY = Root.ScreenHeight / 2 - Height / 2;
    
            // Arranging buttons
            int numbuttons = 0;
            bool doOKButton = false;
            bool doYesButton = false;
            bool doNoButton = false;
            bool doCancelButton = false;
            if (ButtonsType == MessageBoxButtons.OK) { numbuttons = 1; doOKButton = true; }
            if (ButtonsType == MessageBoxButtons.OKCancel) { numbuttons = 2; doOKButton = true; doCancelButton = true; }
            if (ButtonsType == MessageBoxButtons.YesNo) { numbuttons = 2; doYesButton = true; doNoButton = true; }
            if (ButtonsType == MessageBoxButtons.YesNoCancel) { numbuttons = 3; doYesButton = true; doNoButton = true; doCancelButton = true; }
            const int buttonwidth = 140;
            const int buttonspace = 20;
            if (Width < numbuttons * (buttonwidth + buttonspace) + 100)
                Width = numbuttons * (buttonwidth + buttonspace) + 100;
            const int buttonheight = 40;
            int buttony = TopLeftY + Height - 10 - buttonheight;
            int x = TopLeftX + Width / 2 - (((buttonwidth + buttonspace) * (numbuttons)) - buttonspace) / 2;
            if (doOKButton)
            {
                Button b = new Button("OK", new Rectangle(x, buttony, buttonwidth, buttonheight));
                b.Click += Button_Click;
                buttons.Add(b);
                x += buttonwidth + buttonspace;
            }
            if (doYesButton)
            {
                Button b = new Button("Yes", new Rectangle(x, buttony, buttonwidth, buttonheight));
                b.Click += Button_Click;
                buttons.Add(b);
                x += buttonwidth + buttonspace;
            }
            if (doNoButton)
            {
                Button b = new Button("No", new Rectangle(x, buttony, buttonwidth, buttonheight));
                b.Click += Button_Click;
                buttons.Add(b);
                x += buttonwidth + buttonspace;
            }
            if (doCancelButton)
            {
                Button b = new Button("Cancel", new Rectangle(x, buttony, buttonwidth, buttonheight));
                b.Click += Button_Click;
                buttons.Add(b);
            }
            base.Initialize(game);
        }

        void Button_Click(Button obj)
        {
            MessageBoxResult msgResult = MessageBoxResult.Cancel;
            if (obj.Caption == "OK") msgResult = MessageBoxResult.OK;
            if (obj.Caption == "Cancel") msgResult = MessageBoxResult.Cancel;
            if (obj.Caption == "Yes") msgResult = MessageBoxResult.Yes;
            if (obj.Caption == "No") msgResult = MessageBoxResult.No;

            if (Root.PhaseStack.Count > 1)
            {
                GamePhase gp = Root.PhaseStack[Root.PhaseStack.Count - 2];
                gp.ReturnedMessageBoxResult = msgResult;
                Root.ReturnedMessageBoxResult = msgResult;
            }
            Root.PopFromPhase();
        }

        /// <summary>
        /// Virtual method. Override this to perform drawing this phase. The base method will draw all UIElements of this phase. 
        /// This method will be called regardless of whether this phase is on top of the stack.
        /// </summary>
        /// <param name="sb">The spriteBatch to use. The method assumes the spriteBatch is already Begun.</param>
        /// <param name="game">The game.</param>
        /// <param name="elapsedSeconds">Seconds elapsed since last draw cycle.</param>
        protected internal override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Rectangle rectBox =new Rectangle(TopLeftX, TopLeftY, Width, Height);
            Rectangle rectTitle = new Rectangle(TopLeftX, TopLeftY, Width, 28);
            Primitives.DrawAndFillRectangle(rectBox, Skin.DialogBackgroundColor, Skin.OuterBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(rectTitle, Skin.InnerBorderColor, Skin.OuterBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawSingleLineText(Caption, new Vector2(TopLeftX + 5, TopLeftY + 3), Skin.TextColor, Skin.Font);
            if (Icon != GuiIcon.None)
                sb.Draw(Library.GetTexture2DFromGuiIcon(Icon), new Rectangle(rectBox.X + 5, rectBox.Y + 60, 45, 45), Color.White);
            BasicStringDrawer.DrawMultiLineText(Text, new Rectangle(rectBox.X + 55, rectBox.Y + 50, rectBox.Width - 65, rectBox.Height - 40), Skin.TextColor, Skin.Font, Primitives.TextAlignment.Top);
            foreach (Button b in buttons)
            {
                b.Draw();
            }
            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        /// <summary>
        /// Updates the message box and its buttons.
        /// </summary>
        protected internal override void Update(Game game, float elapsedSeconds)
        {
            foreach (Button b in buttons)
                b.Update();
            base.Update(game, elapsedSeconds);
        }
        /// <summary>
        /// Creates a new MessageBoxPhase. Use this to gain simple choices from the player.
        /// </summary>
        /// <param name="text">Text to display to the player.</param>
        /// <param name="caption">MessageBox caption.</param>
        /// <param name="icon">Icon displayed in the message box.</param>
        /// <param name="buttons">Buttons displayed in the message box.</param>
        public MessageBoxPhase(string text, string caption, GuiIcon icon, MessageBoxButtons buttons)
        {
            Text = text;
            Caption = caption;
            Icon = icon;
            ButtonsType = buttons;
        }
    }
    /// <summary>
    /// Buttons the MessageBoxPhase will display.
    /// </summary>
    public enum MessageBoxButtons
    {
        /// <summary>
        /// Only the "OK" button.
        /// </summary>
        OK,
        /// <summary>
        /// The "OK" and "Cancel" buttons.
        /// </summary>
        OKCancel,
        /// <summary>
        /// The "Yes" and "No" buttons.
        /// </summary>
        YesNo,
        /// <summary>
        /// The "Yes", "No" and "Cancel" buttons.
        /// </summary>
        YesNoCancel
    }
    /// <summary>
    /// Button pressed by the user during MessageBoxPhase.
    /// </summary>
    public enum MessageBoxResult
    {
        /// <summary>
        /// The "OK" button.
        /// </summary>
        OK,
        /// <summary>
        /// The "Cancel " button.
        /// </summary>
        Cancel,
        /// <summary>
        /// The "Yes" button.
        /// </summary>
        Yes,
        /// <summary>
        ///  The "No" button.
        /// </summary>
        No,
        /// <summary>
        /// No result has yet been returned from the MessageBoxPhase.
        /// </summary>
        Awaiting
    }
}
