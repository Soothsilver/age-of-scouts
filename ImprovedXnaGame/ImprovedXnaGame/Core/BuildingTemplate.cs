using System;
using System.Collections.Generic;
using Age.Animation;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Core
{
    internal class BuildingTemplate
    {
        public BuildingId Id;
        public string Name;
        public string Description;
        public int TileWidth;
        public int TileHeight;
        public TextureName Icon { get; internal set; }
        public int WoodCost;
        public int FoodCost;

        private BuildingTemplate(BuildingId id, string name, string description, TextureName icon, int tileWidth, int tileHeight,
            int food, int wood)
        {
            Id = id;
            FoodCost = food;
            WoodCost = wood;
            Name = name;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Description = description;
            Icon = icon;
        }

        public static BuildingTemplate Kitchen = new BuildingTemplate(BuildingId.Kitchen, "Kuchyně", "Kuchyně je nejdůležitější budova ve hře {b}Age of Scouts{/b}. V kuchyni nabíráš {b}Pracanty{/b}, tito do kuchyně přináší nasbírané suroviny, a pomocí kuchyně také postupuješ do vyššího věku. Pokud je tvoje kuchyně zničena a všichni tvoji pracanti vyřazeni ze hry, nebudeš po zbytek úrovně schopný nic stavět, takže na svoji kuchyni dávej pozor.", TextureName.Kitchen,3,3, 0, 400);
        public static BuildingTemplate Tent = new BuildingTemplate(BuildingId.Tent, "Obytný stan", "Zvyšuje tvůj populační limit o 2. Pokud máš například 7 stanů, tak můžeš mít až 14 skautů.", TextureName.TentStandard,1,1, 20, 50);

        internal bool ApplyCost(Troop playerTroop)
        {
            if (playerTroop.Food >= this.FoodCost &&
                playerTroop.Wood >= this.WoodCost)
            {
                playerTroop.Food -= this.FoodCost;
                playerTroop.Wood -= this.WoodCost;
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static IEnumerable<BuildingTemplate> GetConstructiblesBy(Troop controller)
        {
            yield return Kitchen;
            yield return Tent;
        }

        public bool PlaceableOn(Session session, Tile tile)
        {
            for (int x = 0; x < TileWidth; x++)
            {
                for (int y = 0; y< TileHeight; y++)
                {
                    Tile alsoOn = session.Map.GetTileFromTileCoordinates(tile.X - x, tile.Y - y);
                    if (alsoOn == null || alsoOn.Type != TileType.Grass || alsoOn.NaturalObjectOccupant != null
                        || alsoOn.PreventsMovement)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal void DrawShadow(Session session, IScreenInformation screen, Tile tile, Color color)
        {
            Texture2D what = SpriteCache.GetColoredTexture(this.Icon, color);
            Vector2 whereTo = Isomath.TileToStandard(tile.X + 1, tile.Y + 1);
            Rectangle rectWhere = Isomath.StandardPersonToScreen(whereTo, what.Width, what.Height, screen);
            Color clrTint = Color.White.Alpha(150);
            if (!PlaceableOn(session, tile))
            {
                clrTint = Color.Red;
            }
            Primitives.DrawImage(what, rectWhere, clrTint);
        }
    }

    public enum BuildingId
    {
        Kitchen,
        Tent
    }
}