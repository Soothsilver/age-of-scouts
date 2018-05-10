using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Auxiliary
{
    public struct RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public Rectangle ToRectangle()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }
        public RectangleF(float x, float y, float w, float h)
        {
            X = x; Y = y; Width = w; Height = h;
        }
        public static implicit operator RectangleF(Rectangle rect)
        {
            RectangleF rf = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
            return rf;
        }
    }
}
