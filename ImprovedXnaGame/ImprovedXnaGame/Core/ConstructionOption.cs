using System;
using System.Collections.Generic;
using Age.Phases;
using Auxiliary;

namespace Age.Core
{
    class ConstructionOption
    {
        public static List<ConstructionOption> None { get; } = new List<ConstructionOption>();
        public static List<ConstructionOption> MunitionTentOptions { get; internal set; } = new List<ConstructionOption>();
        public static List<ConstructionOption> KitchenOptions { get; internal set; } = new List<ConstructionOption>();
        public static List<ConstructionOption> PracantOptions { get; internal set; } = new List<ConstructionOption>();

        public static void InitializeAllConstructionOptions()
        {
            foreach(BuildingTemplate buildingTemplate in new[] {  BuildingTemplate.Kitchen, BuildingTemplate.Tent, BuildingTemplate.MunitionTent, BuildingTemplate.HadrakoVez})
            {
                PracantOptions.Add(new ConstructionOption("Postavit budovu " + buildingTemplate.Name, buildingTemplate.Description,
                    (b, s) =>
                    {
                        s.SelectedBuildingToPlace = buildingTemplate;
                    }, buildingTemplate.FoodCost, buildingTemplate.WoodCost, 0, buildingTemplate.Icon));
            }
            foreach(UnitTemplate unitTemplate in new[] {  UnitTemplate.Pracant, UnitTemplate.Hadrakostrelec })
            {
                var option = new ConstructionOption("Nabrat jednotku " + unitTemplate.Name, unitTemplate.Description,
                    (b, s) =>
                    {
                        b.EnqueueConstruction(unitTemplate);
                    }, unitTemplate.FoodCost, unitTemplate.WoodCost, 1, unitTemplate.Icon);
                KitchenOptions.Add(option);
                if (unitTemplate.CanAttack)
                {
                    MunitionTentOptions.Add(option);
                }
            }
        }

        private ConstructionOption(string caption, string description, Action<Building, Selection> whatDo, int foodCost, int woodCost, int populationCost, TextureName icon)
        {
            this.foodCost = foodCost;
            this.populationCost = populationCost;
            this.woodCost = woodCost;
            OnClick = whatDo;
            Description = description;
            this.caption = caption;
            Icon = icon;
        }

        private int foodCost;
        private int woodCost;
        private int populationCost;
        private string caption;
        public string TooltipCaption => caption + " (" + GetResourceCost() + ")";

        private string GetResourceCost()
        {
            string s = "";
            if (foodCost > 0)
            {
                s += foodCost + " jídla";
                if (woodCost > 0)
                {
                    s += ", ";
                }
            }
            if (woodCost > 0)
            {
                s += woodCost + " dřeva";
            }
            return s;
        }

        public string Description { get;  }

        internal bool AffordableBy(Troop controller)
        {
            return controller.Wood >= this.woodCost && controller.Food >= this.foodCost && 
                (this.populationCost == 0 || (controller.PopulationLimit - controller.PopulationUsed) >= this.populationCost);
        }

        public Action<Building, Selection> OnClick;
        public TextureName Icon;
    }
}