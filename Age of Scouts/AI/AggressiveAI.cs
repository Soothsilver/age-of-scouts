using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Core;
using Age.World;

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
            int limit = session.AllBuildings.Count(bld => bld.Template.Id == BuildingId.Tent && bld.Controller == Self) * 2; // including buildings that are in progress

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
                var targets = session.AllUnits.Where(unt => session.AreEnemies(myKitchen, unt)).Cast<AttackableEntity>()
                    .Concat(session.AllBuildings.Where(bld => session.AreEnemies(myKitchen, bld))).ToList();
                if (targets.Count > 0)
                {
                    var target = targets[R.Next(targets.Count)];
                    foreach(var unt in session.AllUnits.Where(unt => unt.CanAttack && unt.FullyIdle && unt.Controller == Self))
                    {
                        unt.Strategy.ResetToAttackMove((IntVector)target.FeetStdPosition);
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
            List<Tile> options = new List<Tile>();
            foreach (var building in session.AllBuildings.Where(bld => bld.Controller == Self))
            {
                int x1 = building.PrimaryTile.X - building.Template.TileWidth - 1;
                int x2 = building.PrimaryTile.X + 2;
                int y1 = building.PrimaryTile.Y - building.Template.TileHeight - 1;
                int y2 = building.PrimaryTile.Y + 2;

                for (int x = x1; x <= x2; x++)
                {
                    for (int y = y1; y <= y2; y++)
                    {
                        if (x == x1 || x == x2 || y == y1 || y == y2)
                        {
                            Tile tile = session.Map.GetTileFromTileCoordinates(x, y);
                            if (tile != null && buildingToPlace.PlaceableOn(session, tile, true))
                            {
                                if (tile.Neighbours.All.All(tl =>
                                    tl.BuildingOccupant == null && tl.NaturalObjectOccupant == null))
                                {
                                    options.Add(tile);
                                }
                            }
                        }
                    }
                }
            }

            if (options.Count > 0)
            {
                return options[R.Next(options.Count)];
            }
            return null;
        }
    }
}
