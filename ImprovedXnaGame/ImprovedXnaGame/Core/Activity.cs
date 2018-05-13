using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Age.Core
{
    class Activity
    {
        public Vector2 MovementTarget;
        public bool HasAGoal => MovementTarget != Vector2.Zero || AttackTarget != null;

        public bool IsMoving => Speed != Vector2.Zero;

        public Vector2 Speed;
        public float SecondsUntilNextRecalculation;
        public float SecondsUntilRecharge;
        public bool AttackingInProgress;
        public LinkedList<Vector2> PathingCoordinates = new LinkedList<Vector2>();
        public Unit AttackTarget;
        public NaturalObject MiningTarget;
        public Building ConstructionTarget;

        internal void Reset()
        {
            this.ConstructionTarget = null;
            this.MiningTarget = null;
            this.SecondsUntilNextRecalculation = 0;
            this.AttackTarget = null;
            this.MovementTarget = Vector2.Zero;
        }
    }
}