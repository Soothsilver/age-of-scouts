using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Auxiliary
{
    /// <summary>
    /// An example GamePhase that displays a video above the game.
    /// </summary>
    public class FullscreenVideoGamePhase : GamePhase
    {
        /// <summary>
        /// The used ImprovedVideoPlayer.
        /// </summary>
        public ImprovedVideoPlayer Player;
        private Rectangle rectVideo;
        

        /// <summary>
        /// Create a new  game phase that displays video over other game phases.
        /// </summary>
        /// <param name="ivp">The video player to use.</param>
        public FullscreenVideoGamePhase(ImprovedVideoPlayer ivp)
        {
            Rectangle screen = Root.Screen;
            Player = ivp;
            rectVideo = new Rectangle(screen.Width / 2 - Player.VideoWidth / 2, screen.Height / 2 - Player.VideoHeight / 2, Player.VideoWidth, Player.VideoHeight);
            rectVideo = Utilities.ScaleRectangle(new Rectangle(screen.X + 3, screen.Y + 3, screen.Width - 6, screen.Height -6), rectVideo.Width, rectVideo.Height, false);
        }
        /// <summary>
        /// Updates the full-screen video phase.
        /// </summary>
        protected internal override void Update(Game game, float elapsedSeconds)
        {
            if (Root.WasMouseLeftClick)
            {
                if (!Root.IsMouseOver(rectVideo))
                {
                    Root.ConsumeLeftClick();
                    EndFullscreen();
                }
            }
            if (Root.WasMouseRightClick)
            {
                Root.ConsumeRightClick();
                EndFullscreen();
            }
            Player.Update(true, Root.IsMouseOver(rectVideo));
            base.Update(game, elapsedSeconds);
        }

        private void EndFullscreen()
        {
            Root.PopFromPhase();
        }

        /// <summary>
        /// Draws the fullscreen video phase.
        /// </summary>
        protected internal override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Rectangle screen = Root.Screen;
            Primitives.FillRectangle(screen, Color.FromNonPremultiplied(0, 0, 0, 150));
            Player.Draw(sb, rectVideo, alreadyFullscreen: true);
            Primitives.DrawRectangle(new Rectangle(rectVideo.X - 2, rectVideo.Y - 2, rectVideo.Width + 4, rectVideo.Height + 4), Color.White, 2);

            base.Draw(sb, game, elapsedSeconds, topmost);
        }
    }
}
