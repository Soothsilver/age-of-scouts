using System;
using Microsoft.Xna.Framework;

namespace Age.Core
{
    public class LeaderPowerInstance
    {
        public LeaderPowerInstance(LeaderPower power, bool usedUp)
        {
            this.Used = usedUp;
            this.Power = power;
        }

        public LeaderPower Power { get; set; }
        public bool Used { get; set; }

        internal void Use(Vector2 where, Session session)
        {
            this.Used = true;
            // Tisik Spy
            FogOfWarMechanics.RevealFogOfWar(where, Tile.WIDTH * 7, session.Map, false, cleartime: 7, fromAir: true);
        }
    }
}