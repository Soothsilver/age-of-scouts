using System;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Phases
{
    internal class DoorPhase : GamePhase
    {
        private float TransitionPercentage = 0;
        private bool? TransitioningDirectionIsUp = null;
        private float TransitionSpeed = 5;
        private float SecondsUntilTransitionCompletes = 0;
        private GamePhase TransitioningInto = null;


        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            base.Draw(sb, game, elapsedSeconds, topmost);
            // Draw
            int gateWidth = Root.ScreenWidth / 2;
            Color innerGateColor = Color.FromNonPremultiplied(239, 181, 64, 255);
            Color outerGateColor = Color.FromNonPremultiplied(112, 74, 0, 255);
            Rectangle rectLeftGate = new Rectangle((int)(-gateWidth + gateWidth * TransitionPercentage), 0, gateWidth, Root.ScreenHeight);
            Rectangle rectRightGate = new Rectangle((int)(2*gateWidth - gateWidth * TransitionPercentage), 0, gateWidth, Root.ScreenHeight);
            Primitives.DrawAndFillRoundedRectangle(rectLeftGate, innerGateColor, outerGateColor, 4);
            Primitives.DrawAndFillRoundedRectangle(rectRightGate, innerGateColor, outerGateColor, 4);
            int logoW = Library.Get(TextureName.LogoRight).Width;
            int logoH = Library.Get(TextureName.LogoRight).Height;
            Primitives.DrawImage(Library.Get(TextureName.LogoLeft), new Rectangle(rectLeftGate.Right - logoW, rectLeftGate.Height / 2 - logoH / 2, logoW, logoH));
            Primitives.DrawImage(Library.Get(TextureName.LogoRight), new Rectangle(rectRightGate.X, rectRightGate.Height / 2 - logoH / 2, logoW, logoH));
            // Then move
            if (TransitioningDirectionIsUp.HasValue && TransitioningDirectionIsUp.Value)
            {
                if (TransitionPercentage == 1)
                {
                    SecondsUntilTransitionCompletes -= elapsedSeconds;
                    if (SecondsUntilTransitionCompletes <= 0)
                    {
                        TransitioningDirectionIsUp = null;
                        TransitionPercentage = 0;
                        if (TransitioningInto is DoorPhase)
                        {
                            DoorPhase dp = (TransitioningInto as DoorPhase);
                            dp.TransitionPercentage = 1;
                            dp.TransitioningDirectionIsUp = false;
                            dp.OnEntering();
                        }
                        if (TransitioningInto != null && !Root.PhaseStack.Contains(TransitioningInto))
                        {
                            Root.PushPhase(TransitioningInto);
                        } else
                        {
                            Root.PopFromPhase();
                        }
                        TransitioningInto = null;
                    }
                }
                else
                {
                    TransitionPercentage += elapsedSeconds * TransitionSpeed;
                    if (TransitionPercentage >= 1)
                    {
                        TransitionPercentage = 1;
                        SecondsUntilTransitionCompletes = 0.5f;
                    }
                }
            }
            else if (TransitioningDirectionIsUp.HasValue && !TransitioningDirectionIsUp.Value) {
                TransitionPercentage -= elapsedSeconds * TransitionSpeed;
                if (TransitionPercentage <= 0)
                {
                    TransitionPercentage = 0;
                    TransitioningDirectionIsUp = null;
                }
            }
        }

        protected virtual void OnEntering()
        {
        }

        public void TransitionIntoExit()
        {
            if (TransitioningInto == null)
            {
                TransitioningInto = Root.GetPhaseBeneath(this);
                TransitioningDirectionIsUp = true;
            }
        }
        public void TransitionTo(GamePhase anotherPhase)
        {
            if (TransitioningInto == null)
            {
                TransitioningInto = anotherPhase;
                TransitioningDirectionIsUp = true;
            }
        }
    }
}