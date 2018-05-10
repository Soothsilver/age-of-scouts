using Age.Core;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.HUD
{
    internal class ViewObjectivesPhase : GamePhase
    {
        private Session session;

        public ViewObjectivesPhase(Session session)
        {
            this.session = session;
        }

        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            Rectangle rectObjectives = new Rectangle(Root.ScreenWidth / 2 - 200, 100, 400, 500);
            Primitives.DrawAndFillRoundedRectangle(rectObjectives, ColorScheme.Background, ColorScheme.Foreground);
            Primitives.DrawSingleLineText("Tvoje úkoly", new Vector2(rectObjectives.X + 20, rectObjectives.Y + 20), Color.Black, Library.FontNormal);
            int y = rectObjectives.Y + 80;
            foreach (Objective objective in session.Objectives)
            {
                if (objective.Visible)
                {
                    Primitives.DrawSingleLineText(objective.Caption, new Vector2(rectObjectives.X + 10, y), Color.Black, objective.Complete ? Library.FontTiny : Library.FontTinyBold);
                }
                else
                {

                }
                y += 20;
            }
            UI.DrawButton(new Rectangle(rectObjectives.X + rectObjectives.Width / 2 - 150, rectObjectives.Bottom - 60, 300, 40),
                topmost, "Zavřít", () => Root.PopFromPhase());
            base.Draw(sb, game, elapsedSeconds, topmost);
        }

        protected override void Update(Game game, float elapsedSeconds)
        {
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Root.PopFromPhase();
            }
        }
    }
}