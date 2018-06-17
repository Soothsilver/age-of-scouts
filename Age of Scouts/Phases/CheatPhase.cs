using Age.Core;
using Age.HUD;
using Auxiliary;
using Auxiliary.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Age.Phases
{
    class CheatPhase : GamePhase
    {
        Rectangle rectMenu = new Rectangle(Root.ScreenWidth / 2 - 500, Root.ScreenHeight / 2 - 400, 1000, 800);
        Textbox textbox;
        LevelPhase levelPhase;

        public CheatPhase(LevelPhase levelPhase)
        {
            textbox = new Textbox("", new Rectangle(rectMenu.X + 10, rectMenu.Y + 50, rectMenu.Width - 20, 40));
            textbox.EnterPress += Textbox_EnterPress;
            this.levelPhase = levelPhase;
        }

        protected override void Initialize(Game game)
        {
            base.Initialize(game);
            textbox.Activate();
        }

        private void Textbox_EnterPress(Textbox obj)
        {
            if (cheatToUse != null)
            {
                cheatToUse.SoWhat(levelPhase, levelPhase.Session);
            }
            Root.PopFromPhase();
        }
        Cheating.Cheat cheatToUse = null;

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Primitives.DrawAndFillRectangle(rectMenu, ColorScheme.Background, ColorScheme.Foreground);

            Primitives.DrawMultiLineText("Zadej cheat a potvrď ho klávesou Enter:", rectMenu.Extend(-4, -4), Color.Black, FontFamily.Normal);

            textbox.Draw();

            int y = rectMenu.Y + 100;
            bool first = true;
            cheatToUse = null;
            foreach(Cheating.Cheat cheat in Cheating.Cheat.Cheats)
            {
                if (y >= rectMenu.Bottom - 130)
                {
                    break;
                }
                if (cheat.Command.Contains(textbox.Text.ToLower()) || cheat.Description.RemoveDiacritics().ToLower().Contains(textbox.Text.ToLower()))
                {
                    Rectangle rCheat = new Rectangle(rectMenu.X + 10, y, rectMenu.Width - 20, 75);
                    Primitives.DrawAndFillRectangle(rCheat, first ? Color.LightGoldenrodYellow : Color.Wheat, Color.Black, 2);
                    Primitives.DrawMultiLineText("{b}" + cheat.Command + "{/b}\n" + cheat.Description, rCheat.Extend(-4, -4), Color.Black, FontFamily.Normal);
                    if (first && textbox.Text.Length > 0)
                    {
                        cheatToUse = cheat;
                    }
                    first = false;
                    y += 80;
                }
            }
            

            UI.DrawButton(new Rectangle(rectMenu.Right - 310, rectMenu.Bottom - 50, 300, 40), topmost,
                "Zavřít", () => Root.PopFromPhase());

            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Root.PopFromPhase();
            }
            textbox.Update();
            base.Update(game, elapsedSeconds);
        }
    }
}
