using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age
{
    public static class Extensions
    {
        public static bool WithinDistance(this Vector2 me, Vector2 withinDistanceOf, int distance)
        {
            return (me - withinDistanceOf).LengthSquared() <= distance * distance;
        }
    }
}
