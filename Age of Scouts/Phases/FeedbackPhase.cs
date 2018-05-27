using Age.HUD;
using Age.Internet;
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
    class FeedbackPhase : GamePhase
    {
        private static string text = "";
        Rectangle rectMenu = new Rectangle(Root.ScreenWidth / 2 - 500, Root.ScreenHeight / 2 - 400, 1000, 800);
        Textbox textbox;
        
        protected override void Initialize(Game game)
        {
            base.Initialize(game);
            textbox = new Textbox(text, new Rectangle(rectMenu.X + 10, rectMenu.Y + 90, rectMenu.Width - 20, rectMenu.Height - 150), true);
            textbox.Skin.Font = Library.FontNormalBold;
            textbox.Activate();
        }

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Primitives.DrawAndFillRectangle(rectMenu, ColorScheme.Background, ColorScheme.Foreground);

            Primitives.DrawSingleLineText("Zpětná vazba", new Vector2(rectMenu.X + 10, rectMenu.Y + 10), Color.Black, Library.FontNormalBold);

            Primitives.DrawSingleLineText("Řekni autorům například, co se ti líbilo nebo jestli jsi našel nějakou chybu."
                , new Vector2(rectMenu.X + 10, rectMenu.Y + 50), Color.Black, Library.FontMid);

            textbox.Draw();
            UI.DrawButton(new Rectangle(rectMenu.X + 10, rectMenu.Bottom - 50, 300, 40), topmost,
              "Odeslat", () =>
              {
                  Eqatec.ScheduleSendMessage(Eqatec.FEEDBACK, textbox.Text);
                  text = "";
                  // TODO rict dekujeme
                  Root.PopFromPhase();
              });

            UI.DrawButton(new Rectangle(rectMenu.Right - 310, rectMenu.Bottom - 50, 300, 40), topmost,
                "Zavřít", () =>
                {
                    text = textbox.Text;
                    Root.PopFromPhase();
                });

            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            textbox.Update();
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Root.PopFromPhase();
            }
            base.Update(game, elapsedSeconds);
        }
    }
}
