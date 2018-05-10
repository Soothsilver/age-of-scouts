using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Auxiliary
{
    /// <summary>
    /// Represents a video player control you can use.
    /// </summary>
    public class ImprovedVideoPlayer
    {
        private readonly VideoPlayer videoPlayer = new VideoPlayer();
        private readonly Texture2D stoppedTexture;
        private readonly bool hasPlayPauseButton;
        private readonly bool hasExtendToFullscreenButton;
        private readonly bool onClickPlayPause;
        private readonly bool onClickExtendToFullscreen;
        private const bool PerformScaling = true;
        private bool mouseOverPlayPauseButton;
        private bool mouseOverStopButton;
        private bool mouseOverFullscreenButton;
        private bool mouseOverThisElement;

        /// <summary>
        /// Updates the video player.
        /// </summary>
        /// <param name="alreadyFullscreen">This is true only if this displayed in the FullscreenVideoGamePhase.</param>
        /// <param name="mouseisoverthis">Is mouse over this control?</param>
        public void Update(bool alreadyFullscreen = false, bool mouseisoverthis = false)
        {
            if (Root.WasMouseLeftClick && (mouseOverThisElement || mouseisoverthis))
            {
                Root.ConsumeLeftClick();
                if (mouseOverPlayPauseButton)
                {
                    if (videoPlayer.State == MediaState.Playing) Pause();
                    else Play();
                }
                else if (mouseOverStopButton)
                {
                    Stop();
                }
                else if (mouseOverFullscreenButton)
                {
                    GoFullscreen();
                }
                else if (onClickExtendToFullscreen)
                {
                    if (!alreadyFullscreen)
                        GoFullscreen();
                    else
                        Root.PopFromPhase();
                }
                else if (onClickPlayPause)
                {
                    if (videoPlayer.State == MediaState.Playing) Pause();
                    else Play();
                }
            }
        }

        private void GoFullscreen()
        {
            FullscreenVideoGamePhase fvgp = new FullscreenVideoGamePhase(this);
            Root.PushPhase(fvgp);
        }

        /// <summary>
        /// Draws the video player.
        /// </summary>
        /// <param name="sb">Spritebatch to use.</param>
        /// <param name="rect">Rectangle to fill.</param>
        /// <param name="alreadyFullscreen">Is the video player part of the FullscreenVideoGamePhase?</param>
        public void Draw(SpriteBatch sb, Rectangle rect, bool alreadyFullscreen = false)
        {
            mouseOverFullscreenButton = false;
            mouseOverPlayPauseButton = false;
            mouseOverStopButton = false;
            mouseOverThisElement = false;
            if (State == MediaState.Playing || State == MediaState.Paused)
            {
                Texture2D tex = this.GetTexture();
                if (tex != null)
                {
                    Primitives.DrawImage(tex, rect, Color.White, scale: PerformScaling, scaleUp: false, scaleBgColor: Color.Black);
                }
                else
                {
                    Primitives.FillRectangle(rect, Color.DarkGreen);
                }
            }
            else
            {
                if (stoppedTexture != null)
                    Primitives.DrawImage(stoppedTexture, rect, Color.White, scale: PerformScaling, scaleUp: false, scaleBgColor: Color.Black);
                else
                    Primitives.FillRectangle(rect, Color.Black);
            }
            if (Root.IsMouseOver(rect))
            {
                mouseOverThisElement = true;
                Color clrSemitransparent = Color.FromNonPremultiplied(255, 255, 255, 150);
                Rectangle rectBottom = new Rectangle(rect.X, rect.Bottom - 5, rect.Width, 5);
                Primitives.FillRectangle(rectBottom, Color.Gray);
                double percent = videoPlayer.PlayPosition.TotalSeconds / videoPlayer.Video.Duration.TotalSeconds;
                Primitives.FillRectangle(new Rectangle(rectBottom.X, rectBottom.Y, (int)(rectBottom.Width * percent), rectBottom.Height), Color.White);
                const int ICONSIZE = 30;
                Rectangle rectPlayPause = new Rectangle(rectBottom.X + 1, rectBottom.Bottom - 1 - ICONSIZE, ICONSIZE, ICONSIZE);
                Rectangle rectStop = new Rectangle(rectBottom.X + ICONSIZE + 1, rectBottom.Bottom - 1 - ICONSIZE, ICONSIZE, ICONSIZE);
                Rectangle rectFullscreen = new Rectangle(rectBottom.Right - ICONSIZE - 1, rectBottom.Bottom - 1 - ICONSIZE, ICONSIZE, ICONSIZE);
                if (hasPlayPauseButton)
                {
                    Texture2D icon = videoPlayer.State == MediaState.Playing ? Library.IconPause : Library.IconPlay;
                    mouseOverPlayPauseButton = Root.IsMouseOver(rectPlayPause);
                    sb.Draw(icon, rectPlayPause, mouseOverPlayPauseButton ? Color.White : clrSemitransparent);
                    if (State == MediaState.Playing || State == MediaState.Paused)
                    {
                        mouseOverStopButton = Root.IsMouseOver(rectStop);
                        sb.Draw(Library.IconStop, rectStop, mouseOverStopButton ? Color.White : clrSemitransparent);
                    }
                }
                if (hasExtendToFullscreenButton && !alreadyFullscreen)
                {
                    mouseOverFullscreenButton = Root.IsMouseOver(rectFullscreen);
                    sb.Draw(Library.IconFullscreen, rectFullscreen, mouseOverFullscreenButton ? Color.White : clrSemitransparent);
                }

                if (onClickPlayPause)
                    sb.Draw(videoPlayer.State == MediaState.Playing ? Library.IconPause : Library.IconPlay, new Rectangle(rect.X + rect.Width / 2 - 30, rect.Y + rect.Height / 2 - 30, 60, 60), clrSemitransparent);
                if (onClickExtendToFullscreen && !alreadyFullscreen)
                    sb.Draw(Library.IconFullscreen, new Rectangle(rect.X + rect.Width / 2 - 30, rect.Y + rect.Height / 2 - 30, 60, 60), clrSemitransparent);

            }
        }
        /// <summary>
        /// Creates a new video player.
        /// </summary>
        /// <param name="video">Video to play.</param>
        /// <param name="defaultTexture">Texture to display when video is not playing.</param>
        /// <param name="hasButtons">Show buttons (play, stop, fullscreen)</param>
        /// <param name="onclickFullscreen">When clicked, toggles fullscreen.</param>
        /// <param name="onclickPlayPause">When clicked, plays or pauses the video playback.</param>
        public ImprovedVideoPlayer(Video video, Texture2D defaultTexture, bool hasButtons = true, bool onclickFullscreen = false, bool onclickPlayPause = false)
        {
            Video = video;
            stoppedTexture = defaultTexture;
            if (hasButtons)
            {
                hasExtendToFullscreenButton = true;
                hasPlayPauseButton = true;
            }
            if (onclickPlayPause)
                onClickPlayPause = true;
            if (onclickFullscreen)
                onClickExtendToFullscreen = true;
        }

        /// <summary>
        /// Gets the texture currently displayed in the video.
        /// </summary>
        /// <returns></returns>
        public Texture2D GetTexture()
        {
            return videoPlayer.GetTexture();
        }
        /// <summary>
        /// Gets or sets whether to loop the video.
        /// </summary>
        public bool IsLooped
        {
            get { return videoPlayer.IsLooped; }
            set { videoPlayer.IsLooped = value; }
        }
        /// <summary>
        /// Pauses the video.
        /// </summary>
        public void Pause()
        {
            videoPlayer.Pause();
        }
        /// <summary>
        /// Gets or sets whether the video is muted.
        /// </summary>
        public bool IsMuted
        {
            get { return videoPlayer.IsMuted; }
            set { videoPlayer.IsMuted = value; }
        }
        /// <summary>
        /// Plays the video, if there is any.
        /// </summary>
        public void Play()
        {
            if (Video != null)
                videoPlayer.Play(Video);
        }
        /// <summary>
        /// Changes the video to the parameter, then begins playing it.
        /// </summary>
        /// <param name="video">The new video to play.</param>
        public void Play(Video video)
        {
            Video = video;
            videoPlayer.Play(video);
        }
        /// <summary>
        /// Changes the video to the parameter, then displays the first frame of it and pauses.
        /// </summary>
        /// <param name="video">The new video to play.</param>
        public void PlayAndStopAtFirstFrame(Video video)
        {
            videoPlayer.Play(video);
            videoPlayer.Pause();
        }
        /// <summary>
        /// Gets the time elapsed since the beginning of the video.
        /// </summary>
        public TimeSpan PlayPosition
        {
            get { return videoPlayer.PlayPosition; }
        }
        /// <summary>
        /// Resumes video playback.
        /// </summary>
        public void Resume()
        {
            videoPlayer.Resume();
        }
        /// <summary>
        /// Returns the state of the underlying media player.
        /// </summary>
        public MediaState State
        {
            get { return videoPlayer.State; }
        }
        /// <summary>
        /// Stops playback of the video.
        /// </summary>
        public void Stop()
        {
            videoPlayer.Stop();
        }
        /// <summary>
        /// Gets or sets the video volume. Has no effect if the video is muted.
        /// </summary>
        public float Volume
        {
            get { return videoPlayer.Volume; }
            set { videoPlayer.Volume = value; }
        }

        private Video Video { get; set; }

        /// <summary>
        /// Returns the video width in pixels.
        /// </summary>
        public int VideoWidth
        {
            get { return Video.Width; }
        }
        /// <summary>
        /// Returns the video height in pixels.
        /// </summary>
        public int VideoHeight
        {
            get { return Video.Height; }
        }
    }
}
