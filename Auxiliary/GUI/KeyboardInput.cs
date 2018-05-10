using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Auxiliary.GUI
{
    /// <summary>
    /// A class to provide text input capabilities to an XNA application via Win32 hooks.
    /// </summary>
    public class KeyboardInput : IDisposable
    {

        private string buffer = "";
        private bool backSpace = false;
        private bool enter = false;
        /// <summary>
        /// Text written on the keyboard that was not yet output into a textbox
        /// </summary>
        public string Buffer { get { return buffer; } }
        /// <summary>
        /// Is the Backspace key pressed?
        /// </summary>
        public bool BackSpace
        {
            get
            {
                bool b = backSpace;
                backSpace = false;
                return b;
            }

        }
        /// <summary>
        /// Returns true, if the Enter was pressed since this property was last read.
        /// </summary>
        public bool Enter
        {
            get
            {
                bool b = enter;
                enter = false;
                return b;
            }

        }
        /// <summary>
        /// Clears the keyboard buffer, forgetting any keypresses made but not yet used by the game.
        /// </summary>
        public void ClearBuffer() { buffer = ""; }

        #region Win32
        /// <summary>
        /// Types of hook that can be installed using the SetWindwsHookEx function.
        /// </summary>
        private enum HookId
        {
            WH_GETMESSAGE = 3
        };

        /// <summary>
        /// Window message types.
        /// </summary>
        /// <remarks>Heavily abridged, naturally.</remarks>
        private enum WindowMessage
        {
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x102,
        };

        /// <summary>
        /// A delegate used to create a hook callback.
        /// </summary>
        public delegate int GetMsgProc(int nCode, int wParam, ref Message msg);

        /// <summary>
        /// Install an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="idHook">Specifies the type of hook procedure to be installed.</param>
        /// <param name="lpfn">Pointer to the hook procedure.</param>
        /// <param name="hmod">Handle to the DLL containing the hook procedure pointed to by the lpfn parameter.</param>
        /// <param name="dwThreadId">Specifies the identifier of the thread with which the hook procedure is to be associated.</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure. Otherwise returns 0.</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowsHookExA")]
        private static extern IntPtr SetWindowsHookEx(HookId idHook, GetMsgProc lpfn, IntPtr hmod, int dwThreadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="hHook">Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx.</param>
        /// <returns>If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr hHook);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain.
        /// </summary>
        /// <param name="hHook">Ignored.</param>
        /// <param name="ncode">Specifies the hook code passed to the current hook procedure.</param>
        /// <param name="wParam">Specifies the wParam value passed to the current hook procedure.</param>
        /// <param name="lParam">Specifies the lParam value passed to the current hook procedure.</param>
        /// <returns>This value is returned by the next hook procedure in the chain.</returns>
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int hHook, int ncode, int wParam, ref Message lParam);

        /// <summary>
        /// Translates virtual-key messages into character messages.
        /// </summary>
        /// <param name="lpMsg">Pointer to an Message structure that contains message information retrieved from the calling thread's message queue.</param>
        /// <returns>If the message is translated (that is, a character message is posted to the thread's message queue), the return value is true.</returns>
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref Message lpMsg);


        /// <summary>
        /// Retrieves the thread identifier of the calling thread.
        /// </summary>
        /// <returns>The thread identifier of the calling thread.</returns>
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        #endregion

        #region Hook management and class construction.

        /// <summary>Handle for the created hook.</summary>
        private readonly IntPtr hookHandle;

        private readonly GetMsgProc processMessagesCallback;

        /// <summary>Creates a instance of a the KeyboardInput class. </summary>
        internal KeyboardInput()
        {
            // Create the delegate callback:
            this.processMessagesCallback = processMessages;
            // Create the keyboard hook:
            this.hookHandle = SetWindowsHookEx(HookId.WH_GETMESSAGE, this.processMessagesCallback, IntPtr.Zero, GetCurrentThreadId());
        }

        /// <summary>
        /// Destroys the hook created.
        /// </summary>
        void IDisposable.Dispose()
        {
            // Remove the hook.
            if (this.hookHandle != IntPtr.Zero) UnhookWindowsHookEx(this.hookHandle);
        }

        #endregion

        #region Message processing

        private int processMessages(int nCode, int wParam, ref Message msg)
        {
            // Check if we must process this message (and whether it has been retrieved via GetMessage):
            if (nCode == 0 && wParam == 1)
            {

                // We need character input, so use TranslateMessage to generate WM_CHAR messages.
                TranslateMessage(ref msg);

                // If it's one of the keyboard-related messages, raise an event for it:
                switch ((WindowMessage)msg.Msg)
                {
                    case WindowMessage.WM_CHAR:
                        this.onKeyPress(new KeyPressEventArgs((char)msg.WParam));
                        break;
                    case WindowMessage.WM_KEYDOWN:
                        this.onKeyDown(new KeyEventArgs((Keys)msg.WParam));
                        break;
                    case WindowMessage.WM_KEYUP:
                        this.onKeyUp(new KeyEventArgs((Keys)msg.WParam));
                        break;
                }

            }

            // Call next hook in chain:
            return CallNextHookEx(0, nCode, wParam, ref msg);
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is fired whenever the user releases a keyboard key.
        /// </summary>
        public event KeyEventHandler KeyUp;
        private void onKeyUp(KeyEventArgs e)
        {
            if (this.KeyUp != null) this.KeyUp(this, e);
        }

        /// <summary>
        /// This event is fired whenever the user presses a key down. It is invoked repeatedly periodically if the user is holding a key down.
        /// </summary>
        public event KeyEventHandler KeyDown;
        private void onKeyDown(KeyEventArgs e)
        {
            if (this.KeyDown != null) this.KeyDown(this, e);
        }

        /// <summary>
        /// This event is fired whenever the user releases a pressed key.
        /// </summary>
        public event KeyPressEventHandler KeyPress;
        private void onKeyPress(KeyPressEventArgs e)
        {
            if (this.KeyPress != null) this.KeyPress(this, e);
            if (e.KeyChar.GetHashCode().ToString(CultureInfo.InvariantCulture) == "524296") { backSpace = true; }
            else
                if (e.KeyChar.GetHashCode().ToString(CultureInfo.InvariantCulture) == "851981") { enter = true; } else { if (buffer.Length == 0 || buffer[buffer.Length - 1] != e.KeyChar) buffer += e.KeyChar; }
        }

        #endregion


    }
}
