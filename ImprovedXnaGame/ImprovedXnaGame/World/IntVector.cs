using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Age.World
{
    struct IntVector
    {
        public int X;
        public int Y;
        public IntVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(IntVector thisVector)
        {
            return new Vector2(thisVector.X, thisVector.Y);
        }

        public override string ToString()
        {
            return X + ":" + Y;
        }
    }
}
