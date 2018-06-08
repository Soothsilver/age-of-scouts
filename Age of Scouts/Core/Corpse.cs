using Age.Animation;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.Core
{
    class Corpse
    {
        private const float TIME_EXISTING = 24;
        private const float TIME_TO_DISAPPEAR = 4;
        public Unit WhatItWas;
        public float SecondsUntilDisappearance;
        public float Alpha;
        public bool Lost;

        public Corpse(Unit unit)
        {
            this.Lost = false;
            this.Alpha = 1;
            this.SecondsUntilDisappearance = TIME_EXISTING;
            this.WhatItWas = unit;
        }

        public void Draw(float elapsedSeconds, IScreenInformation screen)
        {
            SecondsUntilDisappearance -= elapsedSeconds;
            if (SecondsUntilDisappearance <= TIME_TO_DISAPPEAR)
            {
                Alpha -= elapsedSeconds / TIME_TO_DISAPPEAR;
            }
            if (SecondsUntilDisappearance <=0)
            {
                this.Lost = true;
            }
            Rectangle rectCorpse = Isomath.StandardPersonToScreen(this.WhatItWas.FeetStdPosition, WhatItWas.Sprite.Sprite.Width, WhatItWas.Sprite.Sprite.Height, screen);
            Primitives.DrawImage(SpriteCache.GetColoredTexture(WhatItWas.UnitTemplate.DeadIcon, false, WhatItWas.Sprite.Color),
                rectCorpse, Color.White.Alpha((int)(Alpha * 255)));
        }
    }
}