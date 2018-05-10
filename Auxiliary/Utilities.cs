using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;

namespace Auxiliary
{
    /// <summary>
    /// Provides several static utility functions related to XNA and graphics.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Loads an image from the given filename. Warning!: The Auxiliary.Root class must already be initialized with Root.Init() for this overload to work.
        /// </summary>
        public static Texture2D LoadTextureAtRuntime(string filename)
        {
            if (Root.inGraphics == null)
            {
                throw new InvalidOperationException(
                    "First call Root.Init(), or use the overload where you need to specify the GraphicsDevice.");
            }
            FileStream fs = new FileStream(filename, FileMode.Open);
            return Texture2D.FromStream(Root.inGraphics.GraphicsDevice, fs);
        }
        /// <summary>
        /// Loads an image from the given filename.
        /// </summary>
        public static Texture2D LoadTextureAtRuntime(string filename, GraphicsDevice graphics)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            return Texture2D.FromStream(graphics, fs);
        }
        /// <summary>
        /// Scales a rectangle identified by 'originalWidth' and 'originalHeight' to fill the area of 'target', except that its aspect ratio must be preserved. The returned rectangle is centered on the center of the target.
        /// </summary>
        /// <param name="target">The rectangle to be filled in.</param>
        /// <param name="originalWidth">VideoWidth of the scaled rectangle.</param>
        /// <param name="originalHeight">Height of the scaled rectangle.</param>
        /// <param name="alsoScaleUp">Even if the scaled rectangle is smaller in all dimensions than the target, it will still be scaled (up, in this case).</param>
        /// <returns></returns>
        public static Rectangle ScaleRectangle(Rectangle target, int originalWidth, int originalHeight, bool alsoScaleUp)
        {
            if (!alsoScaleUp && originalWidth < target.Width && originalHeight < target.Height) return new Rectangle(target.X + target.Width/2 - originalWidth/2, target.Y + target.Height/2-originalHeight/2,originalWidth,originalHeight);
            float orWidth = originalWidth;
            float orHeight = originalHeight;
            float maxWidth = target.Width;
            float maxHeight = target.Height;
            float xPresah = orWidth / maxWidth;
            float yPresah = orHeight / maxHeight;
            if (xPresah >= yPresah)
            {
                float xZvetseni = 1f / xPresah;
                orWidth = maxWidth;
                orHeight = orHeight * xZvetseni;
            }
            else
            {
                float yZvetseni = 1f / yPresah;
                orHeight = maxHeight;
                orWidth = orWidth * yZvetseni;
            }
            return new Rectangle(target.X + target.Width / 2 - (int)orWidth / 2, target.Y + target.Height / 2 - (int)orHeight / 2, (int)orWidth, (int)orHeight);
        }
        /// <summary>
        /// Gets the list of resolutions supported by the computer. It may not be accurate.
        /// </summary>
        public static List<Resolution> GetSupportedResolutions()
        {
            List<Resolution> resolutions = new List<Resolution>();
            foreach(DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                Resolution resolution = new Resolution(mode.Width, mode.Height);
                if (!resolutions.Contains(resolution))
                {
                    resolutions.Add(resolution);
                }
            }
            resolutions.Sort();
            return resolutions;
        }
        /// <summary>
        /// Returns a hard-coded list of common PC monitor resolutions.
        /// </summary>
        public static List<Resolution> GetCommonResolutions()
        {
            var resolutions = new List<Resolution>(
                new[] {
                        new Resolution(1280, 800),
                        new Resolution(1024, 768),
                        new Resolution(1366, 768),
                        new Resolution(1280, 1024),
                        new Resolution(1440, 900),
                        new Resolution(1680, 1050),
                        new Resolution(1920, 1080),
                        new Resolution(1600, 900),
                        new Resolution(1152, 864)
                    }
                );
            resolutions.Sort();
            return resolutions;
        }
    
    }
}
