using Auxiliary;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Voice
{
    class ChatLine
    {
        public string Text;
        public float SecondsRemaining;
        public SoundEffectName Sfx;

        public ChatLine(string text, SoundEffectName sfx)
        {
            Text = text;
            Sfx = sfx;
            SoundEffect sfxReal = Library.Get(sfx);
            SecondsRemaining = (float)sfxReal.Duration.TotalSeconds + 0.3f;
        }
    }
}
