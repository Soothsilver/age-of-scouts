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
        private const int STANDARD_FOOD_STORE = 600;
        private const int STANDARD_TREE_WOOD_STORE = 150;

        internal EntityKind EntityKind;
        public bool PreventsMovement => SpeedMultiplier == 0;
        public float SpeedMultiplier = 1;
        public bool PreventsProjectiles = false;
        public override string Name { get; }
        public string Description;
        public Resource ProvidesResource = 0;
        public int ResourcesLeft;
        public Tile Occupies;

        private NaturalObject(String name, string description, TextureName icon, int pixelWidth, int pixelHeight, Vector2 feetPosition, Tile occupies, EntityKind entityKind,
            float speedMultiplier, bool preventsProjectiles, Session session) : base(icon, pixelWidth, pixelHeight, feetPosition)
        {
            Name = name;
            Description = description;
            this.Occupies = occupies;
            this.EntityKind = entityKind;
            this.PreventsProjectiles = preventsProjectiles;
            this.SpeedMultiplier = speedMultiplier;
            this.Controller = session.GaiaTroop;
            this.Session = session;

          
            switch (entityKind)
            {
                case EntityKind.BerryBush:
                    ProvidesResource = Resource.Food;
                    ResourcesLeft = STANDARD_FOOD_STORE;
                    break;
                case EntityKind.Corn:
                    ProvidesResource = Resource.Food;
                    ResourcesLeft = STANDARD_TREE_WOOD_STORE;
                    break;
                case EntityKind.UntraversableTree:
                    ProvidesResource = Resource.Wood;
                    ResourcesLeft = STANDARD_TREE_WOOD_STORE;
                    break;
            }
        }

        public override Texture2D BottomBarTexture => Library.Get(Icon);

        public static NaturalObject Create(TextureName icon, EntityKind entityKind, int tileX, int tileY, Session session)
        {
            Texture2D drawnIcon = Library.Get(icon);
            int width = drawnIcon.Width;
            int height = drawnIcon.Height;
            Vector2 feet = (Vector2)Isomath.TileToStandard(tileX + 1, tileY + 1);
            bool preventsProjectiles = false;
            float speedMultiplier = 0;
            string name = null;
            string desc = null;
       
            switch (entityKind)
            {
                case EntityKind.BerryBush:
                    name = "Lesní plody";
                    desc = "Z tohoto keře můžou tvoji {b}Pracanti{/b} sbírat lesní plody. Až je donesou zpět do kuchyně, získáš {lime}jídlo{/lime}.";
                    break;
                case EntityKind.Corn:
                    speedMultiplier = 0.5f;
                    name = "Kukuřice";
                    desc = "Z této části kukuřičného pole můžou tvoji {b}Pracanti{/b} odnést kukuřici do kuchyně a získat tak {lime}jídlo{/lime}.";
                    break;
                case EntityKind.TallGrass:
                    speedMultiplier = 0.8f;
                    name = "Vysoká tráva";
                    desc = "Ve vysoké trávě jsou skauti více skrytí před nepřítelem.";
                    break;
                case EntityKind.TutorialFlag:
                    speedMultiplier = 1;
                    name = "Vlajka";
                    desc = "V některých úrovních je cílem hry se dostat k vlajce";
                    break;
                case EntityKind.UnalignedTent:
                    preventsProjectiles = true;
                    name = "Nezničitelný stan";
                    desc = "Pravidla hry nepovolují útočit na tento stan. Je třeba ho obejít.";
                    break;
                case EntityKind.UntraversableTree:
                    preventsProjectiles = true;
                    name = "Strom";
                    desc = "Stomy blokují pohyb, ale tvoji {b}Pracanti{/b} ho mohou pokácet a odnést do kuchyně nebo dřevního kouta a získat tak {red}dřevo{/red}.";
                    break;
            }
            return new NaturalObject(name, desc, icon, width, height, feet, session.Map.Tiles[tileX, tileY], entityKind, speedMultiplier, preventsProjectiles, session);

        }

        public override List<ConstructionOption> ConstructionOptions => ConstructionOption.None;

        internal void DrawActionInProgress(Rectangle rectAllSelectedUnits)
        {
            if (ProvidesResource != 0)
            {
                Primitives.DrawImage(Library.Get(ProvidesResource.ToTextureName()), new Rectangle(rectAllSelectedUnits.X + 10, rectAllSelectedUnits.Bottom - 42, 32, 32));
                Primitives.DrawSingleLineText(ResourcesLeft.ToString(), new Vector2(rectAllSelectedUnits.X + 50, rectAllSelectedUnits.Bottom - 38),
                    Color.Black, Library.FontTinyBold);
            }
        }
    }
}
