using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Age.Animation
{
    class TextureKey
    {
        public Color Color;
        public TextureName OriginalTexture;
        public bool HorizontalFlip;

        public TextureKey(TextureName textureName, bool horizontalFlip, Color color)
        {
            OriginalTexture = textureName;
            HorizontalFlip = horizontalFlip;
            Color = color;
        }

        public override bool Equals(object obj)
        {
            if ( obj != null && obj.GetType() == typeof(TextureKey))
            {
                TextureKey key = (TextureKey)obj;
                return key.Color == this.Color && key.OriginalTexture == this.OriginalTexture && key.HorizontalFlip == this.HorizontalFlip;
            }
            else
            {
                return false;
            }                
        }

        public override int GetHashCode()
        {
            return this.Color.GetHashCode() * 33 + this.OriginalTexture.GetHashCode() * 33 * 33 + this.HorizontalFlip.GetHashCode();
        }
    }
}
