using Age.HUD;
using Age.Music;
using Age.World;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Phases
{
    class AgeMainMenu : DoorPhase
    {
        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Primitives.FillRectangle(Root.Screen, Color.FromNonPremultiplied(144, 237, 192, 255));
            Primitives.DrawImage(Library.Get(TextureName.AgeOfScoutsLogo), new Rectangle(10, 10, 600, 200));
            Primitives.DrawSingleLineText("Toto je alfa verze technického dema. Tato hra není ještě doprogramována.\nPokud bys chtěl nebo chtěla pomoci, napiš mi prosím\nna petrhudecek2010@gmail.com", new Vector2(10, 230), Color.Black, Library.FontNormal);

            UI.DrawButton(new Rectangle(Root.ScreenWidth - 310, 200, 300, 40), topmost, "Úroveň 1: Přepad (tutoriál)", () => { TransitionTo(new LevelPhase(Levels.Tutorial())); }, "V této výukové úrovni se naučíš, jak pohybovat s jednotkami a bojovat proti nepříteli tak, že přepadneš nepřátelský tábor.");
            UI.DrawButton(new Rectangle(Root.ScreenWidth - 310, 300, 300, 40), topmost, "Volná hra", () => { TransitionTo(new LevelPhase(Levels.FreeGame())); }, "Na této mapě není žádný příběh. Začínáš s kuchyní a několika pracanty, a hraješ proti počítačovému soupeři, který je na tom podobně. Abys vyhrál, musíš zneškodit všechny soupeřovy jednotky.");
            UI.DrawButton(new Rectangle(Root.ScreenWidth - 310, 350, 300, 40), topmost, "Volná hra (bez nepřátel)", () => { TransitionTo(new LevelPhase(Levels.FreeNoEnemiesGame())); }, "Tuto mapu nelze vyhrát ani prohrát, ani na ni nelze bojovat. Můžeš ale stavět vlastní tábor bez toho, aby tě přitom někdo vyrušoval.");
            UI.DrawButton(new Rectangle(Root.ScreenWidth - 310, 400, 300, 40), topmost, "Počítač proti počítači", () => { TransitionTo(new LevelPhase(Levels.AIvsAI())); }, "Dva počítačoví hráči budou hrát proti sobě, a ty se můžeš jen dívat.");
            UI.DrawButton(new Rectangle(Root.ScreenWidth - 310, 450, 300, 40), topmost, "Debug mapa", () => { TransitionTo(new LevelPhase(Levels.DebugMap())); }, "Tato mapa je určena pro vývojáře.");
            UI.DrawButton(new Rectangle(Root.ScreenWidth - 310, 600, 300, 40), topmost, "Nastavení", () => { Root.PushPhase(new SettingsPhase()); }, "Změň možnosti zobrazení a další věcičky.");
            UI.DrawButton(new Rectangle(Root.ScreenWidth - 310, 650, 300, 40), topmost, "O autorech", () => { TransitionTo(new CreditsPhase()); }, "Chceš vědět, kdo na této hře pracoval?");
            UI.DrawButton(new Rectangle(Root.ScreenWidth - 310, 700, 300, 40), topmost, "Ukončit hru", () => TransitionIntoExit(), "Okamžitě ukončí hru.");
            UI.MajorTooltip?.Draw(new Rectangle(10, Root.ScreenHeight - 155, 400, 150));
            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void OnEntering()
        {
            BackgroundMusicPlayer.Play(BackgroundMusicPlayer.MenuMusic);
        }
        protected override void Initialize(Game game)
        {
            this.OnEntering();
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.TransitionIntoExit();
            }
            base.Update(game, elapsedSeconds);
        }
    }
}
