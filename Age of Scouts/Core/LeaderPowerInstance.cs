using System;
using Microsoft.Xna.Framework;

namespace Age.Core
{
    class LeaderPowerInstance
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
            switch(Power.ID)
            {
                case LeaderPowerID.Spy:
                    session.Revealers.Add(new FogRevealer(where, 6, fromAir: true, cleartime: 7, onceOnly: true));
                    break;
                case LeaderPowerID.Artillery:
                    Unit outsideEntity = new Unit("Spojenec", Troop.Pseudotroop, UnitTemplate.Hadrakostrelec, Vector2.Zero);
                    for (int i =0; i< 30; i++)
                    {
                        Vector2 endPosition = where + new Vector2(R.FloatAroundZero() * Tile.WIDTH * 2, R.FloatAroundZero() * Tile.HEIGHT * 2);
                        Vector2 startingPosition = endPosition + new Vector2(-4 * Tile.WIDTH, -4 * Tile.HEIGHT);
                        Projectile p = new Projectile(startingPosition, endPosition, outsideEntity);
                        session.SpawnProjectile(p);
                    }
                    break;
            }
            this.Used = true;
        }
    }
}