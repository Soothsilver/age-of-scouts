using System;
using Auxiliary;

namespace Age.Core
{
    public class LeaderPower
    {
        public TextureName Icon;
        public string Name;
        public string Description;
        internal string InstructionalLine;

        public LeaderPower(TextureName icon, string name, string description, string instructionalLine)
        {
            Icon = icon;
            InstructionalLine = instructionalLine;
            Name = name;
            Description = description;
        }

        internal static LeaderPower CreateSpy()
        {
            return new LeaderPower(TextureName.PruzkumGodPower, "Tišíkův průzkum", "Tvůj rádce ti na krátkou dobu ukáže zahalenou část mapy.", "Klikni levým tlačítkem myši na kamkoliv, abys tam seslal {i}Tišíkův průzkum{/i}.");
        }
    }
}