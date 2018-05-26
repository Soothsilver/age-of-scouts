using Age.HUD;
using Age.Internet;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Phases
{
    class CreditsPhase : DoorPhase
    {
        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            string creditsString = "Verze " + Debug.Version.AsString + "\n\nVedoucí projektu: Petr Hudeček (Profesor), 7. oddíl Karibu Tábor\n\nPoděkování: Petr Milichovský (Hedík), 7. oddíl Karibu Tábor";
            Primitives.FillRectangle(Root.Screen, Color.FromNonPremultiplied(144, 237, 192, 255));
            Primitives.DrawImage(Library.Get(TextureName.AgeOfScoutsLogo), new Rectangle(Root.ScreenWidth / 2 - 300 , 100, 600, 200));
            Primitives.DrawSingleLineText(creditsString, new Vector2(Root.ScreenWidth / 2 - 250, 400), Color.Black, Library.FontNormal);
            
            UI.DrawButton(new Rectangle(Root.ScreenWidth /2- 150, Root.ScreenHeight - 80, 300, 40), topmost, "Zpět do hlavního menu", () => TransitionIntoExit());
            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void Initialize(Game game)
        {
            base.Initialize(game);
            Eqatec.ScheduleSendMessage("VIEW CREDITS", "");
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.TransitionIntoExit();
            }
        }

    }
}
