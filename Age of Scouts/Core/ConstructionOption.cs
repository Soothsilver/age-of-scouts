using System;
using System.Collections.Generic;
using Age.Phases;
using Auxiliary;

namespace Age.Core
{
    class ConstructionOption : IHasCosts
    {
        public static List<ConstructionOption> None { get; } = new List<ConstructionOption>();
        public static List<ConstructionOption> MunitionTentOptions { get; internal set; } = new List<ConstructionOption>();
        public static List<ConstructionOption> DrevarskyKoutOptions { get; internal set; } = new List<ConstructionOption>();
        public static List<ConstructionOption> KitchenOptions { get; internal set; } = new List<ConstructionOption>();
        public static List<ConstructionOption> PracantOptions { get; internal set; } = new List<ConstructionOption>();
        public static List<ConstructionOption> RadeninOptions { get; internal set; } = new List<ConstructionOption>();

        public static void InitializeAllConstructionOptions()
        {
            foreach(BuildingTemplate buildingTemplate in new[] {  BuildingTemplate.Kitchen, BuildingTemplate.Tent, BuildingTemplate.Skladiste, BuildingTemplate.Sklipek , BuildingTemplate.MunitionTent, BuildingTemplate.DrevarskyKout, BuildingTemplate.HadrakoVez, BuildingTemplate.Wall,
            BuildingTemplate.MajestatniSocha})
            {
                ConstructionOption option = new ConstructionOption("Postavit budovu " + buildingTemplate.Name, buildingTemplate.Description,
                    (b, s) =>
                    {
                        s.SelectedBuildingToPlace = buildingTemplate;
                        s.StartedBuildingOnThisTile = null;
                    }, buildingTemplate.FoodCost, buildingTemplate.WoodCost, buildingTemplate.ClayCost, 0, buildingTemplate.Icon);
                PracantOptions.Add(option);
                if (buildingTemplate.Id == BuildingId.Tent || buildingTemplate.Id == BuildingId.Hadrakovez)
                {
                    RadeninOptions.Add(option);
                }
            }
            foreach(UnitTemplate unitTemplate in new[] {  UnitTemplate.Pracant, UnitTemplate.Hadrakostrelec, UnitTemplate.Katapult })
            {
                var option = new ConstructionOption("Nabrat jednotku " + unitTemplate.Name, unitTemplate.Description,
                    (b, s) =>
                    {
                        b.EnqueueConstruction(unitTemplate);
                    }, unitTemplate.FoodCost, unitTemplate.WoodCost, unitTemplate.MudCost, 1, unitTemplate.Icon);
                if (unitTemplate.Id == UnitId.Pracant)
                {
                    KitchenOptions.Add(option);
                }
                if (unitTemplate.Id == UnitId.Hadrakostrelec)
                {
                    KitchenOptions.Add(option);
                    MunitionTentOptions.Add(option);
                }
                if (unitTemplate.Id == UnitId.Katapult)
                {
                    DrevarskyKoutOptions.Add(option);
                }
            }
        }

        private ConstructionOption(string caption, string description, Action<Building, Selection> whatDo, int foodCost, int woodCost, int mudCost, int populationCost, TextureName icon)
        {
            this.foodCost = foodCost;
            this.populationCost = populationCost;
            this.woodCost = woodCost;
            this.mudCost = mudCost;
            OnClick = whatDo;
            Description = description;
            this.caption = caption;
            Icon = icon;
        }

        private int foodCost;
        private int mudCost;
        private int woodCost;
        private int populationCost;

        public int FoodCost => foodCost;

        public int WoodCost => woodCost;

        public int ClayCost => mudCost;

        public int PopulationCost => populationCost;
        private string caption;
        public string TooltipCaption => caption + " (" + GetResourceCost() + ")";

        private string GetResourceCost()
        {
            string s = "";
            if (foodCost > 0)
            {
                s += foodCost + " jídla";
                if (woodCost > 0 || mudCost > 0)
                {
                    s += ", ";
                }
            }
            if (woodCost > 0)
            {
                s += woodCost + " dřeva";
                if (mudCost > 0)
                {
                    s += ", ";
                }
            }
            if (mudCost > 0)
            {
                s += mudCost + " turbojílu";
            }
            return s;
        }

        public string Description { get;  }

        internal bool AffordableBy(Troop controller)
        {
            return controller.Wood >= this.woodCost && controller.Food >= this.foodCost && 
                controller.Clay >= this.mudCost &&
                (this.populationCost == 0 || (controller.PopulationLimit - controller.PopulationUsed) >= this.populationCost);
        }

        public Action<Building, Selection> OnClick;
        public TextureName Icon;
    }
}