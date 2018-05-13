using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Age.Core
{
    class Troop
    {
        public Session Session;
        public Troop(string name, Session session, Era era, Color strongColor, Color lightColor)
        {
            this.Name = name;
            this.Session = session;
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

        public int Food { get; set; } = 1000;
        public int Wood { get; set; } = 1000;
        public int Clay { get; set; } = 1000;
        public int PopulationUsed => Session.AllUnits.Count(unt => unt.Controller == this);
        public int PopulationLimit => Session.AllBuildings.Count(bld => bld.Template.Id == BuildingId.Tent && bld.Controller == this) * 2;

        public static Troop Gaia { get; internal set; }

        public int GetResourceStore(Resource resource)
        {
            switch (resource)
            {
                case Resource.Clay: return Clay;
                case Resource.Wood: return Wood;
                case Resource.Food: return Food;
            }
            throw new Exception("This resource does not exist.");
        }

        internal void Surrender()
        {
            foreach(var unit in Session.AllUnits.Where(unt => unt.Controller == this))
            {
                unit.HP = 0;
            }
        }
    }
}