using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.HUD;
using Age.Music;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Phases
{
    class SettingsPhase : GamePhase
    {
        Rectangle rectMenu = new Rectangle(Root.ScreenWidth / 2 - 300, Root.ScreenHeight / 2 - 350, 600, 700);

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Primitives.DrawAndFillRectangle(rectMenu, ColorScheme.Background, ColorScheme.Foreground);

            UI.DrawRadioButton(new Rectangle(rectMenu.X + 10, rectMenu.Y + 50, 300, 40), topmost, "Celoobrazovkový režim", () =>
            {
                Root.GoToFullscreen(Settings.Instance.Resolution);
                Settings.Instance.DisplayMode = DisplayModus.Fullscreen;
            }, () => Settings.Instance.DisplayMode == DisplayModus.Fullscreen, "V celoobrazovkovém režimu má hra největší výkon, ale automaticky se minimalizuje, když se přesuneš na jiné okno, a nemusí se zobrazovat správně, pokud má jiné rozlišení než nativní rozlišení tvého monitoru.");
            UI.DrawRadioButton(new Rectangle(rectMenu.X + 10, rectMenu.Y + 90, 300, 40), topmost, "Okno na celou obrazovku", () =>
            {
                Root.GoToBorderlessWindow(Settings.Instance.Resolution);
                Settings.Instance.DisplayMode = DisplayModus.BorderlessWindow;
            }, () => Settings.Instance.DisplayMode == DisplayModus.BorderlessWindow, "Hra v tomto režimu zabírá celý monitor, ale nezabírá si celou grafickou kartu jen pro sebe. Ve vzácných případech ale může panel úloh překrývat tuto hru.");
            UI.DrawRadioButton(new Rectangle(rectMenu.X + 10, rectMenu.Y + 130, 300, 40), topmost, "Okno", () =>
            {
                Settings.Instance.DisplayMode = DisplayModus.Windowed;
                Root.GoToNormalWindow(Settings.Instance.Resolution);
                }, () => Settings.Instance.DisplayMode == DisplayModus.Windowed, "Hra je v okně podobně jako ostatní aplikace.");

            UI.DrawCheckbox(new Rectangle(rectMenu.X + 10, rectMenu.Y + 190, 300, 40), topmost, "VSync", () =>
            {
                Root.SynchronizeWithVerticalRetrace = !Root.SynchronizeWithVerticalRetrace;
            }, () => game.IsFixedTimeStep, "Když zašrktneš {b}VSync{/b}, bude hra sladěna s obnovovací frekvencí tvého monitoru. Sníží se tak \"trhání\" obrazu, který může tak být plynulejší, ale výkon se může snížit.");

            UI.DrawSlider(new Rectangle(rectMenu.X + 10, rectMenu.Y + 240, 300, 40), topmost, "Hlasitost hudby", (value) =>
            {
                Settings.Instance.MusicVolume = value;
                BackgroundMusicPlayer.SetVolume(value);
            }, () => Settings.Instance.MusicVolume, null);
            UI.DrawSlider(new Rectangle(rectMenu.X + 10, rectMenu.Y + 290, 300, 40), topmost, "Hlasitost zvuků", (value) =>
            {
                Settings.Instance.SfxVolume = value;
            }, () => Settings.Instance.SfxVolume, null);

            UI.DrawButton(new Rectangle(rectMenu.X + 10, rectMenu.Bottom - 50, 300, 40), topmost, "Zavřít", Root.PopFromPhase);

            UI.MajorTooltip?.Draw(new Rectangle(rectMenu.X + 10, rectMenu.Bottom - 150, 400, 90));

            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.F12) || Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Root.PopFromPhase();
            }
            base.Update(game, elapsedSeconds);
        }
    }
}
