using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Age.Core
{
    class Activity
    {
        public Vector2 MovementTarget;
        public bool HasAGoal => MovementTarget != Vector2.Zero || AttackTarget != null;
        public Vector2 Speed;
        public float SecondsUntilNextRecalculation;
        public float SecondsUntilRecharge;
        public bool AttackingInProgress;
        public List<Vector2> PathingCoordinates = new List<Vector2>();
        public Unit AttackTarget;
    }
}