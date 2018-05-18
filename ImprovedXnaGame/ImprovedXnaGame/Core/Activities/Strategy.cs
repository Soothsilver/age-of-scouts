using System;
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
        public NaturalObject GatherTarget;

        private Unit owner;

        public Strategy(Unit owner)
        {
            this.owner = owner;
        }

        public void ResetTo(Vector2 movementTarget)
        {
            FullStop();
            owner.Tactics.MovementTarget = (IntVector)movementTarget;
        }
        public void ResetTo(Building construction)
        {
            FullStop();
            if (owner.CanContributeToBuilding(construction))
            {
                owner.Tactics.BuildTarget = construction;
            }
            else
            {
                owner.Tactics.MovementTarget = (IntVector)construction.FeetStdPosition;
            }
        }
        public void ResetTo(Unit attackTarget)
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
        }        
    }
}
