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
    class Strategy : Goals
    {
        public NaturalObject GatherTarget;

        private Unit owner;

        public Strategy(Unit owner)
        {
            this.owner = owner;
        }

        public void ResetTo(Vector2 movementTarget)
        {
            ResetAll();
            MovementTarget = (IntVector)movementTarget;
        }
        public void ResetTo(Building construction)
        {
            ResetAll();
            BuildingTarget = construction;
        }
        public void ResetTo(Unit attackTarget)
        {
            ResetAll();
            AttackTarget = attackTarget;
        }

        private void ResetAll()
        {
            Reset();
            owner.Tactics.Reset();
            owner.Activity.Reset();
        }
        public override void Reset()
        {
            base.Reset();
            GatherTarget = null;
        }

        public override bool Idle => base.Idle && this.GatherTarget != null;

        public void RecalculateAndDetermineTactics()
        {
            owner.Tactics.Reset();
            if (AttackTarget != null)
            {
                owner.Tactics.AttackTarget = AttackTarget;
            }
            else if (MovementTarget != IntVector.Zero)
            {
                owner.Tactics.MovementTarget = MovementTarget;
            }
            else if (BuildingTarget != null)
            {
                owner.Tactics.MovementTarget = (IntVector)BuildingTarget.FeetStdPosition;
            }
        }
    }
}
