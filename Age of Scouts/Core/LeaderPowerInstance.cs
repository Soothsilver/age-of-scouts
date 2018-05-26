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
            // Tisik Spy
            this.Used = true;
            session.Revealers.Add(new FogRevealer(where, Tile.HEIGHT * 6, fromAir: true, cleartime: 7));
        }
    }
}