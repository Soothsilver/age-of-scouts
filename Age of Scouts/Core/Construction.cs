using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Core
{
    class Construction
    {
        public UnitTemplate ConstructingWhat;
        public float WorkDoneInSeconds;
        public float TotalWorkNeeded;
        internal string Subcaption;
        internal string Caption;

        public Construction(UnitTemplate unitTemplate)
        {
            ConstructingWhat = unitTemplate;
            WorkDoneInSeconds = 0;
            Caption = unitTemplate.Name;
            Subcaption = "Nábor...";
            TotalWorkNeeded = 10;
        }

        internal bool Completed(Building building)
        {
            if (building.Controller.PopulationLimit - building.Controller.PopulationUsed < 1)
            {
                return false;
            }

            Unit trainee = new Unit(NameGenerator.GenerateBoyName(), building.Controller, ConstructingWhat,
                building.FindExitPoint());
            building.Session.SpawnUnit(trainee);
            if (trainee.Controller == building.Session.PlayerTroop)
            {
                trainee.UnitTemplate.PlayUnitCreatedSound();
            }
            if (building.RallyPointInStandardCoordinates != Vector2.Zero)
            {
                trainee.Strategy.ResetTo(building.RallyPointInStandardCoordinates);
            }
            return true;
        }

        internal void Rebate(Troop controller)
        {
            controller.Food += ConstructingWhat.FoodCost;
            controller.Wood += ConstructingWhat.WoodCost;
            controller.Clay += 0; // TODO clay units
        }
    }
}
