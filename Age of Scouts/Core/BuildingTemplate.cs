using System;
using System.Collections.Generic;
using Age.Animation;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Core
{
    internal class BuildingTemplate : IHasCosts
    {
        public BuildingId Id;
        public string Name;
        public string Description;
        public int TileWidth;
        public int TileHeight;
        public TextureName Icon { get; internal set; }
        public int WoodCost { get; }
        public int FoodCost { get; }
        public int ClayCost { get; }
        public int PopulationCost => 0;
        public float SecondsToBuild;
        public SoundEffectName SelectionSfx;

        private BuildingTemplate(BuildingId id, string name, string description, TextureName icon, int tileWidth, int tileHeight,
            int food, int wood, int mud)
        {
            Id = id;
            ClayCost = mud;
            FoodCost = food;
            WoodCost = wood;
            Name = name;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Description = description;
            Icon = icon;
        }

        public static BuildingTemplate Kitchen = new BuildingTemplate(BuildingId.Kitchen, "Kuchyně", "Kuchyně je nejdůležitější budova ve hře {b}Age of Scouts{/b}. V kuchyni nabíráš {b}Pracanty{/b}, tito do kuchyně přináší nasbírané suroviny, a pomocí kuchyně také postupuješ do vyššího věku. Pokud je tvoje kuchyně zničena a všichni tvoji pracanti vyřazeni ze hry, nebudeš po zbytek úrovně schopný nic stavět, takže na svoji kuchyni dávej pozor.", TextureName.Kitchen, 3, 3, 0, 600, 0)
        {
            SecondsToBuild = 180,
            MaxHP = 500,
            SelectionSfx = SoundEffectName.Chord
        };
        public static BuildingTemplate Tent = new BuildingTemplate(BuildingId.Tent, "Obytný stan", "Zvyšuje tvůj populační limit o 2. Pokud máš například 7 stanů, tak můžeš mít až 14 skautů.", TextureName.TentStandard, 1, 1, 20, 50, 0)
        {
            SecondsToBuild = 20,
            SelectionSfx = SoundEffectName.StandardTent,
            MaxHP = 100
        };
        public static BuildingTemplate MunitionTent = new BuildingTemplate(BuildingId.MunitionTent, "Muniční stan", "Dají se z něj nabírat {b}hadrákostřelci{/b}.", TextureName.MunitionTent, 2, 2, 0, 200, 0)
        {
            SecondsToBuild = 50,
            SelectionSfx = SoundEffectName.MunitionTent,
            MaxHP = 250
        };
        public static BuildingTemplate HadrakoVez = new BuildingTemplate(BuildingId.HadrkoVez, "Hadráková věž", "Střílí shora velké množství hadráků po nepřátelích.", TextureName.Tower, 1, 1, 80, 150, 100)
        {
            SecondsToBuild = 30,
            SelectionSfx = SoundEffectName.LadderClimb,
            LineOfSightInTiles = 7,
            MaxHP = 250
        };
        public static BuildingTemplate MajestatniSocha = new BuildingTemplate(BuildingId.MajestatniSocha, "Majestátní socha", "Obrovské umělecké dílo, které je důkazem schopností, vytrvalosti a oddanosti skautů ve tvém oddíle. Jakmile postavíš Majestatní sochu, začne odpočet, na jehož konci automaticky vyhraješ úroveň, pokud do té doby nikdo tvoji sochu nezboří.", TextureName.Statue, 4, 4, 200, 500, 1500)
        {
            SecondsToBuild = 500,
            SelectionSfx = SoundEffectName.SmallFanfare,
            MaxHP = 1500
        };
        internal int LineOfSightInTiles = 3;
        public int MaxHP;

        internal bool ApplyCost(Troop troop)
        {
            if (AffordableBy(troop))
            {
                troop.Food -= this.FoodCost;
                troop.Wood -= this.WoodCost;
                troop.Clay -= this.ClayCost;
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool AffordableBy(Troop troop)
        {
            return
                troop.Food >= this.FoodCost &&
                troop.Wood >= this.WoodCost &&
                troop.Clay >= this.ClayCost;
        }

        public bool PlaceableOn(Session session, Tile tile, bool canYouBuildInFogOfWar)
        {
            if (tile == null) return false;
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
                    if (!canYouBuildInFogOfWar && alsoOn.Fog != FogOfWarStatus.Clear)
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
            if (!PlaceableOn(session, tile, !Settings.Instance.EnableFogOfWar))
            {
                clrTint = Color.Red;
            }
            Primitives.DrawImage(what, rectWhere, clrTint);
        }
    }

    public enum BuildingId
    {
        Kitchen,
        Tent,
        HadrkoVez,
        MunitionTent,
        MajestatniSocha
    }
}