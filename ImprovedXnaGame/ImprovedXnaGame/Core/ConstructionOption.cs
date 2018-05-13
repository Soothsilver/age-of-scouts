using System;
using System.Collections.Generic;
using Age.Phases;
using Auxiliary;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Core
{
    class ConstructionOption
    {
        public static List<ConstructionOption> None { get; } = new List<ConstructionOption>();
        public static List<ConstructionOption> KitchenOptions { get; internal set; } = new List<ConstructionOption>();
        public static List<ConstructionOption> PracantOptions { get; internal set; } = new List<ConstructionOption>();

        public static void InitializeAllConstructionOptions()
        {
            foreach(BuildingTemplate buildingTemplate in new[] {  BuildingTemplate.Kitchen, BuildingTemplate.Tent})
            {
                PracantOptions.Add(new ConstructionOption("Postavit budovu " + buildingTemplate.Name, buildingTemplate.Description,
                    (b, s) =>
                    {
                        s.SelectedBuildingToPlace = buildingTemplate;
                    }, buildingTemplate.FoodCost, buildingTemplate.WoodCost, 0, buildingTemplate.Icon));
            }
            foreach(UnitTemplate unitTemplate in new[] {  UnitTemplate.Pracant, UnitTemplate.Hadrakostrelec })
            {
                KitchenOptions.Add(new ConstructionOption("Nabrat jednotku " + unitTemplate.Name, unitTemplate.Description,
                    (b, s) =>
                    {
                        b.EnqueueConstruction(unitTemplate);
                    }, unitTemplate.FoodCost, unitTemplate.WoodCost, 1, unitTemplate.Icon));
            }
        }

        private ConstructionOption(string caption, string description, Action<Building, Selection> whatDo, int foodCost, int woodCost, int populationCost, TextureName icon)
        {
            FoodCost = foodCost;
            PopulationCost = populationCost;
            WoodCost = woodCost;
            OnClick = whatDo;
            Description = description;
            _caption = caption;
            Icon = icon;
        }

        private int FoodCost;
        private int WoodCost;
        private int PopulationCost;
        private string _caption;
        public string TooltipCaption => _caption + " (" + GetResourceCost() + ")";

        private string GetResourceCost()
        {
            string s = "";
            if (FoodCost > 0)
            {
                s += FoodCost + " jídla";
                if (WoodCost > 0)
                {
                    s += ", ";
                }
            }
            if (WoodCost > 0)
            {
                s += WoodCost + " dřeva";
            }
            return s;
        }

        public string Description { get;  }

        internal bool AffordableBy(Troop controller)
        {
            return controller.Wood >= this.WoodCost && controller.Food >= this.FoodCost && 
                (this.PopulationCost == 0 || (controller.PopulationLimit - controller.PopulationUsed) >= this.PopulationCost);
        }

        public Action<Building, Selection> OnClick;
        public TextureName Icon;
    }
}