using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Age.Music
{
    class BackgroundMusicPlayer
    {
        public static MusicTrack MenuMusic;
        public static MusicTrack LevelMusic;
        private static FlagStatus Status;
        private static float FlagRaisedPercentage = 0;
        private static float RaisingSpeedPerSecond = 1;
        private static float TimeUntilFlagGoesDown = 0;
        private static MusicTrack PlayingWhat;

        internal static void Load(ContentManager content)
        {
            MenuMusic = new MusicTrack("Junácká hymna", content.Load<Song>("Music\\JunackaHymna"));
            LevelMusic = new MusicTrack("Podzimní den", content.Load<Song>("Music\\AutumnDay"));
        }

        public static void Play(MusicTrack track)
        {
            PlayingWhat = track;
            MediaPlayer.Play(track.Song);
            MediaPlayer.IsRepeating = true;
            Status = FlagStatus.Raising;
            FlagRaisedPercentage = 0;
        }

        public static void Draw(float elapsedSeconds)
        {
            switch(Status)
            {
                case FlagStatus.Raising:
                    FlagRaisedPercentage += elapsedSeconds * RaisingSpeedPerSecond;
                    if (FlagRaisedPercentage >= 1)
                    {
                        FlagRaisedPercentage = 1;
                        TimeUntilFlagGoesDown = 5;
                        Status = FlagStatus.Up;
                    }
                    break;
                case FlagStatus.Falling:
                    FlagRaisedPercentage -= elapsedSeconds * RaisingSpeedPerSecond;
                    if (FlagRaisedPercentage <= 0)
                    {
                        FlagRaisedPercentage = 0;
                        Status = FlagStatus.Down;
                    }
                    break;
                case FlagStatus.Up:
                    TimeUntilFlagGoesDown -= elapsedSeconds;
                    if (TimeUntilFlagGoesDown <= 0)
                    {
                        Status = FlagStatus.Falling;
                    }
                    break;
            }

            if (FlagRaisedPercentage > 0)
            {
                int width = 400;
                Rectangle rect = new Microsoft.Xna.Framework.Rectangle(
                    -50 - width + (int) (width * FlagRaisedPercentage), Root.ScreenHeight - 200, width, 80);
                Primitives.DrawAndFillRoundedRectangle(rect, Color.Pink, Color.DeepPink, 2);
                Primitives.DrawSingleLineText("Nyní přehrávám", new Vector2(rect.X + 70, rect.Y + 5), Color.Black,
                    Library.FontNormal);
                Primitives.DrawSingleLineText(PlayingWhat.Name, new Vector2(rect.X + 65, rect.Y + 35), Color.Black,
                    Library.FontNormalBold);
                Primitives.DrawImage(Library.Get(TextureName.MusicIcon),
                    new Rectangle(rect.Right - 80, rect.Y + 8, 64, 64));
            }
        }

        private enum FlagStatus
        {
            Down,
            Raising,
            Up,
            Falling
        }
    }
}
