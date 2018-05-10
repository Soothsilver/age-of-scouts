using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Core
{
    class NaturalObject : Entity
    {
        internal EntityKind EntityKind;
        public bool PreventsMovement => SpeedMultiplier == 0;
        public float SpeedMultiplier = 1;
        public bool PreventsProjectiles = false;
        public string Name;
        public string Description;

        private NaturalObject(String name, string description, TextureName icon, int pixelWidth, int pixelHeight, Vector2 feetPosition, EntityKind entityKind,
            float speedMultiplier, bool preventsProjectiles) : base(icon, pixelWidth, pixelHeight, feetPosition)
        {
            Name = name;
            Description = description;
            this.EntityKind = entityKind;
            this.PreventsProjectiles = preventsProjectiles;
            this.SpeedMultiplier = speedMultiplier;
        }

        public static NaturalObject Create(TextureName icon, EntityKind entityKind, int tileX, int tileY)
        {
            Texture2D drawnIcon = Library.Get(icon);
            int width = drawnIcon.Width;
            int height = drawnIcon.Height;
            Vector2 feet = (Vector2)Isomath.TileToStandard(tileX + 1, tileY + 1);
            bool preventsProjectiles = false;
            float speedMultiplier = 0;
            String name = null;
            String desc = null;
            switch (entityKind)
            {
                case EntityKind.Corn:
                    speedMultiplier = 0.5f;
                    break;
                case EntityKind.TallGrass:
                    speedMultiplier = 0.8f;
                    break;
                case EntityKind.TutorialFlag:
                    speedMultiplier = 1;
                    break;
                case EntityKind.UnalignedTent:
                    preventsProjectiles = true;
                    break;
                case EntityKind.UntraversableTree:
                    preventsProjectiles = true;
                    break;
            }
            return new NaturalObject(name, desc, icon, width, height, feet, entityKind, speedMultiplier, preventsProjectiles);

        }
    }
}
