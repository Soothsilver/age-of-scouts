using Age.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Age.AI
{
    class Subroutines
    {
        public static NaturalObject FindClosestGatherSpotGatherableBy(Building building)
        {
            if (building.Template.Id == BuildingId.Kitchen)
            {
                // Do not gather automatically after builiding a Kitchen.
                return null;
            }
            List<Resource> legalResources = new List<Resource>();
            foreach(Resource resource in StaticData.AllResources)
            {
                if (building.Template.CanAcceptResource(resource))
                {
                    legalResources.Add(resource);
                }
            }
            if (legalResources.Count == 0)
            {
                // This building does not gather resources.
                return null;
            }
            foreach(Tile tile in Subroutines.SweepTilesAroundTile(building.PrimaryTile))
            {
                if (tile.NaturalObjectOccupant != null && tile.NaturalObjectOccupant.ResourcesLeft > 0 &&
                    legalResources.Contains(tile.NaturalObjectOccupant.ProvidesResource))
                {
                    return tile.NaturalObjectOccupant;
                }
            }
            return null;
        }

        private static IEnumerable<Tile> SweepTilesAroundTile(Tile primaryTile)
        {
            for (int round = 1; round < 10; round++)
            {

            }
            yield break;
        }
    }
}
