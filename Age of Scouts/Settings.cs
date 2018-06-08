using Age.Core;
using Auxiliary;
using Cother;
using Microsoft.Xna.Framework.Graphics;

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
        /// Enables display of the fog of war. Fog of war is calculated only if this is enabled.
        /// </summary>
        public bool EnableFogOfWar = true;

        public bool ShowDebugPoints = false;
        public bool VSync = true;
        public bool FixTimeStep = true;

        /// <summary>
        /// Shows FPS, UPS (updates-per-second) and other indicators that may point to a performance problem.
        /// </summary>
        public bool ShowPerformanceIndicators = true;

        public float TimeFactor = 1;

        public DisplayModus DisplayMode = DisplayModus.BorderlessWindow;
        public Resolution Resolution = new Resolution(1920, 1080);
        public bool EnemyUnitsRevealFogOfWar = false;
        /// <summary>
        /// If true, then all buildings are completed instantly, for all players.
        /// </summary>
        public bool Aegis;
        public float MusicVolume = 1;
        public float SfxVolume = 1;
    }
}
