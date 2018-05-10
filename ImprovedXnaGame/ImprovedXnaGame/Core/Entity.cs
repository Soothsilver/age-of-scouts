using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.Core
{
    abstract class Entity
    {
        public TextureName Icon;
        public int PixelWidth;
        public int PixelHeight;
        public Vector2 FeetStdPosition;

        protected Entity(TextureName icon, Vector2 feetPosition)
        {
            Icon = icon;
            FeetStdPosition = feetPosition;
        }
        protected Entity(TextureName icon, int pixelWidth, int pixelHeight, Vector2 feetPosition)
        {
            Icon = icon;
            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;
            FeetStdPosition = feetPosition;
        }
    }
}