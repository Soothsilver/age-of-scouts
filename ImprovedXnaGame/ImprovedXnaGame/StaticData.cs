using Age.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age
{
    /// <summary>
    /// Static data, loaded once per application run, that don't fit elsewhere.
    /// </summary>
    static class StaticData
    {
        public static Stance[] AllStances = new[] { Stance.Aggressive, Stance.StandYourGround, Stance.Stealthy };
        internal static int CarryingCapacity = 10;
    }
}
