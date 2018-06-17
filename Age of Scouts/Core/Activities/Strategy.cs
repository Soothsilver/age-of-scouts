﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.World;
using Microsoft.Xna.Framework;

namespace Age.Core.Activities
{
    /// <summary>
    /// Represents a long-term goal of this unit. Strategy is set exclusively by right-clicking something on the map, or with a rally point,
    /// or by an AI-controlled player.
    /// </summary>
    class Strategy
    {
        public IntVector AttackMoveTarget;
        public NaturalObject GatherTarget;
        public bool BuildingStuff;

        private Unit owner;

        public bool DoesStrategyExist => BuildingStuff || GatherTarget != null || AttackMoveTarget != IntVector.Zero;

        public bool HasNoStrategy => AttackMoveTarget == IntVector.Zero && GatherTarget == null && !BuildingStuff;

        public Strategy(Unit owner)
        {
            this.owner = owner;
        }

        public void ResetTo(Vector2 movementTarget)
        {
            FullStop();
            owner.Tactics.ResetToMovement((IntVector) movementTarget, false);
        }
        public void ResetTo(NaturalObject gatherTarget)
        {
            FullStop();
            GatherTarget = gatherTarget;
        }
        public void ResetTo(Building construction)
        {
            FullStop();
            if (owner.CanContributeToBuilding(construction))
            {
                owner.Tactics.BuildTarget = construction;
                BuildingStuff = true;
            }
            else if (construction.CanAcceptResourcesFrom(owner))
            {
                owner.Tactics.BuildTarget = construction;
            }
            else
            {
                owner.Tactics.ResetToMovement((IntVector)construction.FeetStdPosition, false);
            }
        }
        public void ResetToAttack(AttackableEntity attackTarget)
        {
            FullStop();
            owner.Tactics.ResetTo(attackTarget, false);
        }

        /// <summary>
        /// Fully stops the unit and destroys its strategy and tactics.
        /// </summary>
        private void FullStop()
        {
            Reset();
            owner.Tactics.Reset();
            owner.Activity.ResetActions();
            owner.Activity.Invalidate();
        }
        public void Reset()
        {
            GatherTarget = null;
            AttackMoveTarget = IntVector.Zero;
            BuildingStuff = false;
        }

        internal void DetermineNextTactics()
        {
            if (BuildingStuff)
            {
                var buildables = owner.Session.AllBuildings.Where(bld => 
                                                                            owner.CanContributeToBuilding(bld) &&
                                                                            bld.FeetStdPosition.WithinDistance(
                                                                                owner.FeetStdPosition, Tile.WIDTH * 8));
                Building buildableNext = null;
                float bestDistance = int.MaxValue;
                foreach (var building in buildables)
                {
                    float dst = (building.FeetStdPosition - owner.FeetStdPosition).LengthSquared();
                    if (dst < bestDistance)
                    {
                        bestDistance = dst;
                        buildableNext = building;
                    }
                }
                if (buildableNext == null)
                {
                    // End of strategy, there's nothing to do here.
                    BuildingStuff = false;
                }
                else
                {
                    ResetTo(buildableNext);
                }
            }
            else if (GatherTarget != null)
            {
                if (owner.CarryingResource != GatherTarget.ProvidesResource || owner.CarryingHowMuchResource < StaticData.CarryingCapacity)
                {
                    // Go gather.
                    if (GatherTarget.ResourcesLeft > 0)
                    {
                        owner.Tactics.GatherTarget = GatherTarget;
                    }
                    else
                    {
                        var adjacents = GatherTarget.Occupies.Neighbours.All.Where(tl => tl.NaturalObjectOccupant != null && tl.NaturalObjectOccupant.ProvidesResource == GatherTarget.ProvidesResource).Select(tl => tl.NaturalObjectOccupant).ToList();
                        if (adjacents.Count > 0)
                        {                  
                            // Gather from something close by.
                            GatherTarget = owner.Tactics.GatherTarget = adjacents[R.Next(adjacents.Count)];
                        }
                        else
                        {
                            // End of strategy.
                            GatherTarget = null;
                        }
                    }
                }
                else
                {
                    // Go deposit.
                    Building closestDropOffPoint = null;
                    float distanceToClosestPoint = 0;
                    foreach(var building in owner.Session.AllBuildings)
                    {
                        if (building.CanAcceptResourcesFrom(owner))
                        {
                            var distanceToThis = (building.FeetStdPosition - owner.FeetStdPosition).LengthSquared();
                            if (closestDropOffPoint == null)
                            {
                                closestDropOffPoint = building;
                                distanceToClosestPoint = distanceToThis;
                            }
                            else if (distanceToThis < distanceToClosestPoint)
                            {
                                distanceToClosestPoint = distanceToThis;
                                closestDropOffPoint = building;
                            }
                        }
                    }
                    if (closestDropOffPoint != null)
                    {
                        owner.Tactics.BuildTarget = closestDropOffPoint;
                    }
                }
            }
            else if (AttackMoveTarget != IntVector.Zero)
            {
                owner.Tactics.ResetToMovement(AttackMoveTarget, true);
            }
        }
        public override string ToString()
        {
            if (GatherTarget != null)
            {
                return "Gather from: " + GatherTarget;
            }
            else if (BuildingStuff)
            {
                return "Building stuff";
            }
            else if (AttackMoveTarget != IntVector.Zero)
            {
                return "Attack move";
            }
            else if (DoesStrategyExist)
            {
                return "UNIDENTIFIED STRATEGY";
            }
            else
            {
                return "None";
            }
        }

        public void ResetToAttackMove(IntVector standardTarget)
        {
            FullStop();
            AttackMoveTarget = standardTarget;
        }

        /// <summary>
        /// Returns a natural resource with the same resource as the blocked resource and as close as possible that has an empty gatherer spot and is reachable by this unit.
        /// If none exists, this returns null.
        /// </summary>
        /// <param name="blockedResource">A gathering spot that this unit cannot access because the path is blocked, probably by other units.</param>
        internal NaturalObject FindSpotWhichYouCanGatherIfAble(NaturalObject blockedResource)
        {
            var allOptions = new HashSet<Vector2>();
            owner.Session.Map.ForEachTile((x, y, tile) =>
            {
                // PERFORMANCE!
                if (tile.NaturalObjectOccupant != null && tile.NaturalObjectOccupant.ProvidesResource == blockedResource.ProvidesResource &&
                tile.NaturalObjectOccupant.ResourcesLeft > 0)
                {
                    foreach (var neighbour in tile.Neighbours.All)
                    {
                        if (neighbour.Occupants.Count == 0)
                        {
                            allOptions.Add(Isomath.TileToStandard(neighbour.X + 0.5f, neighbour.Y + 0.5f));
                        }
                    }
                }
            });
            var bestPath = Pathfinding.Pathfinding.DijkstraMultiple(this.owner, allOptions, owner.Session.Map);
            if (bestPath == null)
            {
                return null;
            }
            Vector2 bestSpot = bestPath.Last.Value;
            Tile bestSpotTile = owner.Session.Map.GetTileFromStandardCoordinates(bestSpot);
            foreach(var tile in bestSpotTile.Neighbours.All)
            {
                if (tile.NaturalObjectOccupant?.ProvidesResource == blockedResource.ProvidesResource)
                {
                    return tile.NaturalObjectOccupant;
                }
            }
            return null; // This should not happen....

        }
    }
}
