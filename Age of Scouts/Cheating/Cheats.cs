using Age.Core;
using Auxiliary;
using Microsoft.Xna.Framework;

namespace Age.Phases
{
    internal class Cheats
    {
        public static void Update(LevelPhase levelPhase)
        {
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Q, ModifierKey.Ctrl))
            {
                Settings.Instance.EnableFogOfWar = !Settings.Instance.EnableFogOfWar;
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.F9, ModifierKey.Ctrl))
            {
                levelPhase.Session.PlayerTroop.Omniscience = !levelPhase.Session.PlayerTroop.Omniscience;
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.E, ModifierKey.Ctrl))
            {
                Settings.Instance.EnemyUnitsRevealFogOfWar = !Settings.Instance.EnemyUnitsRevealFogOfWar;
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.V, ModifierKey.Ctrl))
            {
                levelPhase.Session.AchieveEnding(Ending.Victory);
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.R, ModifierKey.Ctrl))
            {
                Settings.Instance.ShowDebugPoints = !Settings.Instance.ShowDebugPoints;
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.I, ModifierKey.Ctrl))
            {
                Settings.Instance.TimeFactor *= 2;
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.U, ModifierKey.Ctrl))
            {
                Settings.Instance.TimeFactor /= 2;
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.F7, ModifierKey.Ctrl))
            {
                levelPhase.Session.PlayerTroop.Food += 1000;
                levelPhase.Session.PlayerTroop.Clay += 1000;
                levelPhase.Session.PlayerTroop.Wood += 1000;
                SFX.PlaySoundUnlessPlaying(SoundEffectName.SoftFanfare);
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.F8, ModifierKey.Ctrl))
            {
                Settings.Instance.Aegis = !Settings.Instance.Aegis;
                SFX.PlaySoundUnlessPlaying(SoundEffectName.SoftFanfare);
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.F10, ModifierKey.Ctrl))
            {
                levelPhase.Session.PlayerTroop.LeaderPowers = new System.Collections.Generic.List<LeaderPowerInstance>
                {
                    new LeaderPowerInstance(LeaderPower.CreateSpy(), false),
                    new LeaderPowerInstance(LeaderPower.CreateSpy(), false),
                    new LeaderPowerInstance(LeaderPower.CreateArtillery(), false),
                    new LeaderPowerInstance(LeaderPower.CreateArtillery(), false)
                };
                SFX.PlaySoundUnlessPlaying(SoundEffectName.SoftFanfare);
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.F2, ModifierKey.Ctrl))
            {
                Settings.Instance.ShowPerformanceIndicators = !Settings.Instance.ShowPerformanceIndicators;
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.F5, ModifierKey.Ctrl, ModifierKey.Alt, ModifierKey.Shift))
            {
                throw new System.Exception("Game crashed because you pressed Ctrl+Alt+Shift+F5.");
            }
        }
        public static void Draw(LevelPhase levelPhase)
        {

            if (Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                string strCheats =
                    "Držet [F1]: Zobrazovat obrazovku s cheaty\n[Ctrl+Q]: Aktivovat/deaktivovat válečnou mlhu\n[Ctrl+V]: Vyhrát level\n[Ctrl+R]: Zobrazit debugovací body\n[Ctrl+U]: Zpomalit čas\n[Ctrl+I]: Zrychlit čas\n[Ctrl+F2]: Zobrazit/skrýt indikátory výkonu\n[Ctrl+E] Zobrazit/skrýt odhalení válečné mlhy cizími jednotkami\n[Ctrl+F7] Přidat sobě +1000 od každé suroviny.\n[Ctrl+F8] Zapnout/vypnout rychlé stavění (aegis)\n[Ctrl+F9] Zapnout/vypnout vidění skrze nepřátele\n[Ctrl+F10] Získat další vůdcovské schopnosti\n[Ctrl+Alt+Shift+F5] Shodit hru (!!)";
                Rectangle rectCheats = new Rectangle(0, 200, 400, 600);
                Primitives.FillRectangle(rectCheats, Color.Brown.Alpha(240));
                BasicStringDrawer.DrawMultiLineText(strCheats, rectCheats.Extend(-3, -3), Color.White, Library.FontTiny);
            }
        }
    }
}