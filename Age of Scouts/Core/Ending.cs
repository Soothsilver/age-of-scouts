using Age.HUD;
using Age.Phases;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Core
{
    class Ending
    {
        public bool IsVictory;
        public bool Collapsed;

        public static Ending Victory => new Ending() { IsVictory = true };
        public static Ending Defeat => new Ending() { IsVictory = false };

        internal void Draw(bool topmost)
        {
            if (!Collapsed)
            {
                Microsoft.Xna.Framework.Graphics.Texture2D endsprite =
                    Library.Get(IsVictory ? TextureName.Victory : TextureName.Defeat);
                Rectangle rectEnding = new Rectangle(Root.ScreenWidth / 2 - endsprite.Width / 2, Root.ScreenHeight / 2 - endsprite.Height / 2 - 150, endsprite.Width, endsprite.Height);

                Primitives.DrawImage(endsprite, rectEnding);

                UI.DrawButton(new Rectangle(rectEnding.X, rectEnding.Bottom + 10, 300, 40), topmost, "Pokračovat ve hře", () => Collapsed = true, "Úroveň bude považována za dohranou a už nebudeš moci znovu vyhrát ani prohrát, ale budeš moci pokračovat v hraní na této mapě.");
                UI.DrawButton(new Rectangle(rectEnding.Right - 310, rectEnding.Bottom + 10, 300, 40), topmost, "Návrat do menu", () => (Root.PhaseStack.Peek() as DoorPhase).TransitionIntoExit(), "Vrátí tě do hlavního menu, odkud budeš moci vybrat další úroveň.");
            }
            else
            {
                Primitives.DrawMultiLineText((IsVictory ? "Hra skončila tvým vítězstvím. " : "Hra skončila tvou prohrou. ") + " Stiskni Esc pro návrat do menu.",
                    new Rectangle(0, Root.ScreenHeight - 400, Root.ScreenWidth, 40), Color.White, FontFamily.Tiny, Primitives.TextAlignment.Top);
            }

        }
    }
}
