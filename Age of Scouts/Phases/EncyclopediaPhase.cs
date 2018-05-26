using Age.Core;
using Age.HUD;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Age.Phases
{
    class EncyclopediaPhase : GamePhase
    {
        Rectangle rectMenu = new Rectangle(Root.ScreenWidth / 2 - 500, Root.ScreenHeight / 2 - 400, 1000, 800);
        string Title;
        string Description;
        Texture2D Photo;
        Entity entity;
        public EncyclopediaPhase(Entity entity)
        {
            this.entity = entity;
        }

        protected override void Initialize(Game game)
        {
            try
            {
                string txt = System.IO.File.ReadAllText("Encyclopedia\\" + entity.EncyclopediaFilename  + ".txt");
                string[] split = txt.Split(new char[] { '\n' } , 2);
                Title = split[0];
                Description = split[1];
            }
            catch (Exception exception)
            {
                Title = entity.Name;
                Description = "";
            }
            Photo = Library.Get(entity.Icon);
            base.Initialize(game);
        }

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Primitives.DrawAndFillRectangle(rectMenu, ColorScheme.Background, ColorScheme.Foreground);

            Primitives.DrawMultiLineText("{b}" + Title + "{/b}\n\n" + Description, rectMenu.Extend(-4, -4), Color.Black, FontFamily.Mid);

            UI.DrawButton(new Rectangle(rectMenu.Right - 310, rectMenu.Height - 50, 300, 40), topmost,
                "Zavřít", () => Root.PopFromPhase());

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
