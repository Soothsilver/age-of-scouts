using Age.World;
using Microsoft.Xna.Framework;

namespace Age.Core.Activities
{
    abstract class Goals
    {
        public NaturalObject ApproachObject;
        public IntVector MovementTarget;
        public Unit AttackTarget;
        public Building BuildingTarget;


        public virtual bool Idle => ApproachObject == null && MovementTarget == IntVector.Zero && AttackTarget == null && BuildingTarget == null;
        public virtual void Reset()
        {
            this.ApproachObject = null;
            this.MovementTarget = IntVector.Zero;
            this.AttackTarget = null;
        }
    }
}