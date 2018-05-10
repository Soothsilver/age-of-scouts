using System;
using System.Collections.Generic;
using Age.Core;
using Auxiliary;
using Microsoft.Xna.Framework.Audio;

namespace Age.Phases
{
    internal class SFX
    {
        internal static Unit LastUnitSelected = null;
        internal static int LastUnitSelectedXTimes = 0;
        private static Dictionary<SoundEffectName, DateTime> EndsIn = new Dictionary<SoundEffectName, DateTime>();
        internal static void PlaySound(SoundEffectName sfxName)
        {
            var sfx = Library.Get(sfxName);
            EndsIn[sfxName] = DateTime.Now.Add(sfx.Duration);
            sfx.Play();
        }

        internal static void PlaySoundUnlessPlaying(SoundEffectName sfxName)
        {
            if (EndsIn.ContainsKey(sfxName))
            {
                if (DateTime.Now < EndsIn[sfxName])
                {
                    return;
                }
            }
            PlaySound(sfxName);
        }

        internal static void PlayRandom(params SoundEffect[] sfxs)
        {
            SoundEffect sfx = sfxs[R.Integer(sfxs.Length)];
            sfx.Play();
        }

        internal static void Play(SoundEffect sfx)
        {
            sfx.Play();
        }
    }
}