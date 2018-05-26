using System;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Animation
{
    class SpriteInstance
    {
        private TimeSpan TimeSpanToBeSpentOnEachFrame = TimeSpan.FromSeconds(0.25f);

        public Sprite Sprite;
        public AnimationListKey CurrentAnimation;
        private int FrameIndex;
        private TimeSpan TimeSpentOnThisFrame;
        public Color Color;

        public SpriteInstance(Sprite sprite, Color color, AnimationListKey idle)
        {
            this.Sprite = sprite;
            Color = color;
            this.CurrentAnimation = idle;
        }


        internal Texture2D AnimationTick(float elapsedSeconds)
        {
            TimeSpentOnThisFrame = TimeSpentOnThisFrame.Add(TimeSpan.FromSeconds(elapsedSeconds));
            if (TimeSpentOnThisFrame >= TimeSpanToBeSpentOnEachFrame)
            {
                TimeSpentOnThisFrame -= TimeSpanToBeSpentOnEachFrame;
                FrameIndex++;
            }
            return this.Sprite.GetTexture(CurrentAnimation, FrameIndex, Color);
        }

        internal AnimationListKey DetermineAnimationKeyFromMovement(Vector2 newPosition, Vector2 oldPosition)
        {
            float xdif = newPosition.X - oldPosition.X;
            float ydif = newPosition.Y - oldPosition.Y;
            bool xIsGreater = Math.Abs(xdif) > Math.Abs(ydif);
            if (xdif > 0 && xIsGreater)
            {
                return AnimationListKey.MoveRight;
            }
            else if (xdif < 0 && xIsGreater)
            {
                return AnimationListKey.MoveLeft;
            }
            else if (ydif > 0)
            {
                return AnimationListKey.MoveDown;
            }
            else if (ydif < 0)
            {
                return AnimationListKey.MoveUp;          
            }
            else 
            {
                return AnimationListKey.Idle;
            }
        }
    }
}