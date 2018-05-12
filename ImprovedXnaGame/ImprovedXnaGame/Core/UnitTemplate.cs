﻿using System;
using Age.Animation;
using Age.Phases;
using Auxiliary;
using Microsoft.Xna.Framework.Audio;

namespace Age.Core
{
    class UnitTemplate
    {
        public Sprite Sprite;
        public string Name;
        public string Description;
        public TextureName Icon;
        public TextureName DeadIcon;
        public bool CanBuildStuff;

        private SoundEffect Ack1;
        private SoundEffect Ack2;
        private SoundEffect AckMove;
        private SoundEffect Fail;
        private SoundEffect Joke1;
        private SoundEffect Joke2;
        private SoundEffect Joke3;
        private SoundEffect Selection1;
        private SoundEffect Selection2;
        private SoundEffect Selection3;
        private SoundEffect Selection4;

        private UnitTemplate(string name, string description, TextureName icon, TextureName deadIcon, Sprite sprite)
        {
            Name = name;
            Description = description;
            Icon = icon;
            DeadIcon = deadIcon;
            Sprite = sprite;
        }

        public static UnitTemplate Hadrakostrelec;
        public static UnitTemplate Pracant;

        public static void InitUnitTemplates()
        {
            Pracant = new UnitTemplate("Pracant", "Pracant je nejdůležitější jednotka. Může sbírat suroviny a stavět a opravovat budovy. Pracanty nabíráš z {b}kuchyně{/b}.", TextureName.PracantLogo, TextureName.KidBroken, Sprite.Kid)
            {
                CanBuildStuff = true
            };
            Hadrakostrelec  = new UnitTemplate("Hadrákostřelec", "Hadrákostřelec je základní bojová jednotka. Hází po nepřátelích přesné papírové míčky z dálky. Dá se nabrat z {b}muničního stanu{/b}.", TextureName.HadrakometLogo, TextureName.KidBroken, Sprite.Kid);
        }

        public void LoadSounds(SoundEffect ack1, SoundEffect ack2, SoundEffect ackMove, SoundEffect fail, SoundEffect joke1, SoundEffect joke2, SoundEffect joke3, SoundEffect sel1, SoundEffect sel2, SoundEffect sel3, SoundEffect sel4)
        {
            Ack1 = ack1;
            Ack2 = ack2;
            AckMove = ackMove;
            Fail = fail;
            Joke1 = joke1;
            Joke2 = joke2;
            Joke3 = joke3;
            Selection1 = sel1;
            Selection2 = sel2;
            Selection3 = sel3;
            Selection4 = sel4;
        }

        internal void PlayMovementSound()
        {
            SFX.LastUnitSelectedXTimes = 0;
            SFX.PlayRandom(Ack1, Ack2, AckMove);
        }

        internal void PlaySelectionSound(Unit unit)
        {
            if (SFX.LastUnitSelected == unit)
            {
                SFX.LastUnitSelectedXTimes++;
            }
            else
            {
                SFX.LastUnitSelectedXTimes = 1;
                SFX.LastUnitSelected = unit;
            }
            if (SFX.LastUnitSelectedXTimes % 8 >= 5)
            {
                if (SFX.LastUnitSelectedXTimes % 8 == 5) SFX.Play(Joke1);
                if (SFX.LastUnitSelectedXTimes % 8 == 6) SFX.Play(Joke2);
                if (SFX.LastUnitSelectedXTimes % 8 == 7) SFX.Play(Joke3);
            }
            else
            {
                SFX.PlayRandom(Selection1, Selection2, Selection3, Selection4);
            }
        }
    }
}