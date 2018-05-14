﻿using Age.Core;

namespace Age
{
    /// <summary>
    /// Application-wide settings, many for debug purposes only.
    /// </summary>
    class Settings
    {
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        internal static Settings Instance = new Settings();

        public float KeyboardMoveSpeed = Tile.WIDTH * 8;
        public float MouseMoveSpeed = Tile.WIDTH * 8;
        /// <summary>
        /// Enables display of the fog of war. Fog of war is calculated even if this is disabled.
        /// </summary>
        public bool EnableFogOfWar = true;
        public bool ShowDebugPoints = false;
        /// <summary>
        /// Shows FPS, UPS (updates-per-second) and other indicators that may point to a performance problem.
        /// </summary>
        public bool ShowPerformanceIndicators = true;
        public float TimeFactor = 1;

        private Settings()
        {
        }
    }
}
