using Age.HUD;
using Auxiliary;
using System;
using System.Collections.Generic;

namespace Age.Core
{
    class Tile : PathingVertex
    {
        internal static int WIDTH = 128;
        internal static int HEIGHT = 64;

        internal static int HALF_WIDTH = 64;
        internal static int HALF_HEIGHT = 32;
        public int X;
        public int Y;
        public TextureName Icon;
        public TileType Type;
        public FogOfWarStatus Fog;
        public float SecondsUntilFogStatusCanChange = 0;
        public List<Unit> Occupants = new List<Unit>();
        public List<Corpse> BrokenOccupants = new List<Corpse>();
        public NaturalObject NaturalObjectOccupant = null;
        public Building BuildingOccupant = null;
        public Neighbours Neighbours = new Neighbours();

        public Tile(int x, int y, TextureName icon)
        {
            X = x;
            Y = y;
            Icon = icon;
            Fog = FogOfWarStatus.Black;
        }

        public bool PreventsMovement => this.Type == TileType.Water || (this.NaturalObjectOccupant?.PreventsMovement ?? false)
            || this.BuildingOccupant != null;

        public bool PreventsProjectiles => this.NaturalObjectOccupant?.PreventsProjectiles ?? false;

        public override string ToString()
        {
            return X + ":" + Y;
        }

        internal void SetClearFogStatus(float cleartime)
        {
            if (this.Fog == FogOfWarStatus.Clear && this.SecondsUntilFogStatusCanChange > cleartime)
            {
                return;
            }
            this.Fog = FogOfWarStatus.Clear;
            this.SecondsUntilFogStatusCanChange = cleartime;
        }

        internal Tooltip GetTooltip()
        {
            switch (NaturalObjectOccupant?.EntityKind) {
                case EntityKind.BerryBush:
                    return new Tooltip("Lesní plody", "Z tohoto keře můžou tvoji {b}Pracanti{/b} sbírat lesní plody. Až je donesou zpět do kuchyně, získáš {lime}jídlo{/lime}.");
                case EntityKind.Corn:
                    return new Tooltip("Kukuřice", "Z této části kukuřičného pole můžou tvoji {b}Pracanti{/b} odnést kukuřici do kuchyně a získat tak {lime}jídlo{/lime}.");
                case EntityKind.TutorialFlag:
                    return new Tooltip("Vlajka", "V některých úrovních je cílem hry se dostat k vlajce.");
                case EntityKind.UnalignedTent:
                    return new Tooltip("Nezničitelný stan", "Pravidla hry nepovolují útočit na tento stan. Je třeba ho obejít.");
                case EntityKind.UntraversableTree:
                    return new Tooltip("Strom", "Stomy blokují pohyb, ale tvoji {b}Pracanti{/b} ho mohou pokácet a odnést do kuchyně nebo dřevního kouta a získat tak {red}dřevo{/red}.");

                default: return null;
            }
        }
    }

    internal class PathingVertex
    {

        public int Pathfinding_EncounteredDuringSearch;
        public bool Pathfinding_Closed;
        public int Pathfinding_F;
        public int Pathfinding_G;
        public Tile Pathfinding_Parent;
    }

    class Neighbours
    {
        public Tile Top;
        public Tile Right;
        public Tile Bottom;
        public Tile Left;
        public Tile TopRight;
        public Tile BottomRight;
        public Tile BottomLeft;
        public Tile TopLeft;
        public List<Tile> All = new List<Tile>();
        public List<Edge> Traversable = new List<Edge>();
        public void FillAll()
        {
            All.Clear();
            Traversable.Clear();
            int CORNER_VERTICAL = 64;
            int CORNER_HORIZONTAL = 128;
            int EDGE = 72;
            FillAll(Top, CORNER_VERTICAL, TopLeft, TopRight);
            FillAll(Right, CORNER_HORIZONTAL, TopRight, BottomRight);
            FillAll(Bottom, CORNER_VERTICAL, BottomLeft, BottomRight);
            FillAll(Left, CORNER_HORIZONTAL, TopLeft, BottomLeft);

            FillAll(TopLeft, EDGE);
            FillAll(TopRight, EDGE);
            FillAll(BottomLeft, EDGE);
            FillAll(BottomRight, EDGE);
        }

        private void FillAll(Tile tile, int difficulty, Tile wallBlocker, Tile wallBlocker2)
        {
            if (tile != null)
            {
                All.Add(tile);
                if (wallBlocker != null && wallBlocker.PreventsMovement) return;
                if (wallBlocker2 != null && wallBlocker2.PreventsMovement) return;
                Traversable.Add(new Edge(tile, difficulty));
            }
        }

        private void FillAll(Tile tile, int difficulty)
        {
            if (tile != null)
            {
                All.Add(tile);
                Traversable.Add(new Edge(tile, difficulty));
            }
        }
    }
    class Edge
    {
        public Tile Destination;
        public int Difficulty;
        public Edge (Tile destination, int difficulty)
        {
            Difficulty = difficulty;
            Destination = destination;
        }
    }
}