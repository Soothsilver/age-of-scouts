using System;
using System.Collections.Generic;
using System.Linq;
using Auxiliary.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Auxiliary
{
    /// <summary>
    /// An abstract class that represents a layer of UI. These layers ("phases") are stacked on top of each other. All of them get drawn, but only the top one gets updated.
    /// </summary>
    public abstract class GamePhase
    {
        /// <summary>
        /// List of UI elements drawn during the phase.
        /// </summary>
        public List<UIElement> UIElements = new List<UIElement>();
        /// <summary>
        /// What button the user clicked using the MessageBoxPhase directly on top of this phase, if any (Awaiting otherwise)
        /// </summary>
        public MessageBoxResult ReturnedMessageBoxResult = MessageBoxResult.Awaiting;
        /// <summary>
        /// Gets or sets the flag that determines whether this phase will be deleted during the next Root.Update() cycle.
        /// </summary>
        public bool ScheduledForElimination { get; set; }

        /// <summary>
        /// Adds a new UI element to this phase.
        /// </summary>
        public void AddUIElement(UIElement ui)
        {
            UIElements.Add(ui);
        }
        /// <summary>
        /// Virtual method. Override this to perform drawing this phase. The base method will draw all UIElements of this phase. 
        /// This method will be called regardless of whether this phase is on top of the stack.
        /// </summary>
        /// <param name="sb">The spriteBatch to use. The method assumes the spriteBatch is already Begun.</param>
        /// <param name="game">The game.</param>
        /// <param name="elapsedSeconds">Seconds elapsed since last draw cycle.</param>
        protected internal virtual void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Draw();
            }
        }
        /// <summary>
        /// Virtual method. Override this to perform updates in this phase. This method will only be called if this phase is on top of stack. The base method causes all UI Elements in the UIElement list to update.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="elapsedSeconds">Seconds elapsed since last update cycle.</param>
        protected internal virtual void Update(Game game, float elapsedSeconds)
        {
            foreach (UIElement uiElement in UIElements)
            {
                uiElement.Update();
            }
        }
        /// <summary>
        /// Performs any initialization code. Base method is empty.
        /// </summary>
        /// <param name="game">The game.</param>
        protected internal virtual void Initialize(Game game)
        {

        }
        /// <summary>
        /// Peforms any destruction code, then causes the phase to be flagged for removal from stack.
        /// Base method causes this by setting the ScheduledForElimination flag.
        /// </summary>
        /// <param name="game">The game.</param>
        public virtual void Destruct(Game game)
        {
            this.ScheduledForElimination = true;
        }
    }
}
