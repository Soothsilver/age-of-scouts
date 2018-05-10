using Age.Core;
using Age.HUD;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Phases
{
    class IngameMenuPhase : GamePhase
    {
        private readonly LevelPhase levelPhase;
        Rectangle rectMenu = new Rectangle(Root.ScreenWidth / 2 - 250, Root.ScreenHeight / 2 - 400, 500, 800);

        public IngameMenuPhase(LevelPhase levelPhase)
        {
            this.levelPhase = levelPhase;
        }

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Primitives.DrawAndFillRectangle(rectMenu, ColorScheme.Background, ColorScheme.Foreground);

            int x = rectMenu.X + rectMenu.Width / 2 - 150;
            int y = rectMenu.Y + 50;
            int width = 300;
            int height = 40;
            UI.DrawButton(new Rectangle(x, y, width, height), topmost, "Zobrazit úkoly", () => Root.PushPhase(new ViewObjectivesPhase(levelPhase.Session)));
            UI.DrawButton(new Rectangle(x, y + 60, width, height), topmost, "Vzdát úroveň!", () =>
            {
                levelPhase.Session.AchieveEnding(Ending.Defeat);
                Root.PopFromPhase();
            });
        
            UI.DrawButton(new Rectangle(x, y + 120, width, height), topmost, "Ukončit do hlavního menu!", () => { (Root.PhaseStack[Root.PhaseStack.Count - 2] as DoorPhase).TransitionIntoExit(); Root.PopFromPhase(); });
            UI.DrawButton(new Rectangle(x, y + 180, width, height), topmost, "Ukončit do Windows!", () => {
                Root.PhaseStack.Clear();
            });
            UI.DrawButton(new Rectangle(x, rectMenu.Height - 40, width, height), topmost, "Vrátit se ke hře", () => {
                Root.PopFromPhase();
            });

            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Root.PopFromPhase();
            }
            base.Update(game, elapsedSeconds);
        }
    }
}
