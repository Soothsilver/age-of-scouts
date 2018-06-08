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
        private static Dictionary<TextureKey, Texture2D> Cache = new Dictionary<TextureKey, Texture2D>();

        public static Texture2D GetColoredTexture(TextureName textureName, bool horizontalFlip, Color color)
        {
            var key = new TextureKey(textureName, horizontalFlip, color);
            if (!Cache.ContainsKey(key))
            {
                Cache.Add(key, CreateColoredVariant(textureName, horizontalFlip, color));
            }
            return Cache[key];
        }

        private static Texture2D CreateColoredVariant(TextureName textureName, bool horizontalFlip, Color color)
        {
            var primaryTexture = Library.Get(textureName);
            Color[] oldData = new Color[primaryTexture.Width * primaryTexture.Height];
            primaryTexture.GetData(oldData);

            Texture2D newTexture = new Texture2D(Root.GraphicsDevice, primaryTexture.Width, primaryTexture.Height);
            Color[] newData = new Color[primaryTexture.Width * primaryTexture.Height];


            if (horizontalFlip)
            {
                // Flipped copy
                for (int column = 0; column < primaryTexture.Width; column++)
                {
                    int flippedColumn = primaryTexture.Width - column - 1;
                    for (int row = 0; row < primaryTexture.Height; row++)
                    {
                        int index = row * primaryTexture.Width + column;
                        int indexFlipped = row * primaryTexture.Width + flippedColumn;
                        newData[index] = oldData[indexFlipped];
                    }
                }
            }
            else
            {
                // Pure copy
                for (int i = 0; i < oldData.Length; i++)
                {
                    newData[i] = oldData[i];
                }
            }
            // Recolor
            for (int i = 0; i < newData.Length; i++)
            {
                if (newData[i] == Color.White)
                {
                    newData[i] = color;
                }
            }


            newTexture.SetData(newData);
            return newTexture;
        }
    }
}