using System;
using System.Collections.Generic;
using Auxiliary;

namespace Age.Core
{
    internal class BuildingTemplate
    {
        public string Name;
        public string Description;
        public int TileWidth;
        public int TileHeight;
        public TextureName Icon { get; internal set; }

        private BuildingTemplate(string name, string description, TextureName icon, int tileWidth, int tileHeight)
        {
            Name = name;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Description = description;
            Icon = icon;
        }

        public static BuildingTemplate Kitchen = new BuildingTemplate("Kuchyně", "Kuchyně je nejdůležitější budova ve hře {b}Age of Scouts{/b}. V kuchyni nabíráš {b}Pracanty{/b}, tito do kuchyně přináší nasbírané suroviny, a pomocí kuchyně také postupuješ do vyššího věku. Pokud je tvoje kuchyně zničena a všichni tvoji pracanti vyřazeni ze hry, nebudeš po zbytek úrovně schopný nic stavět, takže na svoji kuchyni dávej pozor.", TextureName.Kitchen,3,3);
        public static BuildingTemplate Tent = new BuildingTemplate("Obytný stan", "Zvyšuje tvůj populační limit o 2. Pokud máš například 7 stanů, tak můžeš mít až 14 skautů.", TextureName.TentStandard,1,1);

        internal static IEnumerable<BuildingTemplate> GetConstructiblesBy(Troop controller)
        {
            yield return Kitchen;
            yield return Tent;
        }

        internal bool AffordableBy(Troop controller)
        {
            return true;
        }
    }
}