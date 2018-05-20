using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Core
{
    interface IHasCosts
    {
        int FoodCost { get; }
        int WoodCost { get; }
        int ClayCost { get; }
        int PopulationCost { get; }
    }
}
