using System;
using System.Collections.Generic;
using Age.Core;
using Auxiliary;
using Microsoft.Xna.Framework;
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
            sfx.Play(Settings.Instance.SfxVolume, 0, 0);
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

        internal static void PlayRandom(float volume, params SoundEffect[] sfxs)
        {
            SoundEffect sfx = sfxs[R.Next(sfxs.Length)];
            sfx.Play(Settings.Instance.SfxVolume * volume, 0, 0);
        }

        internal static void Play(SoundEffect sfx, float volume)
        {
            sfx.Play(Settings.Instance.SfxVolume * volume, 0, 0);
        }

        internal static float VolumeFromDistance(Vector2 soundSource, Vector2 listenerPoint)
        {
            float distanceInScreens = (soundSource - listenerPoint).Length() / (Root.ScreenWidth);
            float minDistanceToFade = 1;
            float maxDistance = 5;
            float fade = MathHelper.Clamp(1 - (distanceInScreens - minDistanceToFade)/(maxDistance - minDistanceToFade), 0.1f, 1f);
            return fade;
        }
    }
}