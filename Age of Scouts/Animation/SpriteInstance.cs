using System;
using Age.Core;
using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Age.Animation
{
    class SpriteInstance
    {
        private TimeSpan TimeSpanToBeSpentOnEachFrame = TimeSpan.FromSeconds(0.25f);

        public Sprite Sprite;
        public AnimationListKey CurrentAnimation { get; private set; }
        public bool CurrentAnimationIsFlipped { get; private set; } = false;
        internal void SetCurrentAnimation(AnimationListKey animation, bool horizontalFlip)
        {
            CurrentAnimation = animation;
            CurrentAnimationIsFlipped = horizontalFlip;
        }
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
            return this.Sprite.GetTexture(CurrentAnimation, CurrentAnimationIsFlipped, FrameIndex, Color);
        }

        internal Tuple<AnimationListKey, bool> DetermineAnimationKeyFromMovement(Vector2 newPosition, Vector2 oldPosition)
        {
            float xdif = newPosition.X - oldPosition.X;
            float ydif = newPosition.Y - oldPosition.Y;
            bool xIsGreater = Math.Abs(xdif) > Math.Abs(ydif);
            if (xdif > 0 && xIsGreater)
            {
                return new Tuple<AnimationListKey, bool>(AnimationListKey.MoveRight, false);
            }
            else if (xdif < 0 && xIsGreater)
            {
                return new Tuple<AnimationListKey, bool>(AnimationListKey.MoveRight, true);
            }
            else if (ydif > 0)
            {
                return new Tuple<AnimationListKey, bool>(AnimationListKey.MoveDown, false);
            }
            else if (ydif < 0)
            {
                return new Tuple<AnimationListKey, bool>(AnimationListKey.MoveUp, false);
            }
            else
            {
                return new Tuple<AnimationListKey, bool>(AnimationListKey.Idle, false);
            }
        }
        internal bool ShouldFlip(Vector2 yourPosition, Vector2 targetPosition)
        {
            float xdif = targetPosition.X - yourPosition.X;
            bool flip = xdif < 0;
            return flip;
        }
        internal Tuple<AnimationListKey, bool> DetermineAnimationKeyFromGathering(Vector2 gatherPointPosition, Vector2 gathererPosition, NaturalObject gatheringFrom)
        {
            bool flip = ShouldFlip(gathererPosition, gatherPointPosition);
            switch(gatheringFrom.ProvidesResource)
            {
                case Resource.Clay:
                    return new Tuple<AnimationListKey, bool>(AnimationListKey.GatherClayRight, flip);
                case Resource.Food:
                    return new Tuple<AnimationListKey, bool>(AnimationListKey.GatherBerriesRight, flip);
                case Resource.Wood:
                    return new Tuple<AnimationListKey, bool>(AnimationListKey.GatherWoodRight, flip);
                default:
                    return new Tuple<AnimationListKey, bool>(AnimationListKey.Idle, false);
            }
        }
    }
}