using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Phases
{
    internal class DoorPhase : GamePhase
    {
        private float transitionPercentage;
        private bool? transitioningDirectionIsUp;
        private const float TransitionSpeed = 5;
        private float secondsUntilTransitionCompletes;
        private GamePhase transitioningInto;


        protected override void Draw(SpriteBatch sb, Game game, float elapsedSeconds, bool topmost)
        {
            base.Draw(sb, game, elapsedSeconds, topmost);
            if (transitioningDirectionIsUp != null)
            {
                // Draw
                int gateWidth = Root.ScreenWidth / 2;
                Color innerGateColor = Color.FromNonPremultiplied(239, 181, 64, 255);
                Color outerGateColor = Color.FromNonPremultiplied(112, 74, 0, 255);
                Rectangle rectLeftGate = new Rectangle((int) (-gateWidth + gateWidth * transitionPercentage), 0,
                    gateWidth, Root.ScreenHeight);
                Rectangle rectRightGate = new Rectangle((int) (2 * gateWidth - gateWidth * transitionPercentage), 0,
                    gateWidth, Root.ScreenHeight);
                Primitives.DrawAndFillRoundedRectangle(rectLeftGate, innerGateColor, outerGateColor, 4);
                Primitives.DrawAndFillRoundedRectangle(rectRightGate, innerGateColor, outerGateColor, 4);
                int logoW = Library.Get(TextureName.LogoRight).Width;
                int logoH = Library.Get(TextureName.LogoRight).Height;
                Primitives.DrawImage(Library.Get(TextureName.LogoLeft),
                    new Rectangle(rectLeftGate.Right - logoW, rectLeftGate.Height / 2 - logoH / 2, logoW, logoH));
                Primitives.DrawImage(Library.Get(TextureName.LogoRight),
                    new Rectangle(rectRightGate.X, rectRightGate.Height / 2 - logoH / 2, logoW, logoH));
                // Then move
                if (transitioningDirectionIsUp.Value)
                {
                    if (transitionPercentage == 1)
                    {
                        secondsUntilTransitionCompletes -= elapsedSeconds;
                        if (secondsUntilTransitionCompletes <= 0)
                        {
                            transitioningDirectionIsUp = null;
                            transitionPercentage = 0;
                            if (transitioningInto is DoorPhase phase)
                            {
                                DoorPhase dp = phase;
                                dp.transitionPercentage = 1;
                                dp.transitioningDirectionIsUp = false;
                                dp.OnEntering();
                            }

                            if (transitioningInto != null && !Root.PhaseStack.Contains(transitioningInto))
                            {
                                Root.PushPhase(transitioningInto);
                            }
                            else
                            {
                                Root.PopFromPhase();
                            }

                            transitioningInto = null;
                        }
                    }
                    else
                    {
                        transitionPercentage += elapsedSeconds * TransitionSpeed;
                        if (transitionPercentage >= 1)
                        {
                            transitionPercentage = 1;
                            secondsUntilTransitionCompletes = 0.5f;
                        }
                    }
                }
                else if (!transitioningDirectionIsUp.Value)
                {
                    transitionPercentage -= elapsedSeconds * TransitionSpeed;
                    if (transitionPercentage <= 0)
                    {
                        transitionPercentage = 0;
                        transitioningDirectionIsUp = null;
                    }
                }
            }
        }

        protected virtual void OnEntering()
        {
        }

        public void TransitionIntoExit()
        {
            if (transitioningInto == null)
            {
                transitioningInto = Root.GetPhaseBeneath(this);
                transitioningDirectionIsUp = true;
            }
        }
        public void TransitionTo(GamePhase anotherPhase)
        {
            if (transitioningInto == null)
            {
                transitioningInto = anotherPhase;
                transitioningDirectionIsUp = true;
            }
        }
    }
}