using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Age.Animation
{
    /// <summary>
    /// Holds colored variants of various textures, especially units and buildings, because each troop has different color units.
    /// </summary>
    static class SpriteCache
    {
        private static Dictionary<Tuple<TextureName, Color>, Texture2D> Cache = new Dictionary<Tuple<TextureName, Color>, Texture2D>();
        public static Texture2D GetColoredTexture(TextureName textureName, Color color)
        {
            var key = new Tuple<TextureName, Color>(textureName, color);
            if (!Cache.ContainsKey(key))
            {
                Cache.Add(key, CreateColoredVariant(textureName, color));
            }
            return Cache[key];
        }

        private static Texture2D CreateColoredVariant(TextureName textureName, Color color)
        {
            var primaryTexture = Library.Get(textureName);
            Color[] oldData = new Color[primaryTexture.Width * primaryTexture.Height];
            primaryTexture.GetData(oldData);

            Texture2D newTexture = new Texture2D(Root.GraphicsDevice, primaryTexture.Width, primaryTexture.Height);
            Color[] newData = new Color[primaryTexture.Width * primaryTexture.Height];

            for (int i = 0; i < oldData.Length; i++)
            {
                if (oldData[i] == Color.White)
                {
                    newData[i] = color;
                }
                else
                {
                    newData[i] = oldData[i];
                }
            }

            newTexture.SetData(newData);
            return newTexture;
        }
    }
}