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
        public abstract string EncyclopediaFilename { get; }


        public Troop Controller;

        public abstract string Name { get; }
        public abstract Texture2D BottomBarTexture { get; }
        public abstract List<ConstructionOption> ConstructionOptions { get; }
        public virtual bool CanAttack => false;

        public bool CanRangeAttack(AttackableEntity attackTarget, int tileRange)
        {
            return
                this.CanAttack &&
                attackTarget != null && this.Session.AreEnemies(this, attackTarget)
                && !attackTarget.Broken 
                && this.FeetStdPosition.WithinDistance(attackTarget.FeetStdPosition, tileRange * Tile.HEIGHT)
                && Session.Map.HasLineOfEffect(this.FeetStdPosition, attackTarget.FeetStdPosition);
        }

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