using Age.HUD;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Priority_Queue;

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
            || (this.BuildingOccupant != null && !this.BuildingOccupant.SelfConstructionInProgress);

        public bool PreventsProjectiles => this.NaturalObjectOccupant?.PreventsProjectiles ?? false;

        public float SpeedMultiplier
        {
            get
            {
                if (NaturalObjectOccupant != null)
                {
                    return NaturalObjectOccupant.SpeedMultiplier * (this.Type == TileType.Mud ? 0.5f : 1);
                }
                if (this.Type == TileType.Road)
                {
                    return 1.1f;
                }
                else if (Type == TileType.Mud)
                {
                    return 0.5f;
                }
                else
                {
                    return 1;
                }
            }
        }

        public override string ToString()
        {
            return X + ":" + Y;
        }

        internal void SetClearFogStatus(float cleartime, float[,] revealMap)
        {
            if (this.Fog == FogOfWarStatus.Clear && revealMap[this.X, this.Y] > cleartime)
            {
                return;
            }
            this.Fog = FogOfWarStatus.Clear;
            revealMap[this.X, this.Y] = cleartime;
        }

        internal Tooltip GetTooltip()
        {
            if (NaturalObjectOccupant != null)
            {
                return new Tooltip(NaturalObjectOccupant.Name, NaturalObjectOccupant.Description);
            }
            return null;
        }

        public void CopyValuesFrom(Tile realMapTile)
        {
            this.Fog = realMapTile.Fog;
            this.Icon = realMapTile.Icon;
            this.Type = realMapTile.Type;
            this.NaturalObjectOccupant = realMapTile.NaturalObjectOccupant; // TODO not thread safe
            this.BuildingOccupant = realMapTile.BuildingOccupant; // TODO not thread safe
            this.Occupants.Clear();
            this.Occupants.AddRange(realMapTile.Occupants); // TODO not thread sfae

        }
    }

    internal class PathingVertex : FastPriorityQueueNode
    {

        public int Pathfinding_EncounteredDuringSearch;
        public bool Pathfinding_Closed;
        public int Pathfinding_F;
        public int Pathfinding_G;
        public int Pathfinding_IsTargetDuringThisSearch;
        public Vector2 Pathfinding_TargetPreciseLocation;
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