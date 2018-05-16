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

        public static IntVector Zero = new IntVector(0, 0);

        public static implicit operator Vector2(IntVector thisVector)
        {
            return new Vector2(thisVector.X, thisVector.Y);
        }

        public override string ToString()
        {
            return X + ":" + Y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IntVector))
            {
                return false;
            }

            var vector = (IntVector)obj;
            return X == vector.X &&
                   Y == vector.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(IntVector one, IntVector two)
        {
            return one.X == two.X && one.Y == two.Y;
        }

        public static bool operator !=(IntVector one, IntVector two)
        {
            return !(one == two);
        }
    }
}
