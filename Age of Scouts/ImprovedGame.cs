using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Auxiliary;
using Age.Music;
using Age.HUD;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Age.Core;
using Age.Animation;
using Age.Internet;

namespace Age
{
    /// <summary>
    /// Main game class.
    /// </summary>
    public class ImprovedGame : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        CommandLineArguments commandLineArguments;


        public ImprovedGame(String[] args)
        {
            graphics = new GraphicsDeviceManager(this);
            commandLineArguments = new CommandLineArguments(args);
            Eqatec.Start(commandLineArguments.DoNotTrack);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            IntPtr hWnd = this.Window.Handle;          
            var control = Control.FromHandle(hWnd);
            Form form = control.FindForm();
            Root.Form = form;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Window.Title = "Age of Scouts";

            // Visuals
            Root.Init(this, spriteBatch, graphics, commandLineArguments.Resolution);
            switch (commandLineArguments.DisplayMode)
            {
                case DisplayModus.BorderlessWindow:
                    Root.GoToBorderlessWindow(commandLineArguments.Resolution);
                    break;
                case DisplayModus.Fullscreen:
                    Root.GoToFullscreen(commandLineArguments.Resolution);
                    break;
                case DisplayModus.Windowed:
                    Root.GoToNormalWindow(commandLineArguments.Resolution);
                    break;
            }
            Settings.Instance.DisplayMode = commandLineArguments.DisplayMode;
            Root.SynchronizeWithVerticalRetrace = Settings.Instance.VSync;

            // Load assets
            Library.LoadTexturesIntoTextureCacheFromDirectories(Content, "Interface", "GodPowers", "Tiles", "Units\\Kid", "Buildings", "Units\\Pracant");
            Primitives.Fonts[FontFamily.Tiny] = new FontGroup(Library.FontTiny, Library.FontTinyItalics, Library.FontTinyBold, Library.FontTinyItalics);
            Primitives.Fonts[FontFamily.Mid] = new FontGroup(Library.FontMid, Library.FontMidItalics, Library.FontMidBold, Library.FontMidItalics);
            Primitives.Fonts[FontFamily.Normal] = new FontGroup(Library.FontTiny, Library.FontNormal, Library.FontNormalBold, Library.FontNormalItalics);
            Library.LoadVoicesIntoTextureCacheFromDirectories(Content, "Voice", "SFX", "Voice\\Tutorial", "SFX\\Buildings");
            BackgroundMusicPlayer.Load(Content);
            
            // Initialize
            Sprite.PerformStaticInitialization();
            UnitTemplate.InitUnitTemplates();
            ConstructionOption.InitializeAllConstructionOptions();

            // Load unit sounds
            UnitTemplate.Hadrakostrelec.LoadSounds(SfxKid("Ack1"), SfxKid("Ack2"), SfxKid("AckMove"), SfxKid("Fail"), SfxKid("Joke2"), SfxKid("Joke3"), SfxKid("KidJoke1"), SfxKid("Selection1"), SfxKid("Selection2"), SfxKid("Selection3"), SfxKid("Selection4"), SfxKid("Naverbovan1"));
            UnitTemplate.Hadrakostrelec.LoadAttackSounds(SfxKid("AckAttack"), SfxKid("AckAttack2"));
            UnitTemplate.Pracant.LoadSounds(SfxPrc("Ack1"), SfxPrc("Ack2"), SfxPrc("AckMove"), SfxKid("Fail"), SfxPrc("Joke1"), SfxPrc("Joke2"), SfxPrc("Joke3"),  SfxPrc("Selection1"), SfxPrc("Selection2"), SfxPrc("Selection3"), SfxPrc("Selection4"), SfxPrc("Naverbovan"));
            UnitTemplate.Pracant.LoadPracantSounds(SfxPrc("AckBuild"), SfxPrc("Corn"), SfxPrc("Mud"), SfxPrc("Bobule"), SfxPrc("Wood"));

            // Go to main menu
            Root.PushPhase(new Phases.AgeMainMenu());
            Eqatec.ScheduleSendMessage("GAME START", "");
        }

        private SoundEffect SfxKid(string path)
        {
            return Content.Load<SoundEffect>("Voice\\Kid\\" + path);
        }
        private SoundEffect SfxPrc(string path)
        {
            return Content.Load<SoundEffect>("Voice\\Pracant\\" + path);
        }


        protected override void Update(GameTime gameTime)
        {
            PerformanceCounter.StartMeasurement(PerformanceGroup.UpdateCycle);
            PerformanceCounter.Instance.UpdateCycleBegins();
            if (Root.WasKeyPressed(Keys.Enter, ModifierKey.Alt))
            {
                if (!Root.IsFullscreen)
                {
                    Settings.Instance.DisplayMode = DisplayModus.Fullscreen;
                    Root.GoToFullscreen(Settings.Instance.Resolution);
                }
                else
                {
                    Settings.Instance.DisplayMode = DisplayModus.Windowed;
                    Root.GoToNormalWindow(Settings.Instance.Resolution);
                }
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
            PerformanceCounter.EndMeasurement(PerformanceGroup.UpdateCycle);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);
            PerformanceCounter.Instance.DrawCycleBegins();
            UI.MouseOverOnClickAction = null;
            UI.MajorTooltip = null;

            // Use pixel-perfect drawing:
            PerformanceCounter.StartMeasurement(PerformanceGroup.DrawCycle);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            Root.DrawPhase(gameTime); 

            BackgroundMusicPlayer.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            PerformanceCounter.EndMeasurement(PerformanceGroup.DrawCycle);
            PerformanceCounter.Instance.DrawSelf(new Vector2(Root.ScreenWidth - 300, 50));

            PerformanceCounter.StartMeasurement(PerformanceGroup.SpriteBatchEnd);
            spriteBatch.End();
            PerformanceCounter.EndMeasurement(PerformanceGroup.SpriteBatchEnd);

            base.Draw(gameTime);
        }
    }
}
