using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.Core
{
    abstract class AttackableEntity : Entity
    {
        public bool Broken;
        public float HP = 50;
        public int MaxHP = 50;

        protected AttackableEntity(TextureName icon, Vector2 feetPosition) : base(icon, feetPosition)
        {
        }

        protected AttackableEntity(TextureName icon, int pixelWidth, int pixelHeight, Vector2 feetPosition) : base(icon, pixelWidth, pixelHeight, feetPosition)
        {
        }

        public abstract void TakeDamage(int damage, Entity source);
    }
}
