using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age
{
    static class R
    {
        public static float Flicker = 0.5f;
        private static float FlickerMin = 0;
        private static float FlickerMax = 1;
        private static float FlickerSpeed = 0.5f;
        private static bool FlickerAscending = true;
        public static void UpdateFlicker(float elapsedSeconds)
        {
            Flicker += FlickerAscending ? FlickerSpeed * elapsedSeconds : -FlickerSpeed * elapsedSeconds;
            if (Flicker > FlickerMax) { Flicker = FlickerMax; FlickerAscending = false; }
            else if (Flicker < FlickerMin) { Flicker = FlickerMin; FlickerAscending = true; }
        }
        private static Random rgen = new Random();
        public static int Integer(int maxExclusive)
        {
            return rgen.Next(maxExclusive);
        }
        public static int Integer(int min, int maxExclusive)
        {
            return rgen.Next(min, maxExclusive);
        }
        public static float FloatAroundZero()
        {
            return (float)(2 * (rgen.NextDouble() - 0.5f));
        }
        public static float Float()
        {
            return (float)rgen.NextDouble();
        }
        public static bool Coin()
        {
            return rgen.Next(0, 2) == 0;
        }

        internal static Vector2 RandomUnitVector()
        {
            float x = FloatAroundZero();
            float y = (float)Math.Sqrt(1 - x * x);
            if (R.Coin()) y *= -1;
            return new Vector2(x, y);
        }
    }
}
