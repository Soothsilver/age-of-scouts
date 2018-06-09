﻿using Age.AI;
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
            this.AI = new DoNothingAI(this);
        }

        public string Name { get; set; }
        public Color StrongColor { get; set; }
        public Color LightColor { get; set; }
        public Era Era { get; set; }
        public Gender Gender => Gender.Boy;
        public List<LeaderPowerInstance> LeaderPowers = new List<LeaderPowerInstance>();
        internal bool Convertible;
        public List<Troop> Allies = new List<Troop>();

        public int Food { get; set; } = 1000;
        public int Wood { get; set; } = 1000;
        public int Clay { get; set; } = 1000;
        public int PopulationUsed => Session.AllUnits.Count(unt => unt.Controller == this);
        public int PopulationLimit => Session.AllBuildings.Count(bld => bld.Template.Id == BuildingId.Tent && !bld.SelfConstructionInProgress && bld.Controller == this) * 2;

        public static Troop Pseudotroop { get; internal set; } = new Troop("Gaia", null, Era.EraNacelniku, Color.Black, Color.Black);
        public BaseAI AI;
        internal bool Omniscience;

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
                unit.TakeDamage((int)unit.HP, unit);
            }
            Session.AllBuildings.Where(bld => bld.Controller == this).ToList().ForEach(
                bld => Session.DestroyBuilding(bld));
        }

        public void WinTheGame()
        {
            if (this == Session.PlayerTroop)
            {
                Session.AchieveEnding(Ending.Victory);
            }
            else
            {
                Session.AchieveEnding(Ending.Defeat);
            }
        }

        internal bool IsAlliedWith(Troop anotherPlayer)
        {
            return Allies.Contains(anotherPlayer);
        }
    }
}