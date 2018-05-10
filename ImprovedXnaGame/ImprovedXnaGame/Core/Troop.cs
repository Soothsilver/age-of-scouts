using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Age.Core
{
    class Troop
    {
        private Session session;
        public Troop(string name, Session session, Era era, Color strongColor, Color lightColor)
        {
            this.Name = name;
            this.session = session;
            this.Era = era;
            this.StrongColor = strongColor;
            this.LightColor = lightColor;
            this.LeaderPowers.Add(new LeaderPowerInstance(LeaderPower.CreateSpy(), false));
        }

        public string Name { get; set; }
        public Color StrongColor { get; set; }
        public Color LightColor { get; set; }
        public Era Era { get; set; }
        public Gender Gender => Gender.Boy;
        public List<LeaderPowerInstance> LeaderPowers = new List<LeaderPowerInstance>();
        internal bool Convertible;

        public int Food { get; set; } = 100;
        public int Wood { get; set; } = 100;
        public int Clay { get; set; } = 100;
        public int PopulationUsed => session.AllUnits.Count(unt => unt.Controller == this);
        public int PopulationLimit => 0;

        internal void Surrender()
        {
            foreach(var unit in session.AllUnits.Where(unt => unt.Controller == this))
            {
                unit.HP = 0;
            }
        }
    }
}