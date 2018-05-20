using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Core;

namespace Age.AI
{
    class AggressiveAI : BaseAI
    {
        int skippedUpdates = 0;

        public AggressiveAI(Troop self) : base(self)
        {
        }

        public override void Update(Session session)
        {
            // TODO skip some updates somehow
            // Now act.
            // Without kitchen, do nothing.
            Building myKitchen = session.AllBuildings.FirstOrDefault(bld => bld.Controller == Self && bld.Template.Id == BuildingId.Kitchen);
            if (myKitchen == null)
            {
                Self.Surrender();
                return;
            }
            // Take stock
            int woodcutters = 0;
            int foodgatherers = 0;
            int villagers = 0;
            int soldiers = 0;
            int idleSoldiers = 0;
            int population = Self.PopulationUsed;
            int limit = Self.PopulationLimit;

            // Need a tent?
            bool wantATent = limit - population < 5;
            bool canHaveATent = BuildingTemplate.Tent.AffordableBy(Self);

            // Take stock.
            for (int unitIndex = 0; unitIndex < session.AllUnits.Count; unitIndex++)
            {
                Unit unit = session.AllUnits[unitIndex];
                if (unit.Controller == Self)
                {
                    if (unit.UnitTemplate.CanBuildStuff)
                    {
                        if (unit.Strategy.GatherTarget != null)
                        {
                            if (unit.Strategy.GatherTarget.ProvidesResource == Resource.Wood) woodcutters++;
                            if (unit.Strategy.GatherTarget.ProvidesResource == Resource.Food) foodgatherers++;
                        }
                        villagers++;
                    }
                    if (unit.UnitTemplate.CanAttack)
                    {
                        soldiers++;
                        if (unit.FullyIdle)
                        {
                            idleSoldiers++;
                        }
                    }
                }
            }

            if (wantATent && canHaveATent)
            {
                // Order someone to make a tent. If possible, use an idle village for that.
                Unit unit = FindSomeoneToWorkOnBuilding(session);
                if (unit != null)
                {
                    Tile whereTo = WhereToPlaceBuilding(unit, session, BuildingTemplate.Tent);
                    if (whereTo != null && BuildingTemplate.Tent.ApplyCost(Self))
                    {
                        Building newTent = session.SpawnBuildingAsConstruction(BuildingTemplate.Tent, Self, whereTo);
                        unit.Strategy.ResetTo(newTent);
                        return;
                    }
                }
            }

            // Order Kitchen:
            if (myKitchen.ConstructionInProgress == null && myKitchen.ConstructionQueue.Count == 0)
            {
                if (villagers < soldiers)
                {
                    myKitchen.EnqueueConstruction(UnitTemplate.Pracant);
                }
                else
                {
                    myKitchen.EnqueueConstruction(UnitTemplate.Hadrakostrelec);
                }
            }

            // Order Pracants to gather:
            for (int unitIndex = 0; unitIndex < session.AllUnits.Count; unitIndex++)
            {
                Unit unit = session.AllUnits[unitIndex];
                if (unit.UnitTemplate.CanBuildStuff && unit.Controller == Self && unit.FullyIdle)
                {
                    // Go gather that of which we have less
                    if (foodgatherers < woodcutters)
                    {
                        SendGather(unit, session, Resource.Food);
                        foodgatherers++;
                    }
                    else
                    {
                        SendGather(unit, session, Resource.Wood);
                        woodcutters++;
                    }
                }
            }

            // Order soldiers to fight:
            if (idleSoldiers >=  10)
            {
                var targets = session.AllUnits.Where(unt => unt.Controller != Self).ToList();
                if (targets.Count > 0)
                {
                    var target = targets[R.Next(targets.Count)];
                    foreach(var unt in session.AllUnits.Where(unt => unt.CanAttack && unt.FullyIdle && unt.Controller == Self))
                    {
                        unt.Strategy.ResetTo(target);
                    }
                }
            }
        }

        private Unit FindSomeoneToWorkOnBuilding(Session session)
        {
            Unit bestNonIdle = null; 
            foreach(var unt in session.AllUnits.Where(unt => unt.UnitTemplate.CanBuildStuff && unt.Controller == Self))
            {
                if (unt.FullyIdle)
                {
                    return unt;
                }
                if (!unt.Strategy.BuildingStuff)
                {
                    bestNonIdle = unt;
                }
            }
            return bestNonIdle;
        }

        private void SendGather(Unit unit, Session session, Resource gatherWhat)
        {
            NaturalObject closestTile = null;
            int closestManhattan = Int32.MaxValue;
            for (int x = 0; x < session.Map.Width; x++)
            {
                for (int y= 0; y< session.Map.Height; y++)
                {
                    NaturalObject here = session.Map.Tiles[x, y].NaturalObjectOccupant;
                    if (here?.ProvidesResource == gatherWhat)
                    {
                        int toHere = Math.Abs(x - unit.Occupies.X) + Math.Abs(y - unit.Occupies.Y);
                        if (toHere < closestManhattan)
                        {
                            closestManhattan = toHere;
                            closestTile = here;
                        }
                    }
                }
            }
            if (closestTile != null)
            {
                unit.Strategy.ResetTo(closestTile);
            }
        }

        private Tile WhereToPlaceBuilding(Unit unit, Session session, BuildingTemplate buildingToPlace)
        {
            for (int i = 0; i < 50; i++)
            {
                int x = R.Next(-7, 7);
                int y = R.Next(-7, 7);
                Tile tile = session.Map.GetTileFromTileCoordinates(unit.Occupies.X + x, unit.Occupies.Y + y);
                if (tile != null && buildingToPlace.PlaceableOn(session, tile))
                {
                    return tile;
                }
            }
            return null;
        }
    }
}
