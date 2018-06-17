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
        public string EncyclopediaFilename;

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
            SelectionSfx = SoundEffectName.Chord,
            EncyclopediaFilename = "Kuchyne"
        };
        public static BuildingTemplate Tent = new BuildingTemplate(BuildingId.Tent, "Obytný stan", "Zvyšuje tvůj populační limit o 2. Pokud máš například 7 stanů, tak můžeš mít až 14 skautů.", TextureName.TentStandard, 1, 1, 20, 50, 0)
        {
            SecondsToBuild = 20,
            SelectionSfx = SoundEffectName.StandardTent,
            MaxHP = 100
            ,EncyclopediaFilename = "ObytnyStan"
        };
        public static BuildingTemplate MunitionTent = new BuildingTemplate(BuildingId.MunitionTent, "Muniční stan", "Dají se z něj nabírat {b}hadrákostřelci{/b}.", TextureName.MunitionTent, 2, 2, 0, 200, 0)
        {
            SecondsToBuild = 50,
            SelectionSfx = SoundEffectName.MunitionTent,
            MaxHP = 250
        };
        public static BuildingTemplate HadrakoVez = new BuildingTemplate(BuildingId.Hadrakovez, "Hadráková věž", "Střílí shora velké množství hadráků po nepřátelích.", TextureName.Tower, 1, 1, 20, 150, 100)
        {
            SecondsToBuild = 30,
            SelectionSfx = SoundEffectName.LadderClimb,
            LineOfSightInTiles = 8,
            MaxHP = 250
        };
        public static BuildingTemplate MajestatniSocha = new BuildingTemplate(BuildingId.MajestatniSocha, "Majestátní socha", "Obrovské umělecké dílo, které je důkazem schopností, vytrvalosti a oddanosti skautů ve tvém oddíle. Jakmile postavíš Majestatní sochu, začne odpočet, na jehož konci automaticky vyhraješ úroveň, pokud do té doby nikdo tvoji sochu nezboří.", TextureName.Statue, 4, 4, 200, 500, 1500)
        {
            SecondsToBuild = 500,
            SelectionSfx = SoundEffectName.SmallFanfare,
            MaxHP = 1500
        };
        public static BuildingTemplate Skladiste = new BuildingTemplate(BuildingId.Skladiste, "Skladiště", "Do skladiště mohou tvoji Pracanti odnášet dřevo a turbojíl, aby je nemuseli nosit až do kuchyně.", TextureName.Skladiste, 1, 1, 0, 100, 0)
        {
            SecondsToBuild = 20,
            SelectionSfx = SoundEffectName.MovingBoxes,
            MaxHP = 100,
            EncyclopediaFilename = "Skladiste"
        };
        public static BuildingTemplate Sklipek = new BuildingTemplate(BuildingId.Sklipek, "Sklípek", "Do sklípku mohou tvoji Pracanti odnášet jídlo, aby ho nemuseli nosit až do kuchyně.", TextureName.Sklipek, 1, 1, 0, 100, 0)
        {
            SecondsToBuild = 20,
            SelectionSfx = SoundEffectName.StandardTent,
            MaxHP = 100,
            EncyclopediaFilename = "Sklipek"
        };
        public static BuildingTemplate DrevarskyKout = new BuildingTemplate(BuildingId.DrevarskyKout, "Dřevařský kout", "V dřevařském koutě můžeš stavět katapulty, a pracanti tam mohou odnášet dřevo.", TextureName.DrevarskyKout, 2, 2, 0, 300, 0)
        {
            SecondsToBuild = 70,
            SelectionSfx = SoundEffectName.Saw,
            MaxHP = 250,
            EncyclopediaFilename = "DrevarskyKout"
        };
        public static BuildingTemplate Wall = new BuildingTemplate(BuildingId.Wall, "Val", "Zeď chrání váš tábor před nepřítelem.", TextureName.WallStandalone, 1, 1, 0, 0, 10)
        {
            SecondsToBuild = 10,
            SelectionSfx = SoundEffectName.MovingBoxes,
            EncyclopediaFilename = "Val",
            MaxHP = 250,
            LineOfSightInTiles = 2
        };
        public int LineOfSightInTiles = 3;
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
                    if (alsoOn == null || alsoOn.NaturalObjectOccupant != null
                        || alsoOn.BuildingOccupant != null
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
            Texture2D what = SpriteCache.GetColoredTexture(this.Icon, false, color);
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
        Hadrakovez,
        MunitionTent,
        MajestatniSocha,
        Skladiste,
        Sklipek,
        DrevarskyKout,
        Wall
    }
}