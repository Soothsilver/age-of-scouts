using Age.Animation;
using Age.Core;
using Age.HUD;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age
{
    static class Extensions
    {
        public static Texture2D Color(this TextureName textureName, Troop controller)
        {
            return SpriteCache.GetColoredTexture(textureName, controller.LightColor);
        }

        public static Color OverlayOnto(this Color color, Color overlayOntoWhat)
        {
            float dstA = (float)overlayOntoWhat.A / 255f;
            float srcA = (float)color.A / 255f;
            float outA = srcA + dstA * (1 - srcA);
            if (outA == 0)
            {
                return Microsoft.Xna.Framework.Color.Transparent;
            }

            int targetR = (int)(color.R + overlayOntoWhat.R * (1 - srcA));
            int targetG = (int)(color.G + overlayOntoWhat.G * (1 - srcA));
            int targetB = (int)(color.B +overlayOntoWhat.B * (1 - srcA));

            return new Color(targetR, targetG, targetB, (int)(outA * 255));
        }
        public static bool WithinDistance(this Vector2 me, Vector2 withinDistanceOf, int distance)
        {
            return (me - withinDistanceOf).LengthSquared() <= distance * distance;
        }
        public static TextureName ToTexture(this Stance stance)
        {
            switch(stance)
            {
                case Stance.Aggressive: return TextureName.AggressiveStance;
                case Stance.StandYourGround: return TextureName.StandYourGroundStance;
                case Stance.Stealthy: return TextureName.StealthyStance;
            }
            return TextureName.None;
        }
        public static Tooltip ToTooltip(this Stance stance)
        {
            switch (stance)
            {
                case Stance.Aggressive: return new Tooltip("Agresivní postoj", "Když se nepřítel přiblíží k této jednotce, tato jednotka na něj zaútočí a bude ho aktivně pronásledovat, dokud nebude nepřítel vyřazen.");
                case Stance.StandYourGround: return new Tooltip("Drž pozici", "Tato jednotka se nebude bez příkazů pohybovat, ale zaútočí na kolem procházející nepřátele.");
                case Stance.Stealthy: return new Tooltip("Plížení", "Tato jednotka se nebude bez příkazů pohybovat ani nebude na nikoho útočit.");
            }
            return new Tooltip("?", "!");
        }
    }
}
