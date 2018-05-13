﻿using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Age.Core
{
    abstract class Entity
    {
        public TextureName Icon;
        public int PixelWidth;
        public int PixelHeight;
        public Vector2 FeetStdPosition;
        public Session Session;



        public Troop Controller;

        public abstract string Name { get; }
        public abstract Texture2D BottomBarTexture { get; }
        public abstract List<ConstructionOption> ConstructionOptions { get; }

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