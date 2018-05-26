using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.HUD;
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

            UI.DrawButton(new Rectangle(rectMenu.X + 10, rectMenu.Y + 50, 300, 40), topmost, "Celoobrazovkový režim", () => Root.GoToFullscreen(Settings.Instance.Resolution));
            UI.DrawButton(new Rectangle(rectMenu.X + 10, rectMenu.Y + 90, 300, 40), topmost, "Celoobrazovkové okno", () => Root.GoToBorderlessWindow(Settings.Instance.Resolution));
            UI.DrawButton(new Rectangle(rectMenu.X + 10, rectMenu.Y + 130, 300, 40), topmost, "Okno", () =>
                {
                    Root.GoToNormalWindow(Settings.Instance.Resolution);
                });

            UI.DrawButton(new Rectangle(rectMenu.X + 10, rectMenu.Y + 190, 580, 40), topmost, "Synchronizovat se zatemňovacím impulsem (vsync)", () =>
            {
                Root.SynchronizeWithVerticalRetrace = true;
            });
            UI.DrawButton(new Rectangle(rectMenu.X + 10, rectMenu.Y + 230, 300, 40), topmost, "Zakázat vsync", () =>
            {
                Root.SynchronizeWithVerticalRetrace = false;
            });

            UI.DrawButton(new Rectangle(rectMenu.X + 10, rectMenu.Bottom - 50, 300, 40), topmost, "Zavřít", Root.PopFromPhase);

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
