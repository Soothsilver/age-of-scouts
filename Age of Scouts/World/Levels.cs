using Age.AI;
using Age.Core;
using Age.Voice;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.World
{
    class Levels
    {
        internal static Session Tutorial()
        {
            Session s = CreateBasic1v1();
            LoadMapIntoSession(s, "Levels\\Prepad.tmx");
            float FLAG_RANGE = 1.5f;
            Objective o4 = new Objective("Vyřaď ze hry 'Mravence', vedoucího nepřátelského tábora.")
            {
                Visible = false,
                StateTrigger = (levelPhase) => levelPhase.Session.AllUnits.Count(unt => unt.Name == "Mravenec") == 0,
                OnComplete = (session) =>
                {
                    session.AchieveEnding(Ending.Victory);
                }
            };
            Objective o3 = new Objective("Pokračuj po cestě podle instrukcí tutoriálu.")
            {
                Visible = false,
                StateTrigger = (levelPhase) => levelPhase.Session.AllUnits.Count(unt => unt.Controller.Convertible) == 0,
                OnComplete = (session) =>
                {
                    session.EnqueueVoice(new ChatLine("Další skauti ti přišli na pomoc. Přidrž levé tlačítko myši a přejeď s ním přes všechny své skauty, abys je mohl úkolovat všechny najednout.", SoundEffectName.Tut10));
                    o4.Visible = true;
                }
            };
            Objective o2 = new Objective("Dotkni se druhé vlajky.")
            {
                Visible = false,
                StateTrigger = (levelPhase) => levelPhase.Session.AllUnits.Any(unt => unt.FeetStdPosition.WithinDistance(Isomath.TileToStandard(28.5f, 37.5f), Tile.HEIGHT * 3)),
                OnComplete = (session) =>
                {
                    o3.Visible = true;
                    session.EnqueueVoice(new ChatLine("Někteří skauti umí plavat, tento ale ne. Najdi most přes řeku a dojdi k další vlajce.", SoundEffectName.Tut6));
                    session.Revealers.Add(new FogRevealer(Isomath.TileToStandard(new Vector2(36.5f, 13.5f)), FLAG_RANGE));
                }
            };
            s.Objectives.Add(new Objective("Dotkni se první vlajky.")
            {
                StateTrigger = (levelPhase) =>
                levelPhase.Session.AllUnits.Any(unt => unt.FeetStdPosition.WithinDistance(Isomath.TileToStandard(14.5f, 37.5f), Tile.HEIGHT * 3)),
                OnComplete = (session) =>
                {
                    o2.Visible = true;
                    session.EnqueueVoice(new ChatLine("Hustým lesem se v Age of Scouts nedá procházet. Obejdi les a dojdi až k další vlajce.", SoundEffectName.Tut5));
                    session.Revealers.Add(new FogRevealer(Isomath.TileToStandard(new Vector2(28.5f, 37.5f)), FLAG_RANGE));
                }
            });
            s.Objectives.Add(o2);
            s.Objectives.Add(o3);
            s.Objectives.Add(o4);
            s.OtherTriggers.Add(new Trigger()
            {
                StateTrigger = (levelPhase) =>
                {
                    return levelPhase.Selection.SelectedUnits.Count(unt => unt.Controller == levelPhase.Session.PlayerTroop) >= 1;
                },
                OnComplete = (session) =>
                {
                    session.EnqueueVoice(new ChatLine("Výborně. Nyní klikni pravým tlačítkem myši na nedalekou vlajku a skaut se tam přesune.", SoundEffectName.Tut4));
                }
            });
            s.OtherTriggers.Add(new Trigger()
            {
                StateTrigger = (levelPhase) =>
                {
                    return levelPhase.Selection.SelectedUnits.Count(unt => unt.Controller == levelPhase.Session.PlayerTroop) >= 2 && levelPhase.Session.AllUnits.Count(unt => unt.Controller.Convertible) == 0;
                },
                OnComplete = (session) =>
                {
                    session.EnqueueVoice(new ChatLine("Výborně. Nyní s nimi dojdi až do nepřátelského tábora a zneškodni nepřátelské skauty.", SoundEffectName.Tut11));
                }
            });


            s.OtherTriggers.Add(new Trigger()
            {
                StateTrigger = (levelPhase) => levelPhase.Session.AllUnits.Any(unt => unt.FeetStdPosition.WithinDistance(Isomath.TileToStandard(36.5f, 13.5f), Tile.HEIGHT * 3)),
                OnComplete = (session) =>
                {
                    session.EnqueueVoice(new ChatLine("Výborně. Pokračuj dál, dokud nenarazíš na pole vysoké trávy.", SoundEffectName.Tut7));
                }
            });

            s.OtherTriggers.Add(new Trigger()
            {
                StateTrigger = (levelPhase) => levelPhase.Session.AllUnits.Any(unt => unt.FeetStdPosition.WithinDistance(Isomath.TileToStandard(3.5f, 2.5f), Tile.HEIGHT * 3)),
                OnComplete = (session) =>
                {
                    session.EnqueueVoice(new ChatLine("Pokud klikneš pravým tlačítkem na nepřítele, tvoji skauti na něj začnou střílet.", SoundEffectName.Tut12));
                }
            });

            s.OtherTriggers.Add(new Trigger()
            {
                StateTrigger = (levelPhase) => levelPhase.Session.AllUnits.Any(unt => unt.FeetStdPosition.WithinDistance(Isomath.TileToStandard(24.5f, 10.5f), Tile.HEIGHT * 3)),
                OnComplete = (session) =>
                {
                    session.EnqueueVoice(new ChatLine("Pozor! Přes tebou hlídá předsunutá hlídka nepřátelského tábora. Dávej pozor, aby tě nespatřila, nebo budeš muset úroveň opakovat.", SoundEffectName.Tut8));
                    session.EnqueueVoice(new ChatLine("Zůstaň ve vysoké trávě a daleko od hlídky, aby tě neviděla, a hlídku obejdi.", SoundEffectName.Tut9));
                    session.Revealers.Add(new FogRevealer(Isomath.TileToStandard(new Vector2(13.5f, 17.5f)), 6));

                }
            });

            s.CenterOfScreenInStandardPixels = new Vector2(0, s.Map.Height * Tile.HEIGHT / 2);
            for (int x = 0; x < 1; x++)
            {
                for (int y = 0; y < 1; y++)
                {
                    s.SpawnUnit(new Unit(NameGenerator.GenerateBoyName(), s.PlayerTroop, UnitTemplate.Hadrakostrelec, (Vector2)Isomath.TileToStandard(4, 36) + new Vector2(x * 40, y * 40))
                    {
                        Stance = Stance.Stealthy
                    });
                }
            }
            var enemyTroop = s.Troops[1];
            var gaia = s.Troops[2];
            s.SpawnUnit(new Unit(NameGenerator.GenerateBoyName(), enemyTroop, UnitTemplate.Hadrakostrelec, Isomath.TileToStandard(13.5f, 18.5f)));
            s.SpawnUnit(new Unit(NameGenerator.GenerateBoyName(), enemyTroop, UnitTemplate.Hadrakostrelec, Isomath.TileToStandard(13.5f, 16.5f)));
            s.SpawnUnit(new Unit("Mravenec", enemyTroop, UnitTemplate.Hadrakostrelec, Isomath.TileToStandard(19.5f, 3.5f)));
            s.SpawnUnit(new Unit(NameGenerator.GenerateBoyName(), gaia, UnitTemplate.Hadrakostrelec, Isomath.TileToStandard(0.5f, 21.5f)));
            s.SpawnUnit(new Unit(NameGenerator.GenerateBoyName(), gaia, UnitTemplate.Hadrakostrelec, Isomath.TileToStandard(0.5f, 22.5f)));
            s.SpawnUnit(new Unit(NameGenerator.GenerateBoyName(), gaia, UnitTemplate.Hadrakostrelec, Isomath.TileToStandard(1.5f, 21.5f)));
            s.SpawnUnit(new Unit(NameGenerator.GenerateBoyName(), gaia, UnitTemplate.Hadrakostrelec, Isomath.TileToStandard(1.5f, 22.5f)));
            s.Revealers.Add(new FogRevealer(Isomath.TileToStandard(14.5f, 37.5f), FLAG_RANGE));
            s.EnqueueVoice(new ChatLine("Ahoj, hráči. Vítej ve hře \"Age of Scouts\". V tomto tutoriálu zkusíš cvičně přepadnout nepřátelský tábor.", SoundEffectName.Tut1));
            s.EnqueueVoice(new ChatLine("Nejprve se ale musíme naučit chodit.", SoundEffectName.Tut2));
            s.EnqueueVoice(new ChatLine("Klikni levým tlačítkem myši na skauta uprostřed obrazovky.", SoundEffectName.Tut3));
            s.LevelName = "Tutorial";
            return s;
        }

     

        internal static Session AIvsAI()
        {
            Session session = new Session();
            session.OtherTriggers.Clear();
            session.Troops.Add(new Troop("Divák", session, Era.EraRadcu, Color.FromNonPremultiplied(4, 44, 204, 255), Color.FromNonPremultiplied(153, 173, 255, 255)));
            Troop enemyTroop = new Troop("9. skautský oddíl Rudý havran", session, Era.EraRadcu, Color.FromNonPremultiplied(221, 0, 0, 255), Color.FromNonPremultiplied(247, 12, 12, 255));
            session.Troops.Add(enemyTroop);
            Troop friendTroop = new Troop("8. skautský oddíl Zelené příšery", session, Era.EraRadcu, Color.FromNonPremultiplied(4,142,15, 255), Color.FromNonPremultiplied(0,255,21, 255));
            session.Troops.Add(friendTroop);
            LoadMapIntoSession(session, "Levels\\BlankMap.tmx");
            var enemy = session.Troops[1];
            enemy.AI = new AggressiveAI(enemy);
            var friend = session.Troops[2];
            friend.AI = new AggressiveAI(friend);
            foreach(var unt in session.AllUnits)
            {
                if (unt.Controller == session.Troops[0])
                {
                    unt.SwitchControllerTo(friend);
                }
            }
            foreach (var unt in session.AllBuildings)
            {
                if (unt.Controller == session.Troops[0])
                {
                    unt.SwitchControllerTo(friend);
                }
            }
            session.Objectives.Add(new Objective("Sleduj hru.")
            {
                Visible = true
            });
            session.ObjectivesChanged = false;
            session.PlayerTroop.Omniscience = true;
            session.LevelName = "AI vs. AI";
            return session;
        }

        private static void LoadMapIntoSession(Session session, string pathFile)
        {
            MapLoader mapLoader = new MapLoader(pathFile);
            mapLoader.LoadInto(session, pathFile);
        }

        private static Session CreateBasic1v1()
        {
            Session session = new Session();
            session.Troops.Add(new Troop("7. skautský oddíl Karibu", session, Era.EraRadcu, Color.FromNonPremultiplied(4, 44, 204, 255), Color.FromNonPremultiplied(153, 173, 255, 255)));
            Troop enemyTroop = new Troop("3. skautský oddíl Cviční nepřátelé", session, Era.EraRadcu, Color.FromNonPremultiplied(221, 0, 0, 255), Color.FromNonPremultiplied(247, 12, 12, 255));
            session.Troops.Add(enemyTroop);
            Troop gaia = new Troop("Ztracení skautíci", session, Era.EraRadcu, Color.FromNonPremultiplied(188, 185, 0, 255), Color.Yellow)
            {
                Convertible = true
            };
            session.Troops.Add(gaia);
            session.LevelName = "Basic 1v1";
            return session;
        }
        internal static Session BlackCastle()
        {
            Session s = new Session
            {
                LevelName = "Raseliniste"
            };
            s.Troops.Add(new Troop("7. skautský oddíl Karibu", s, Era.EraRadcu, Color.FromNonPremultiplied(4, 44, 204, 255), Color.FromNonPremultiplied(153, 173, 255, 255)));
            s.Troops.Add(new Troop("4. skautský oddíl Jaguáři", s, Era.EraRadcu, Color.FromNonPremultiplied(0, 33, 86, 255), Color.FromNonPremultiplied(178,178,178, 255)));
            s.Troops.Add(new Troop("11. skautský oddíl Medvědi", s, Era.EraRadcu, Color.FromNonPremultiplied(221, 0, 0, 255), Color.FromNonPremultiplied(247, 12, 12, 255)));
            s.Troops[1].AI = new AggressiveAI(s.Troops[1]);
            s.Troops[2].AI = new AggressiveAI(s.Troops[2]);
            s.Troops[1].Allies.Add(s.Troops[2]);
            s.Troops[2].Allies.Add(s.Troops[1]);
            LoadMapIntoSession(s, "Levels\\BlackCastle.tmx");
            s.Objectives.Add(new Objective("Znič všechny kuchyně obou nepřátelských oddílů.")
            {
                OnComplete = (ss) => ss.AchieveEnding(Ending.Victory),
                StateTrigger = (ss) => ss.Session.AllBuildings.Count(bld => bld.Controller != ss.Session.PlayerTroop && bld.Template.Id == BuildingId.Kitchen) == 0
            });
            return s;
        }

        internal static Session LevelRadenin()
        {
            Session s = CreateBasic1v1();
            s.LevelName = "Radenin";
            s.Flags.RadeninConstructionOnly = true;
            LoadMapIntoSession(s, "Levels\\Radenin.tmx");
            s.Troops.ForEach(trp =>
            {
                trp.Food = 0;
                trp.Clay = 0;
                trp.Wood = 0;
            });
            Objective oTarget = new Objective("Finální úkol: Znič nepřátelskou věž.")
            {
                OnComplete = (ss) => ss.AchieveEnding(Ending.Victory),
                StateTrigger = (ss)=> ss.Session.AllBuildings.Count(bld => bld.Controller != ss.Session.PlayerTroop) == 0,
                Visible = true
            };
            Objective oDalsiHadrakostrelci = new Objective("Naber 5 hadrákostřelců.")
            {
                Visible = false,
                StateTrigger = (ss)=>ss.Session.AllUnits.Count(unt => unt.UnitTemplate.CanAttack) >= 5,
                OnComplete = (ss) =>
                {
                    s.EnqueueVoice("Někde na této louce se skrývá nepřátelská věž. Tvým posledním úkolem zde je tuto věž najít a zničit.", SoundEffectName.Rad11);
                }
            };
            Objective oDalsiPracanti = new Objective("Naber dva další pracanty.")
            {
                Visible = false,
                StateTrigger = (ss) => ss.Session.AllUnits.Count(unt => unt.UnitTemplate.CanBuildStuff) >= 6,
                OnComplete = (ss) =>
                {
                    s.EnqueueVoice("Nové Pracanty můžeš poslat sbírat jídlo, dřevo nebo stavět další obytné stany.", SoundEffectName.Rad9);
                    s.EnqueueVoice("Tvým dalším úkolem je nabrat pět hadrákostřelců. K tomu budeš muset postavit i další obytné stany.", SoundEffectName.Rad10);
                    oDalsiHadrakostrelci.Visible = true;
                }
            };
            Objective oStan2 = new Objective("Postav obytný stan.")
            {
                Visible = false,
                StateTrigger = (ss)=>ss.Session.PlayerTroop.PopulationLimit > 4,
                OnComplete = (ss) =>
                {
                    s.EnqueueVoice("Nyní můžeš nabrat další Pracanty. Vyber Kuchyni a klikni v dolní liště dvakrát na 'Pracant'.", SoundEffectName.Rad8);
                    oDalsiPracanti.Visible = true;
                }
            };
            Objective oStan1 = new Objective("Vyber pracanta, pak vyber, aby postavil obytný stan.")
            {
                Visible = false,
                StateTrigger = (ss)=> ss.Selection.SelectedBuildingToPlace?.Id == BuildingId.Tent,
                OnComplete = (ss) =>
                {
                    s.EnqueueVoice("Nyní klikni levým tlačítkem kamkoliv na louku, a Pracant tam postaví stan.", SoundEffectName.Rad6);
                    s.EnqueueVoice("V levém horním rohu obrazovky vidíš také svůj populační limit. Nemůžeš mít více skautů, než kolik dokážeš ubytovat ve stanech.", SoundEffectName.Rad7);
                    oStan2.Visible = true;
                }
            };
            Objective oStrom = new Objective("Pošli pracanta těžit strom.")
            {
                Visible = false,
                StateTrigger = (ss)=>ss.Session.AllUnits.Any(unt => unt.Tactics.GatherTarget?.ProvidesResource == Resource.Wood),
                OnComplete = (ss) =>
                {
                    s.EnqueueVoice("Tvoji Pracanti teď sbírají suroviny. Kolik máš jídla a dřeva vidíš v levém horním rohu obrazovky.", SoundEffectName.Rad4);
                    s.EnqueueVoice("Až budeš mít dost dřeva, vyber jednoho Pracanta a v dolní liště klikni na Obytný stan.", SoundEffectName.Rad5);
                    oStan1.Visible = true;
                }
            };
            s.Objectives.Add(new Objective("Pošli všechny své pracanty sbírat lesní plody.")
            {
                StateTrigger = (ss)=>ss.Session.AllUnits.Any(unt => unt.Tactics.GatherTarget?.ProvidesResource == Resource.Food),
                OnComplete = (ss)=>
                {
                    s.EnqueueVoice("Výborně. Budeš ale potřebovat i dřevo. Vyber dva své Pracanty a klikni pravým tlačítkem na nějaký strom.", SoundEffectName.Rad3);
                    oStrom.Visible = true;
                }
            });
            s.Objectives.AddRange(new[] { oStrom, oStan1, oStan2, oDalsiPracanti, oDalsiHadrakostrelci, oTarget });
            s.EnqueueVoice(new ChatLine("Jsme na zelené louce v Radeníně, hráči, a zde se naučíme postavit vlastní tábor.", SoundEffectName.Rad1));
            s.EnqueueVoice(new ChatLine("Pracant je tvoje nejdůležitější jednotka. Těží suroviny a staví stavby. Vyber všechny své Pracanty a klikni pravým tlačítkem na keř s lesními plody..", SoundEffectName.Rad2));
            return s;
        }

        internal static Session FreeGame()
        {
            Session s = CreateBasic1v1();
            LoadMapIntoSession(s, "Levels\\BlankMap.tmx");
            var enemy = s.Troops[1];
            enemy.AI = new AggressiveAI(enemy);
            s.Objectives.Add(new Objective("Vyřaď všechny nepřátelské jednotky.")
            {
                Visible = true,
                StateTrigger = (levelPhase) => s.AllUnits.Count(unt => unt.Controller == enemy) == 0,
                OnComplete = (session) => session.AchieveEnding(Ending.Victory)
            });
            s.ObjectivesChanged = false;
            s.LevelName = "Volná hra";
            return s;
        }

        internal static Session FreeNoEnemiesGame()
        {
            Session s = CreateBasic1v1();
            LoadMapIntoSession(s, "Levels\\BlankMap.tmx");
            s.Troops[1].Surrender();
            s.Objectives.Add(new Objective("Nemáš žádné úkoly. Dělej, co chceš!"));
            s.ObjectivesChanged = false;
            s.LevelName = "Bez nepřátel";
            return s;
        }
        internal static Session DebugMap()
        {
            Session s = CreateBasic1v1();
            LoadMapIntoSession(s, "Levels\\DebugMap.tmx");
            s.Objectives.Add(new Objective("Programuj ^^"));
            s.ObjectivesChanged = false;
            s.LevelName = "debug mapa";
            return s;
        }
    }
}
