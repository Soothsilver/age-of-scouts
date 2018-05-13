using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Cother;
using Microsoft.Xna.Framework.Audio;

namespace Auxiliary
{
    /// <summary>
    /// Provides an extension method that allows you to load multiple assets at the same time.
    /// </summary>
    public static class LoadContentExtensionMethod
    {
        /// <summary>
        /// Loads all files from the specified folder as the given type.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        /// <param name="contentFolder">Folder name from which to load assets.</param>
        /// <typeparam name="T">Type of asset. All assets in the folder must be of this type.</typeparam>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException">This folder does not exist.</exception>
        /// <remarks>
        /// This code comes from Stack Overflow, created by Neil Knights (http://stackoverflow.com/users/410636/neil-knight). The post's page is http://stackoverflow.com/questions/4052532/xna-get-an-array-list-of-resources.
        /// </remarks>
        public static Dictionary<string, T> LoadContent<T>(this ContentManager contentManager, string contentFolder, bool lowercase = false)
        {
            var dir = new DirectoryInfo(contentManager.RootDirectory
                                        + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            var result = new Dictionary<string, T>();

            foreach (FileInfo file in dir.GetFiles())
            {
                string key = Path.GetFileNameWithoutExtension(file.Name) ?? file.Name;
                if (lowercase)
                {
                    key = key.ToLower();
                }
                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }

            return result;
        }

    }

    /// <summary>
    /// Contains some textures and font. 
    /// This is a DrawableGameComponent because it needs to load content from an alternate content library.
    /// </summary>
    public class Library: DrawableGameComponent
    {
        private static ContentManager libContent;

        /// <summary>
        /// For the given icon description, returns an appropriate texture.
        /// </summary>
        /// <param name="icon"></param>
        public static Texture2D GetTexture2DFromGuiIcon(GuiIcon icon)
        {
            switch (icon)
            {
                case GuiIcon.Error: return Library.IconError;
                case GuiIcon.Information: return Library.IconInformation;
                case GuiIcon.Question: return Library.IconQuestion;
                case GuiIcon.Warning: return Library.IconWarning;
            }
            return null;
        }
        /// <summary>
        /// A 1x1 white square.
        /// </summary>
        public static Texture2D Pixel;
        /// <summary>
        /// A 1000x1000 full circle texture.
        /// </summary>
        public static Texture2D Circle1000X1000;
        /// <summary>
        /// A 1000x1000 outline of a circle.
        /// </summary>
        public static Texture2D EmptyCircle1000X1000;
        /// <summary>
        /// An information bubble icon.
        /// </summary>
        public static Texture2D IconInformation;
        /// <summary>
        /// A warning triangle icon.
        /// </summary>
        public static Texture2D IconWarning;
        /// <summary>
        /// An error bubble icon.
        /// </summary>
        public static Texture2D IconError;
        /// <summary>
        /// A question mark bubble icon.
        /// </summary>
        public static Texture2D IconQuestion;
        /// <summary>
        /// A "Play" media player button.
        /// </summary>
        public static Texture2D IconPlay;
        /// <summary>
        /// A "Pause" media player button.
        /// </summary>
        public static Texture2D IconPause;
        /// <summary>
        /// A "Stop" media player button.
        /// </summary>
        public static Texture2D IconStop;
        /// <summary>
        /// A "Switch to Fullscreen" media player button.
        /// </summary>
        public static Texture2D IconFullscreen;
        /// <summary>
        /// A 14px Courier New font.
        /// </summary>
        public static SpriteFont FontCourierNew;
        /// <summary>
        /// A 14px Verdana font.
        /// </summary>
        public static SpriteFont FontVerdana;
        
        public static SpriteFont FontBig;

        public static SpriteFont FontTiny;
        public static SpriteFont FontTinyBold;
        public static SpriteFont FontTinyItalics;

        public static SpriteFont FontNormal;
        public static SpriteFont FontNormalBold;
        public static SpriteFont FontNormalItalics;

        public static SpriteFont FontMid;
        public static SpriteFont FontMidBold;
        public static SpriteFont FontMidItalics;

        public static SpriteFont FontBigBold;

        internal Library(Game game)
            : base(game)
        {
            libContent = new ContentManager(game.Services)
                             {
                                 RootDirectory = "AuxiliaryContent"
                             };
        }
        public static Dictionary<TextureName, Texture2D> TextureCache = new Dictionary<TextureName, Texture2D>();
        public static Dictionary<SoundEffectName, SoundEffect> SoundEffectCache = new Dictionary<SoundEffectName, SoundEffect>();
        public static Dictionary<string, Texture2D> Icons = new Dictionary<string, Texture2D>();

        public static void LoadVoicesIntoTextureCacheFromDirectories(ContentManager content, params string[] directoryNames)
        {
            foreach (string dir in directoryNames)
            {
                var loaded = content.LoadContent<SoundEffect>(dir, true);
                foreach (SoundEffectName tname in Enumeration.GetValues<SoundEffectName>())
                {
                    string tnames = tname.ToString().ToLower();
                    if (loaded.ContainsKey(tnames))
                    {
                        if (SoundEffectCache.ContainsKey(tname))
                        {
                            throw new Exception("Duplicate texture loaded.");
                        }
                        else
                        {
                            SoundEffectCache.Add(tname, loaded[tnames]);
                        }
                    }
                }
            }
        }
        public static void LoadTexturesIntoTextureCacheFromDirectories(ContentManager content, params string[] directoryNames)
        {
            foreach (string dir in directoryNames)
            {
                var loaded = content.LoadContent<Texture2D>(dir, true);
                foreach(TextureName tname in Enumeration.GetValues<TextureName>()) {
                    string tnames = tname.ToString().ToLower();
                    if (loaded.ContainsKey(tnames))
                    {
                        if (TextureCache.ContainsKey(tname))
                        {
                            throw new Exception("Duplicate texture loaded.");
                        }
                        else
                        {
                            TextureCache.Add(tname, loaded[tnames]);
                        }
                    }
                }
            }
        }


        internal static void LoadBaseTextures(bool doNotLoadXxlTextures = false)
        {
            Pixel = libContent.Load<Texture2D>("pixel");
            FontCourierNew = libContent.Load<SpriteFont>("fontCourierNew");
            FontVerdana = libContent.Load<SpriteFont>("fontVerdana");

            FontTiny = libContent.Load<SpriteFont>("fontTiny");
            FontTinyBold = libContent.Load<SpriteFont>("fontTinyBold");
            FontTinyItalics = libContent.Load<SpriteFont>("fontTinyItalics");

            FontNormal = libContent.Load<SpriteFont>("fontNormal");
            FontNormalBold = libContent.Load<SpriteFont>("fontNormalBold");
            FontNormalItalics = libContent.Load<SpriteFont>("fontNormalItalics");

            FontMid = libContent.Load<SpriteFont>("fontMid");
            FontMidBold = libContent.Load<SpriteFont>("fontMidBold");
            FontMidItalics = libContent.Load<SpriteFont>("fontMid");

            FontBig = libContent.Load<SpriteFont>("fontBig");
            FontBigBold = libContent.Load<SpriteFont>("fontBigBold");

            IconPlay = libContent.Load<Texture2D>("Icons\\play");
            IconPause = libContent.Load<Texture2D>("Icons\\pause");
            IconInformation = libContent.Load<Texture2D>("Icons\\information");
            IconError = libContent.Load<Texture2D>("Icons\\error");
            IconQuestion = libContent.Load<Texture2D>("Icons\\question");
            IconWarning = libContent.Load<Texture2D>("Icons\\warning");
            IconStop = libContent.Load<Texture2D>("Icons\\stop");
            IconFullscreen = libContent.Load<Texture2D>("Icons\\fullscreen");

            if (doNotLoadXxlTextures)
            {
                Circle1000X1000 = libContent.Load<Texture2D>("pixel");
                EmptyCircle1000X1000 = libContent.Load<Texture2D>("pixel");
            }
            else
            {
                Circle1000X1000 = libContent.Load<Texture2D>("circle1000x1000");
                EmptyCircle1000X1000 = libContent.Load<Texture2D>("emptyCircle1000x1000");
            }
        }

        public static Texture2D Get(TextureName textureName)
        {
            return TextureCache[textureName];
        }
        public static SoundEffect Get(SoundEffectName sfxName)
        {
            return SoundEffectCache[sfxName];
        }
    }

    public enum SoundEffectName
    {
        MiseBylaUspesneSplnena,
        QuestSound,
        Fanfare,
        ItIsOver,
        MiseNebylaSplnena,
        ItIsDone,
        Harp,
        Tut1,
        Tut2,
        Tut3,
        Tut4,
        Tut5,
        Tut6,
        Tut7,
        Tut8,
        Tut9,
        Tut10,
        Tut11,
        Tut12
    }

    public enum TextureName
    {
        Water,
        Electricity,
        WaterCard,
        Draw,
        Crab,
        Fish,
        WaterPoint,
        Dew,
        ColorlessPoint,
        MudSplat,
        AgeOfScoutsLogo,
        ButtonBackground,
        ButtonHoverBackground,
        LogoRight,
        LogoLeft,
        MusicIcon,
        GodPowerBackground,
        PopulationIcon,
        MudIcon,
        WoodIcon,
        MeatIcon,
        PruzkumGodPower,
        GodPowerBorder,
        IsoGrass2,
        IsoWater,
        TallGrass,
        BasicTree,
        BasicFlag,
        BigSimpleTent,
        None,
        Starfield,
        KidIdle,
        KidIdle2,
        HadrakometLogo,
        FogOfWar,
        WhiteTile,
        Victory,
        Defeat,
        KidMoveRight,
        KidMoveDown3,
        KidMoveDown,
        KidMoveDown2,
        KidMoveRight2,
        KidMoveUp,
        KidMoveUp2,
        KidMoveUp3,
        KidMoveLeft,
        KidMoveLeft2,
        IsoWater3,
        IsoWater2,
        IsoWater1,
        KidBroken,
        BerryBush,
        Corn,
        PracantLogo,
        GodPowerUsed,
        Kitchen,
        TentStandard,
        IdleVillager,
        ObjectivesRibbon,
        AggressiveStance,
        StandYourGroundStance,
        StealthyStance,
        Tile64x64,
        LaborerIdle,
        LaborerIdle2,
        KidThrowRight2,
        KidThrowLeft2,
        KidThrowLeft1,
        KidThrowRight1,
        PracantMoveRight,
        PracantMoveLeft,
        PracantMoveUp3,
        PracantMoveUp2,
        PracantMoveUp,
        PracantMoveRight2,
        PracantMoveLeft2,
        PracantMoveDown3,
        PracantMoveDown,
        PracantMoveDown2
    }
}
