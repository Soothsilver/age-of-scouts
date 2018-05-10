using System;
using System.Collections.Generic;
using System.Linq;
using Auxiliary.GUI;

namespace Auxiliary
{
    public partial class Root {
        internal static UIElement GuiActiveElement = null;
        internal static float SecondsSinceStart = 0;
        internal static int SecondsSinceStartInt = 0;
        /// <summary>
        /// The result returned from the last MessageBox removed from stack.
        /// </summary>
        public static MessageBoxResult ReturnedMessageBoxResult = MessageBoxResult.Awaiting;
        /// <summary>
        /// Creates a new message box and pushes it on the top of the stack.
        /// </summary>
        /// <param name="text">Text of the message box.</param>
        /// <param name="caption">Caption of the message box.</param>
        /// <param name="icon">Icon displayed in the message box.</param>
        /// <param name="buttons">Buttons displayed in the message box.</param>
        public static void MessageBox(string text, string caption = "Message", GuiIcon icon = GuiIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            MessageBoxPhase phaseMsgBox = new MessageBoxPhase(text, caption, icon, buttons);
            Root.PushPhase(phaseMsgBox);
        }
    }
}
