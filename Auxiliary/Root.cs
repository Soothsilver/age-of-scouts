/***
 * INSTRUCTIONS:
 * Always run Root.Init(); at the beginning of your game when using the Auxiliary project.
 * To use all functions, add Root.DrawOverlay() and Root.Update() to the end of your Draw() and Update() functions.
 * Then put Root.DrawPhase() wherever you want the bulk of your graphical data to be drawn.
 * 
 */
using System;
using System.Windows.Forms;
using Auxiliary.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Auxiliary
{
    /// <summary>
    /// This static class contains much information used by various components of the Auxiliary engine. 
    /// In addition, it contains many public methods such as Init() or DrawOverlay().
    /// </summary>
    public static partial class Root
    {
        // Properties starting with "in" should not be used by your project
        private static Game inGame;
        private static SpriteBatch inSpriteBatch;
        internal static GraphicsDeviceManager inGraphics;
        internal static KeyboardInput KeyboardInput;

        /// <summary>
        /// Some basic fonts and textures
        /// </summary>
        public static Library Library;
        /// <summary>
        /// The topmost GamePhase can be interacted with. All phases on the stack are drawn (beneath).
        /// </summary>
        public static ImprovedStack<GamePhase> PhaseStack = new ImprovedStack<GamePhase>();

        /// <summary>
        /// Gets the game phase at the top of the stack or pushes a new game phase to the top of the stack.
        /// </summary>
        public static GamePhase CurrentPhase
        {
            get
            {
                if (PhaseStack.Count > 0) return PhaseStack.Peek(); else return null;
            }
            set
            {
                PhaseStack.Push(value);
            }
        }

        /// <summary>
        /// Keyboard state in the previous Update() cycle.
        /// </summary>
        public static KeyboardState Keyboard_OldState = Keyboard.GetState();
        /// <summary>
        /// Keyboard state in the current Update() cycle.
        /// </summary>
        public static KeyboardState Keyboard_NewState = Keyboard.GetState();
        /// <summary>
        /// Mouse state in the previous Update() cycle.
        /// </summary>
        public static MouseState Mouse_OldState = Mouse.GetState();
        /// <summary>
        /// Mouse state in the current Update() cycle.
        /// </summary>
        public static MouseState Mouse_NewState = Mouse.GetState();

        /// <summary>
        /// Will not load the big circle texture necessary for drawing circles.
        /// </summary>
        public static bool Execute_DoNotLoadXxlTextures = false;
        /// <summary>
        /// Displays the FPS counter as an overlay.
        /// </summary>
        public static bool Display_DisplayFpsCounter = false;
        /// <summary>
        /// The position, in pixels, of the FPS counter overlay.
        /// </summary>
        public static Vector2 Display_DisplayFpsCounterWhere = new Vector2(5, 5);

        /// <summary>
        /// Gets the current width of the game window.
        /// </summary>
        public static int ScreenWidth
        {
            get { return Root.inGraphics.PreferredBackBufferWidth; }
        }
        /// <summary>
        /// Gets the current height of the game window.
        /// </summary>
        public static int ScreenHeight
        {
            get { return Root.inGraphics.PreferredBackBufferHeight; }
        }
        public static GraphicsDevice GraphicsDevice
        {
            get
            {
                return inGraphics.GraphicsDevice;
            }
        }
        /// <summary>
        /// Gets the rectangle representing the width and height of the game window.
        /// </summary>
        public static Rectangle Screen
        {
            get { return new Rectangle(0, 0, ScreenWidth, ScreenHeight); }
        }

        /// <summary>
        /// Gets the phase that's just under the GAME PHASE in the phase stack. If no such phase exists, returns null.
        /// </summary>
        public static GamePhase GetPhaseBeneath(GamePhase gamePhase)
        {
            GamePhase previous = null;
            foreach(GamePhase nowPhase in PhaseStack)
            {
                if (nowPhase == gamePhase)
                {
                    return previous;
                }
                previous = nowPhase;
            }
            return null;
        }

        /// <summary>
        /// Adds new game on top of the stack and initializes it.
        /// </summary>
        /// <param name="phase">The phase to put on stack.</param>
        public static void PushPhase(GamePhase phase)
        {
            CurrentPhase = phase;
            phase.Initialize(inGame);
        }

        /// <summary>
        /// Calls the "Destruct" method of the phase, which should, by default, set the ScheduledForElimination flag.
        /// The phase will be popped from stack only later, not immediately.
        /// </summary>
        public static void PopFromPhase()
        {
            GamePhase gp = PhaseStack.Peek();
            if (gp != null) gp.Destruct(inGame);
        }

        /// <summary>
        /// Returns true if the mouse cursor is currently inside the specified rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to check for mouse cursor's position</param>
        public static bool IsMouseOver(Rectangle rectangle)
        {
            return Mouse_NewState.X >= rectangle.X &&
                Mouse_NewState.Y >= rectangle.Y    && 
                Mouse_NewState.X < rectangle.Right && 
                Mouse_NewState.Y < rectangle.Bottom;
        }

        /// <summary>
        /// Sets and applies the new resolution immediately.
        /// </summary>
        /// <param name="width">VideoWidth of the resolution.</param>
        /// <param name="height">Height of the resolution.</param>
        public static void SetResolution(int width, int height)
        {
            inGraphics.PreferredBackBufferWidth = width;
            inGraphics.PreferredBackBufferHeight = height;
            inGraphics.ApplyChanges();
        }
        /// <summary>
        /// Sets and applies the new resolution immediately.
        /// </summary>
        /// <param name="r">Resolution to apply.</param>
        public static void SetResolution(Resolution r)
        {
            inGraphics.PreferredBackBufferWidth = r.Width;
            inGraphics.PreferredBackBufferHeight = r.Height;
            inGraphics.ApplyChanges();
        }
        /// <summary>
        /// Gets or sets whether the game is in fullscreen mode.
        /// </summary>
        public static bool IsFullscreen
        {
            get { return inGraphics.IsFullScreen; }
            set { inGraphics.IsFullScreen = value; inGraphics.ApplyChanges(); }
        }
        /// <summary>
        /// Draws all phases on the stack using the Root spritebatch, in stack order.
        /// </summary>
        /// <param name="gameTime">gameTime parameter from the Game.Draw() method.</param>
        public static void DrawPhase(GameTime gameTime)
        {
            int max = PhaseStack.Count;
            for (int gpi = 0; gpi < max; gpi++)
            {
                GamePhase gp = PhaseStack[gpi];
                gp.Draw(Root.inSpriteBatch, Root.inGame, (float)gameTime.ElapsedGameTime.TotalSeconds, gp == PhaseStack.Peek());
            }
        }
        /// <summary>
        /// Draws toasts and FPS counter, if required, using the Root spritebatch.
        /// </summary>
        /// <param name="gameTime">gameTime parameter from the Game.Draw() method.</param>
        public static void DrawOverlay(GameTime gameTime)
        {
            DrawToasts();
        }
        public static bool Holding
        {
            get
            {
                return Mouse_NewState.LeftButton == ButtonState.Pressed;
            }
        }
        public static void GoToBorderlessWindow(Resolution resolution)
        {
            Form.FormBorderStyle = FormBorderStyle.None;
            Form.Width = resolution.Width;
            Form.Height = resolution.Height;
            Form.WindowState = FormWindowState.Maximized;
            Form.Location = new System.Drawing.Point(0, 0);
            Root.IsFullscreen = false;
        }
        public static void GoToFullscreen(Resolution resolution)
        {
            Root.SetResolution(resolution);
            Root.IsFullscreen = true;
        }
        public static void GoToNormalWindow(Resolution resolution)
        {
            Root.IsFullscreen = false;
            Form.FormBorderStyle = FormBorderStyle.Fixed3D;
            Form.Width = resolution.Width;
            Form.Height = resolution.Height;
            Form.WindowState = FormWindowState.Normal;
        }


        public static int ClickedMouseMovement;
        /// <summary>
        /// Call this from Game.Update(). Updates keyboard and mouse states, updates all phases, then erases phases scheduled for elimination, and updates toasts and FPS counter.
        /// </summary>
        /// <param name="gameTime">gameTime parameter from the Game.Update() method.</param>
        public static void Update(GameTime gameTime)
        {
            Keyboard_OldState = Keyboard_NewState;
            Mouse_OldState = Mouse_NewState;
            Keyboard_NewState = Keyboard.GetState();
            Mouse_NewState = Mouse.GetState();

            if (Mouse_NewState.LeftButton == ButtonState.Pressed && Mouse_OldState.LeftButton == ButtonState.Released)
            {
                ClickedMouseMovement = 0;
            }
            if (Holding)
            {
                ClickedMouseMovement += Math.Abs(Mouse_NewState.X - Mouse_OldState.X) + Math.Abs(Mouse_NewState.Y - Mouse_OldState.Y);
            }
            WasMouseLeftClick = Mouse_NewState.LeftButton == ButtonState.Released && Mouse_OldState.LeftButton == ButtonState.Pressed;
            WasMouseMiddleClick = Mouse_NewState.MiddleButton == ButtonState.Released && Mouse_OldState.MiddleButton == ButtonState.Pressed;
            WasMouseRightClick = Mouse_NewState.RightButton == ButtonState.Released && Mouse_OldState.RightButton == ButtonState.Pressed;

        
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Root.PhaseStack.Count > 0)
                Root.PhaseStack.Peek().Update(Root.inGame, (float)gameTime.ElapsedGameTime.TotalSeconds);

            SecondsSinceStart += elapsedSeconds;
            SecondsSinceStartInt = (int)SecondsSinceStart;

            for (int i = Root.PhaseStack.Count - 1; i >= 0; i--)
            {
                GamePhase ph = Root.PhaseStack[i];
                if (ph.ScheduledForElimination)
                    Root.PhaseStack.RemoveAt(i);
            }

            Root.UpdateToasts(elapsedSeconds);
        }
        /// <summary>
        /// Returns true only if a key was just pressed down and released.
        /// </summary>
        /// <param name="key">We test whether this key was pressed and released</param>
        /// <param name="modifiersPressed">This combination of keys must have been pressed at the time of release</param>
        /// <returns></returns>
        public static bool WasKeyPressed(Keys key, params ModifierKey[] modifiersPressed)
        {
            if (Keyboard_NewState.IsKeyDown(key) || Keyboard_OldState.IsKeyUp(key)) return false;
           
                foreach(ModifierKey mk in modifiersPressed)
                {
                    Keys mkKey = Keys.A;
                    Keys mkKey2 = Keys.B;
                    if (mk == ModifierKey.Alt) { mkKey = Keys.LeftAlt; mkKey2 = Keys.RightAlt; }
                    if (mk == ModifierKey.Ctrl) { mkKey = Keys.LeftControl; mkKey2 = Keys.RightControl; }
                    if (mk == ModifierKey.Shift) { mkKey = Keys.LeftShift; mkKey2 = Keys.RightShift; }
                    if (mk == ModifierKey.Windows) { mkKey = Keys.LeftWindows; mkKey2 = Keys.RightWindows; }
                    if (Keyboard_OldState.IsKeyUp(mkKey) && Keyboard_NewState.IsKeyUp(mkKey) &&
                    Keyboard_OldState.IsKeyUp(mkKey2) && Keyboard_NewState.IsKeyUp(mkKey2)
                    ) return false;
                }
            
            return true;
        }
        /// <summary>
        /// Gets or sets. This is set to true or false depending on whether a left mouse click occured since last calling Root.Update().
        /// </summary>
        public static bool WasMouseLeftClick { get; set; }
        /// <summary>
        /// Gets or sets. This is set to true or false depending on whether a middle mouse click occured since last calling Root.Update().
        /// </summary>
        public static bool WasMouseMiddleClick { get; set; }
        /// <summary>
        /// Gets or sets. This is set to true or false depending on whether a right mouse click occured since last calling Root.Update().
        /// </summary>
        public static bool WasMouseRightClick { get; set; }
        /// <summary>
        /// Sets WasMouseLeftClick to false.
        /// </summary>
        public static void ConsumeLeftClick() { WasMouseLeftClick = false; }
        /// <summary>
        /// Sets WasMouseMiddleClick to false.
        /// </summary>
        public static void ConsumeMiddleClick() { WasMouseMiddleClick = false; }
        /// <summary>
        /// Sets WasMouseRightClick to false.
        /// </summary>
        public static void ConsumeRightClick() { WasMouseRightClick = false; }

        public static bool SynchronizeWithVerticalRetrace
        {
            get { return inGame.IsFixedTimeStep; }
            set
            {
                inGame.IsFixedTimeStep = value;
                inGraphics.SynchronizeWithVerticalRetrace = value;
                inGraphics.ApplyChanges();
            }
        }


        /// <summary>
        /// Binds this Root class to the graphics device from the game, and also sets the resolution and fullscreen/windowed mode. You must call Root.Init() before using any other Auxiliary functions.
        /// </summary>
        /// <param name="game">The primary Game class containg the SpriteBatch and GraphicsDeviceManager.</param>
        /// <param name="spriteBatch">The spriteBatch Auxiliary will use to draw. Auxiliary will assume all calls to its method (that could potentially draw something) are inside spriteBatch.Begin() and spriteBatch.End() calls.</param>
        /// <param name="graphics">The primary GraphicsDeviceManager from the Game class.</param>
        /// <param name="defaultResolution">The resolution to set the game to, or null to leave unchanged.</param>
        /// <param name="fullscreenMode">Whether to set the fullscreen mode or windowed mode.</param>
        public static void Init(Game game,
            SpriteBatch spriteBatch, 
            GraphicsDeviceManager graphics, 
            Resolution defaultResolution = null, 
            bool fullscreenMode = false)
        {
            inGame = game;
            inSpriteBatch = spriteBatch;
            inGraphics = graphics;

            // Load basic textures
            Library = new Auxiliary.Library(game);
            Library.LoadBaseTextures(Execute_DoNotLoadXxlTextures);
            // Load graphical functions
            Auxiliary.Primitives.Init(spriteBatch, game.GraphicsDevice);
            // Load FPS Counter
            // Load keyboard input
            Root.KeyboardInput = new KeyboardInput();
            // Set resolution
            if (defaultResolution != null)
                SetResolution(defaultResolution);
            Root.IsFullscreen = fullscreenMode;
        }

    }
    /// <summary>
    /// A meta-key pressed alongside another key.
    /// </summary>
    public enum ModifierKey
    {
        /// <summary>
        /// Any Control key.
        /// </summary>
        Ctrl,
        /// <summary>
        /// Any Shift key.
        /// </summary>
        Shift,
        /// <summary>
        /// Any Alt key.
        /// </summary>
        Alt,
        /// <summary>
        /// Any Windows key.
        /// </summary>
        Windows
    }
    /// <summary>
    /// Represents a display resolution.
    /// </summary>
    [Serializable]
    public class Resolution : IComparable<Resolution>
    {
        /// <summary>
        /// Gets or sets the resolution height.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Gets or sets the resolution height.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Creates a new resolution class instance.
        /// </summary>
        /// <param name="width">Resolution width.</param>
        /// <param name="height">Resolution height.</param>
        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// A resolution is less than another resolution if its width is less, or if its width is identical and its height is less.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Resolution other)
        {
            if (this.Width < other.Width)
            {
                return -1;
            }
            else if (this.Width == other.Width)
            {
                if (this.Height < other.Height)
                {
                    return -1;
                }
                else if (this.Height == other.Height)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }
        

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj is Resolution r)
            {
                return this.Width == r.Width && this.Height == r.Height;
            }
            else return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Width * 10000 + Height;
        }

        /// <summary>
        /// Returns the string "(width)x(height)". For example, 1024x768.
        /// </summary>
        public override string ToString()
        {
            return Width + "x" + Height;
        }
    }
}
