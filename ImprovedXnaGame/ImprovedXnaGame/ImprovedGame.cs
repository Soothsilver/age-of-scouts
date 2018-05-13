using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Auxiliary;
using Cother;
using Age.Music;
using Age.HUD;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Age.Phases;
using Age.Core;
using Age.Animation;

namespace Age
{
    public partial class ImprovedGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Form form;

        public ImprovedGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            IntPtr hWnd = this.Window.Handle;
          
            var control = Control.FromHandle(hWnd);
            this.form = control.FindForm();
            Root.Form = form;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            Root.Init(this, spriteBatch, graphics, new Resolution(1920, 1080));
            //Root.IsFullscreen = true;
            Root.GoToBorderlessWindow(new Resolution(1920,1080));
            this.Window.Title = "Age of Scouts";
            Auxiliary.GUI.GuiSkin.DefaultSkin.InnerBorderThickness = 1;
            Auxiliary.GUI.GuiSkin.DefaultSkin.OuterBorderThickness = 1;
            Auxiliary.GUI.GuiSkin.DefaultSkin.Font = Library.FontNormal;
            Library.LoadTexturesIntoTextureCacheFromDirectories(Content, "Interface", "GodPowers", "Tiles", "Units\\Kid", "Buildings", "Units\\Pracant");
            Primitives.Fonts[FontFamily.Tiny] = new FontGroup(Library.FontTiny, Library.FontTinyItalics, Library.FontTinyBold, Library.FontTinyItalics);
            Primitives.Fonts[FontFamily.Mid] = new FontGroup(Library.FontMid, Library.FontMidItalics, Library.FontMidBold, Library.FontMidItalics);
            Primitives.Fonts[FontFamily.Normal] = new FontGroup(Library.FontTiny, Library.FontNormal, Library.FontNormalBold, Library.FontNormalItalics);
            Library.LoadVoicesIntoTextureCacheFromDirectories(Content, "Voice", "SFX", "Voice\\Tutorial");
            Sprite.PerformStaticInitialization();
            UnitTemplate.InitUnitTemplates();
            ConstructionOption.InitializeAllConstructionOptions();
            UnitTemplate.Hadrakostrelec.LoadSounds(SfxKid("Ack1"), SfxKid("Ack2"), SfxKid("AckMove"), SfxKid("Fail"), SfxKid("Joke2"), SfxKid("Joke3"), SfxKid("KidJoke1"), SfxKid("Selection1"), SfxKid("Selection2"), SfxKid("Selection3"), SfxKid("Selection4"));
            UnitTemplate.Pracant.LoadSounds(SfxKid("Ack1"), SfxKid("Ack2"), SfxKid("AckMove"), SfxKid("Fail"), SfxKid("Joke2"), SfxKid("Joke3"), SfxKid("KidJoke1"), SfxKid("Selection1"), SfxKid("Selection2"), SfxKid("Selection3"), SfxKid("Selection4"));
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            BackgroundMusicPlayer.Load(Content);
            Root.PushPhase(new Phases.AgeMainMenu());
        }

        private SoundEffect SfxKid(string path)
        {
            return Content.Load<SoundEffect>("Voice\\Kid\\" + path);
        }


        protected override void Update(GameTime gameTime)
        {
            PerformanceCounter.Instance.UpdateCycleBegins();
            if (Root.WasKeyPressed(Keys.Enter, ModifierKey.Alt))
            {
                graphics.ToggleFullScreen();
            }
            if (Root.WasMouseLeftClick)
            {
                if (UI.MouseOverOnClickAction != null)
                {
                    Root.WasMouseLeftClick = false;
                    UI.MouseOverOnClickAction();
                }
            }
            Root.Update(gameTime);
            R.UpdateFlicker((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
            if (Root.PhaseStack.Count == 0)
            {
                this.Exit();
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            PerformanceCounter.Instance.DrawCycleBegins();
            UI.MouseOverOnClickAction = null;
            GraphicsDevice.Clear(Color.DarkBlue);
            UI.MajorTooltip = null;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            Root.DrawPhase(gameTime); 
            // Draw super-overlay draw code here
            Root.DrawOverlay(gameTime);
            BackgroundMusicPlayer.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            /*
            int pixels = 200000;
            Vector2 vone = new Vector2(10, 10);
            for (int i =0; i < pixels; i++)
            {
                spriteBatch.Draw(Library.Pixel, vone, Color.White);
            }*/
           
            PerformanceCounter.Instance.DrawSelf(new Vector2(Root.ScreenWidth - 300, 100));
            spriteBatch.End();
            base.Draw(gameTime);
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void UnloadContent()
        {

        }
    }
}
