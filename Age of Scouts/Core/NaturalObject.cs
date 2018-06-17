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
        private const int STANDARD_FOOD_STORE = 300;
        private const int STANDARD_TREE_WOOD_STORE = 80;
        private const Resource NO_RESOURCE = 0;

        internal EntityKind EntityKind;
        public bool PreventsMovement => SpeedMultiplier == 0;      
        public float SpeedMultiplier = 1;
        public bool PreventsProjectiles = false;
        public override string Name { get; }
        public string Description;
        public Resource ProvidesResource = 0;
        public int ResourcesLeft;
        public Tile Occupies;
        private string encyclopediaFilename;
        public override string EncyclopediaFilename => encyclopediaFilename;

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
                case EntityKind.MudMine:
                    ProvidesResource = Resource.Clay;
                    ResourcesLeft = STANDARD_FOOD_STORE;
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
            string encyc = null;
       
            switch (entityKind)
            {
                case EntityKind.MudMine:
                    name = "Bahenní pole";
                    desc = "Zde mohou tvoji {b}Pracanti{/b} těžit {blue}turbojíl{/blue}.";
                    encyc = "BahenniPole";
                    break;
                case EntityKind.BerryBush:
                    name = "Lesní plody";
                    desc = "Z tohoto keře můžou tvoji {b}Pracanti{/b} sbírat lesní plody. Až je donesou zpět do kuchyně, získáš {lime}jídlo{/lime}.";
                    break;
                case EntityKind.Corn:
                    speedMultiplier = 0.5f;
                    name = "Kukuřice";
                    desc = "Z této části kukuřičného pole můžou tvoji {b}Pracanti{/b} odnést kukuřici do kuchyně a získat tak {lime}jídlo{/lime}.";
                    encyc = "Kukurice";
                    break;
                case EntityKind.CutDownTree:
                    speedMultiplier = 1;
                    name = "Pařez";
                    desc = "Tady kdysi byl strom.";
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
            return new NaturalObject(name, desc, icon, width, height, feet, session.Map.Tiles[tileX, tileY], entityKind, speedMultiplier, preventsProjectiles, session)
            {
                encyclopediaFilename = encyc
            };

        }

        internal void TransferOneResourceTo(Unit owner)
        {
            if (ResourcesLeft > 0)
            {
                ResourcesLeft--;
                if (owner.CarryingResource != this.ProvidesResource)
                {
                    owner.CarryingHowMuchResource = 0;
                    owner.CarryingResource = ProvidesResource;
                }
                owner.CarryingHowMuchResource++;
                if (this.EntityKind == EntityKind.UntraversableTree)
                {
                    if (this.ResourcesLeft > 0)
                    {
                        this.Icon = TextureName.TreeCutDown;
                    }
                }
                if (ResourcesLeft == 0)
                {
                    this.Occupies.NaturalObjectOccupant = null;
                    if (this.EntityKind == EntityKind.UntraversableTree)
                    {
                        NaturalObject stump = NaturalObject.Create(TextureName.TreeStump, EntityKind.CutDownTree, this.Occupies.X, this.Occupies.Y, this.Session);
                        this.Occupies.NaturalObjectOccupant = stump;
                    }
                }
            }
        }

        public override List<ConstructionOption> ConstructionOptions => ConstructionOption.None;

        internal void DrawActionInProgress(Rectangle rectAllSelectedUnits)
        {
            if (ProvidesResource != NO_RESOURCE)
            {
                Primitives.DrawImage(Library.Get(ProvidesResource.ToTextureName()), new Rectangle(rectAllSelectedUnits.X + 10, rectAllSelectedUnits.Bottom - 42, 32, 32));
                Primitives.DrawSingleLineText(ResourcesLeft.ToString(), new Vector2(rectAllSelectedUnits.X + 50, rectAllSelectedUnits.Bottom - 38),
                    Color.Black, Library.FontTinyBold);
            }
        }

        internal HashSet<Vector2> GetFreeSpotsForGatherers()
        {
            HashSet<Vector2> allocation = new HashSet<Vector2>();
            
            foreach (var neighbour in Occupies.Neighbours.All)
            {
                if (neighbour.Occupants.Count == 0)
                {
                    allocation.Add(Isomath.TileToStandard(neighbour.X + 0.5f, neighbour.Y + 0.5f));
                }
            }

            return allocation;
        }

        public override string ToString()
        {
            return this.EntityKind + " (" + this.Occupies.ToString() + ")";
        }
    }
}
