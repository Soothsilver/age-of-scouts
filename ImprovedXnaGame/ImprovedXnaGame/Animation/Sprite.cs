using Auxiliary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Age.Animation
{
    internal class Sprite
    {
        public Dictionary<AnimationListKey, AnimationList> AnimationLists = new Dictionary<AnimationListKey, AnimationList>();
        internal int Width;
        internal int Height;

        public static Sprite Kid;

        internal SpriteInstance CreateInstance(Color color)
        {
            return new SpriteInstance(this, color, AnimationListKey.Idle);
        }

        internal Texture2D GetTexture(AnimationListKey currentAnimation, int frameIndex, Color color)
        {
            return SpriteCache.GetColoredTexture(this.AnimationLists[currentAnimation].Frames[frameIndex % this.AnimationLists[currentAnimation].Frames.Count], color);
        }
        

        internal static void PerformStaticInitialization()
        {
            Kid = new Sprite
            {
                Width = Library.Get(TextureName.KidIdle).Width,
                Height = Library.Get(TextureName.KidIdle).Height
            };
            Kid.AnimationLists.Add(AnimationListKey.Idle, new AnimationList(TextureName.KidIdle, TextureName.KidIdle, TextureName.KidIdle2, TextureName.KidIdle2));
            Kid.AnimationLists.Add(AnimationListKey.MoveRight, new AnimationList(TextureName.KidMoveRight, TextureName.KidMoveRight2));
            Kid.AnimationLists.Add(AnimationListKey.MoveLeft, new AnimationList(TextureName.KidMoveLeft, TextureName.KidMoveLeft2));
            Kid.AnimationLists.Add(AnimationListKey.MoveDown, new AnimationList(TextureName.KidMoveDown, TextureName.KidMoveDown2, TextureName.KidMoveDown, TextureName.KidMoveDown3));
            Kid.AnimationLists.Add(AnimationListKey.MoveUp, new AnimationList(TextureName.KidMoveUp, TextureName.KidMoveUp2, TextureName.KidMoveUp, TextureName.KidMoveUp3));
        }
    }
    class AnimationList
    {
        public List<TextureName> Frames = new List<TextureName>();
        public AnimationList(params TextureName[] frames)
        {
            Frames.AddRange(frames);
        }
    }
    enum AnimationListKey
    {
        Idle,
        MoveDown,
        MoveRight,
        MoveUp,
        MoveLeft
    }
}