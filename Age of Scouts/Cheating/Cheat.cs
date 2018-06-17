using Age.Core;
using Age.Phases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Age.Cheating
{
    class Cheat
    {
        public string Command { get; }
        public string Description { get; }
        public Action<LevelPhase, Session> SoWhat { get; }

        public Cheat(string command, string description, Action<LevelPhase, Session> soWhat)
        {
            Command = command;
            this.Description = description;
            this.SoWhat = soWhat;
        }

        public static List<Cheat> Cheats = new List<Cheat>();

        static Cheat()
        {
            Cheats.Add(new Cheat("marco", "vypnout mlhu války", (l, s) => Settings.Instance.EnableFogOfWar = !Settings.Instance.EnableFogOfWar));
            Cheats.Add(new Cheat("polo", "vidět za všechny nepřátele", (l, s) => s.PlayerTroop.Omniscience = !s.PlayerTroop.Omniscience));
            Cheats.Add(new Cheat("aegis", "rychlé stavění", (l, s) => {
                Settings.Instance.Aegis = !Settings.Instance.Aegis;
                SFX.PlaySoundUnlessPlaying(Auxiliary.SoundEffectName.Harp);
            }));
            Cheats.Add(new Cheat("i r winner", "vítězství", (l, s) => s.AchieveEnding(Ending.Victory)));
            Cheats.Add(new Cheat("speedy", "zrychlí hru 2x", (l, s) => Settings.Instance.TimeFactor *= 2));
            Cheats.Add(new Cheat("slowly", "zpomalí hru 2x", (l, s) => Settings.Instance.TimeFactor /= 2));
            Cheats.Add(new Cheat("give me stuff", "dostaneš +1000 od každé suroviny", (l, s) => {
                s.PlayerTroop.Food += 1000;
                s.PlayerTroop.Clay += 1000;
                s.PlayerTroop.Wood += 1000;
                SFX.PlaySoundUnlessPlaying(Auxiliary.SoundEffectName.Harp);
            }));
            Cheats.Add(new Cheat("pandoras box", "dostaneš čtyři náhodné vůdcovské schopnosti", (l, s) => {
                s.PlayerTroop.LeaderPowers = new System.Collections.Generic.List<LeaderPowerInstance>
                {
                    new LeaderPowerInstance(LeaderPower.CreateSpy(), false),
                    new LeaderPowerInstance(LeaderPower.CreateSpy(), false),
                    new LeaderPowerInstance(LeaderPower.CreateArtillery(), false),
                    new LeaderPowerInstance(LeaderPower.CreateArtillery(), false)
                };
                SFX.PlaySoundUnlessPlaying(Auxiliary.SoundEffectName.Harp);
            }));
            Cheats.Add(new Cheat("halt and catch fire", "hra spadne a ukončí se", (l, s) => {               
                SFX.PlaySoundUnlessPlaying(Auxiliary.SoundEffectName.Harp);
                throw new HaltAndCatchFireException("Hra spadla, protože hráč použil cheat kód HCF.");
            }));
            Cheats.Add(new Cheat("mikes", "tvoje jednotky a budovy jsou nezničitelné", (l, s) => {
                SFX.PlaySoundUnlessPlaying(Auxiliary.SoundEffectName.Harp);
                s.PlayerTroop.Invincible = !s.PlayerTroop.Invincible;
            }));
        }

        private class HaltAndCatchFireException : Exception
        {
            public HaltAndCatchFireException(string msg)
                : base(msg)
            {
            }
        }
    }
}
