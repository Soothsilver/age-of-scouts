using System;
using Auxiliary;

namespace Age.Core
{
    class LeaderPower
    {
        public LeaderPowerID ID;
        public TextureName Icon;
        public string Name;
        public string Description;
        internal string InstructionalLine;

        public LeaderPower(LeaderPowerID id, TextureName icon, string name, string description, string instructionalLine)
        {
            ID = id;
            Icon = icon;
            InstructionalLine = instructionalLine;
            Name = name;
            Description = description;
        }

        internal static LeaderPower CreateSpy()
        {
            return new LeaderPower(LeaderPowerID.Spy, TextureName.PruzkumGodPower, "Tišíkův průzkum", "Tvůj rádce ti na krátkou dobu ukáže zahalenou část mapy.", "Klikni levým tlačítkem myši na kamkoliv, abys tam seslal {i}Tišíkův průzkum{/i}.");
        }

        internal static LeaderPower CreateArtillery()
        {
            return new LeaderPower(LeaderPowerID.Artillery, TextureName.ArtilleryGodPower, "Papírová děla", "Tvoji spojenci vystřelí mraky hadráků z dalekých děl. Tyto hadráky zraňují jak soupeře, tak i tebe!", "Klikni levým tlačítkem myši na kamkoliv, abys tam seslal mraky hadráků.");
        }
    }

    enum LeaderPowerID
    {
        Spy,
        Artillery
    }
}