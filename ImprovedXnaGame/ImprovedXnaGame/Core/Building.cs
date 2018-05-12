using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Animation;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Core
{
    class Building : Entity
    {
        public Troop Controller;
        public Tile PrimaryTile;
        public BuildingTemplate Template;

        public Building(BuildingTemplate buildingTemplate, Troop controller, Vector2 feetPosition, Tile primaryTile) : base(buildingTemplate.Icon, feetPosition)
        {
            this.Template = buildingTemplate;
            Texture2D texture = Library.Get(buildingTemplate.Icon);
            this.PixelHeight = texture.Height;
            this.PixelWidth = texture.Width;
            this.Controller = controller;
            this.PrimaryTile = primaryTile;
        }

        public void Draw(IScreenInformation screen)
        {
            Rectangle rectWhere = Isomath.StandardPersonToScreen(this.FeetStdPosition, this.PixelWidth, this.PixelHeight, screen);
            Primitives.DrawImage(SpriteCache.GetColoredTexture(this.Template.Icon, this.Controller.LightColor), rectWhere);
        }
    }
}
