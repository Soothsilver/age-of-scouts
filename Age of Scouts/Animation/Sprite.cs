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
        public static Sprite Pracant;

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
            Kid.AnimationLists.Add(AnimationListKey.ReadyToAttackRight, new AnimationList(TextureName.KidThrowRight1));
            Kid.AnimationLists.Add(AnimationListKey.ReadyToAttackLeft, new AnimationList(TextureName.KidThrowLeft1));
            Kid.AnimationLists.Add(AnimationListKey.AfterAttackRight, new AnimationList(TextureName.KidThrowRight2));
            Kid.AnimationLists.Add(AnimationListKey.AfterAttackLeft, new AnimationList(TextureName.KidThrowLeft2));
            Pracant = new Sprite
            {
                Width = Library.Get(TextureName.LaborerIdle).Width,
                Height = Library.Get(TextureName.LaborerIdle).Height
            };
            Pracant.AnimationLists.Add(AnimationListKey.Idle, new AnimationList(TextureName.LaborerIdle, TextureName.LaborerIdle, TextureName.LaborerIdle2, TextureName.LaborerIdle2));
            Pracant.AnimationLists.Add(AnimationListKey.MoveRight, new AnimationList(TextureName.PracantMoveRight, TextureName.PracantMoveRight2));
            Pracant.AnimationLists.Add(AnimationListKey.MoveLeft, new AnimationList(TextureName.PracantMoveLeft, TextureName.PracantMoveLeft2));
            Pracant.AnimationLists.Add(AnimationListKey.MoveDown, new AnimationList(TextureName.PracantMoveDown, TextureName.PracantMoveDown2, TextureName.PracantMoveDown, TextureName.PracantMoveDown3));
            Pracant.AnimationLists.Add(AnimationListKey.MoveUp, new AnimationList(TextureName.PracantMoveUp, TextureName.PracantMoveUp2, TextureName.PracantMoveUp, TextureName.PracantMoveUp3));
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
        MoveLeft,
        ReadyToAttackRight,
        ReadyToAttackLeft,
        AfterAttackRight,
        AfterAttackLeft
    }
}