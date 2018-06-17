using System;
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
        public bool CanAttack = true;
        public string EncyclopediaFilename;
        public float VoiceBasicVolume = 1;

        private SoundEffect Ack1;
        private SoundEffect Ack2;
        private SoundEffect AckMove;
        private SoundEffect Fail;
        private SoundEffect Joke1;
        private SoundEffect Joke2;
        private SoundEffect Build;
        private SoundEffect Joke3;
        private SoundEffect Selection1;
        private SoundEffect Selection2;
        private SoundEffect Selection3;
        private SoundEffect Selection4;
        private SoundEffect AckAttack;
        private SoundEffect AckAttack2;
        private SoundEffect UnitCreated;
        private SoundEffect GatherWood;
        private SoundEffect GatherMud;
        private SoundEffect GatherBerries;

       

        private SoundEffect GatherCorn;
        public UnitId Id;

        private UnitTemplate(UnitId id, string name, string description, TextureName icon, TextureName deadIcon, Sprite sprite,
            int foodcost, int woodcost, int mudcost)
        {
            Id = id;
            Name = name;
            Description = description;
            FoodCost = foodcost;
            WoodCost = woodcost;
            MudCost = mudcost;
            Icon = icon;
            DeadIcon = deadIcon;
            Sprite = sprite;
        }

        public static UnitTemplate Katapult;
        public static UnitTemplate Hadrakostrelec;
        public static UnitTemplate Pracant;
        internal int FoodCost;
        internal int WoodCost;
        internal int MudCost;

        public bool CanGatherStuff;
        /// <summary>
        /// In tiles, how far can this unit attack.
        /// </summary>
        internal int AttackRange = 5;
        /// <summary>
        /// In standard pixels, how much distance can this unit cross per second.
        /// </summary>
        internal float Speed = Tile.WIDTH;

        public static void InitUnitTemplates()
        {
            Pracant = new UnitTemplate(UnitId.Pracant, "Pracant", "Pracant je nejdůležitější jednotka. Může sbírat suroviny a stavět a opravovat budovy. Pracanty nabíráš z {b}kuchyně{/b}.", TextureName.PracantLogo, TextureName.KidBroken, Sprite.Pracant, 50, 0, 0)
            {
                CanBuildStuff = true,
                CanGatherStuff = true,
                CanAttack = false,
                EncyclopediaFilename = "Pracant"
            };
            Hadrakostrelec = new UnitTemplate(UnitId.Hadrakostrelec, "Hadrákostřelec", "Hadrákostřelec je základní bojová jednotka. Hází po nepřátelích přesné papírové míčky z dálky. Dá se nabrat z {b}muničního stanu{/b}.", TextureName.HadrakometLogo, TextureName.KidBroken, Sprite.Kid, 80, 20, 0)
            {
                EncyclopediaFilename = "Hadrakostrelec",
                VoiceBasicVolume = 0.6f
            };
            Katapult = new UnitTemplate(UnitId.Katapult, "Katapult", "Katapult je zvláště silný proti stavbám. Střílí turbojílové koule z obrovské dálky. Dá se vyrobit v {b}dřevařském koutě{/b}.", TextureName.KatapultRight1, TextureName.KatapultCorpse, Sprite.Katapult, 0, 150, 50)
            {
                EncyclopediaFilename = "Katapult",
                Speed = Tile.WIDTH / 3,
                AttackRange = 10
            };
        }

        internal void PlayBuildSound()
        {
            if (this.CanBuildStuff)
            {
                SFX.Play(Build, VoiceBasicVolume);
            }
            else
            {
                PlayMovementSound();
            }
        }

        public void LoadSounds(SoundEffect ack1, SoundEffect ack2, SoundEffect ackMove, SoundEffect fail, SoundEffect joke1, SoundEffect joke2, SoundEffect joke3, SoundEffect sel1, SoundEffect sel2, SoundEffect sel3, SoundEffect sel4, SoundEffect born)
        {
            Ack1 = Build = GatherBerries = GatherCorn = GatherMud = GatherWood = AckAttack = AckAttack2 = ack1;
            Ack2 = ack2;
            UnitCreated = born;
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
        public void LoadAttackSounds(SoundEffect one, SoundEffect two)
        {
            AckAttack = one;
            AckAttack2 = two;
        }
        public void LoadPracantSounds(SoundEffect build, SoundEffect corn, SoundEffect mud, SoundEffect berries, SoundEffect wood)
        {
            GatherWood = wood;
            GatherCorn = corn;
            GatherMud = mud;
            GatherBerries = berries;
            Build = build;
        }

        internal void PlayMovementSound()
        {
            SFX.LastUnitSelectedXTimes = 0;
            SFX.PlayRandom(VoiceBasicVolume, Ack1, Ack2, AckMove);
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
                if (SFX.LastUnitSelectedXTimes % 8 == 5) SFX.Play(Joke1, VoiceBasicVolume);
                if (SFX.LastUnitSelectedXTimes % 8 == 6) SFX.Play(Joke2, VoiceBasicVolume);
                if (SFX.LastUnitSelectedXTimes % 8 == 7) SFX.Play(Joke3, VoiceBasicVolume);
            }
            else
            {
                SFX.PlayRandom(VoiceBasicVolume, Selection1, Selection2, Selection3, Selection4);
            }
        }

        internal void PlayGatherSound(EntityKind kind)
        {
            SFX.LastUnitSelectedXTimes = 0;
            switch (kind)
            {
                case EntityKind.BerryBush:
                    SFX.Play(GatherBerries, VoiceBasicVolume);
                    break;
                case EntityKind.Corn:
                    SFX.Play(GatherCorn, VoiceBasicVolume);
                    break;
                case EntityKind.UntraversableTree:
                    SFX.Play(GatherWood, VoiceBasicVolume);
                    break;
                case EntityKind.MudMine:
                    SFX.Play(GatherMud, VoiceBasicVolume);
                    break;
            }
        }

        internal void PlayUnitCreatedSound(float volumeModifier)
        {
            SFX.Play(UnitCreated, VoiceBasicVolume * volumeModifier);
        }
        internal void PlayAttackSound()
        {
            SFX.LastUnitSelectedXTimes = 0;
            SFX.PlayRandom(VoiceBasicVolume, AckAttack, AckAttack2, Ack1, Ack2);
        }
    }

    internal enum UnitId
    {
        Pracant,
        Hadrakostrelec,
        Katapult
    }
}