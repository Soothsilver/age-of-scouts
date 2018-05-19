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
            if (skippedUpdates < 20)
            {
                skippedUpdates++;
                return;
            }
            skippedUpdates = 0;

            // Now act.

            // Without kitchen, do nothing.
            Building myKitchen = session.AllBuildings.FirstOrDefault(bld => bld.Controller == Self && bld.Template.Id == BuildingId.Kitchen);
            if (myKitchen == null)
            {
                Self.Surrender();
                return;
            }

            // Order Kitchen:
            if (myKitchen.ConstructionQueue.Count == 0)
            {
                myKitchen.EnqueueConstruction(UnitTemplate.Pracant);
            }
            // Order Pracants:
            for (int unitIndex = 0; unitIndex < session.AllUnits.Count; unitIndex++)
            {
                Unit unit = session.AllUnits[unitIndex];
                if (unit.UnitTemplate.CanBuildStuff && unit.Controller == Self && unit.FullyIdle)
                {
                    AssignWorkToPracant(unit, session, myKitchen);
                }
            }
        }

        private void AssignWorkToPracant(Unit unit, Session session, Building myKitchen)
        {
            // Build tents 
            unit.AttemptToExitConstructionSite(myKitchen);
        }
    }
}
