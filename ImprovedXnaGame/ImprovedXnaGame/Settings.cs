using Age.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age
{
    class Settings
    {
        internal static Settings Instance = new Settings();

        public float KeyboardMoveSpeed = Tile.WIDTH * 8;
        public float MouseMoveSpeed = Tile.WIDTH * 8;
        public bool EnableFogOfWar = true;
        public bool ShowDebugPoints = false;
        public bool ShowPerformanceIndicators = true;
        public float TimeFactor = 1;
    }
}
