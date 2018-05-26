using Age.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Pathfinding
{
    class MovementRules
    {
        public static MoveLegality IsMoveLegal(Unit unit, Vector2 from, Vector2 to, Tile targetTile, Session session)
        {
            Rectangle oldHitbox = unit.GetHitboxFromFeet(from);
            Rectangle newHitbox = unit.GetHitboxFromFeet(to);
            if (targetTile == null)
            {
                return MoveLegality.UnmovableIllegal;
            }
            if (targetTile.PreventsMovement)
            {
                return MoveLegality.UnmovableIllegal;
            }
            foreach(var obstacle in session.AllUnits)
            {
                if (obstacle != unit && obstacle.Hitbox.Intersects(newHitbox))
                {
                    if (!obstacle.Hitbox.Intersects(oldHitbox))
                    {
                        return new MoveLegality()
                        {
                            IsLegal = false,
                            ObstacleUnit = obstacle
                        };
                    }
                }
            }
            return MoveLegality.Legal;
        }
        
    }
    class MoveLegality
    {
        public static MoveLegality Legal = new MoveLegality() { IsLegal = true };
        public static MoveLegality UnmovableIllegal = new MoveLegality() { IsLegal = false, ObstacleUnit = null };
        public bool IsLegal;
        public Unit ObstacleUnit;
    }
}
