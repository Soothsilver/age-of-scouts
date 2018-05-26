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

        private UnitTemplate(string name, string description, TextureName icon, TextureName deadIcon, Sprite sprite,
            int foodcost, int woodcost)
        {
            Name = name;
            Description = description;
            FoodCost = foodcost;
            WoodCost = woodcost;
            Icon = icon;
            DeadIcon = deadIcon;
            Sprite = sprite;
        }

        public static UnitTemplate Hadrakostrelec;
        public static UnitTemplate Pracant;
        internal int FoodCost;
        internal int WoodCost;

        public bool CanGatherStuff;

        public static void InitUnitTemplates()
        {
            Pracant = new UnitTemplate("Pracant", "Pracant je nejdůležitější jednotka. Může sbírat suroviny a stavět a opravovat budovy. Pracanty nabíráš z {b}kuchyně{/b}.", TextureName.PracantLogo, TextureName.KidBroken, Sprite.Pracant, 50, 0)
            {
                CanBuildStuff = true,
                CanGatherStuff = true,
                CanAttack = false,
                EncyclopediaFilename = "Pracant"
            };
            Hadrakostrelec = new UnitTemplate("Hadrákostřelec", "Hadrákostřelec je základní bojová jednotka. Hází po nepřátelích přesné papírové míčky z dálky. Dá se nabrat z {b}muničního stanu{/b}.", TextureName.HadrakometLogo, TextureName.KidBroken, Sprite.Kid, 80, 20)
            {
                EncyclopediaFilename = "Hadrakostrelec"
            };
        }

        internal void PlayBuildSound()
        {
            if (this.CanBuildStuff)
            {
                SFX.Play(Build);
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

        internal void PlayGatherSound(EntityKind kind)
        {
            SFX.LastUnitSelectedXTimes = 0;
            switch (kind)
            {
                case EntityKind.BerryBush:
                    SFX.Play(GatherBerries);
                    break;
                case EntityKind.Corn:
                    SFX.Play(GatherCorn);
                    break;
                case EntityKind.UntraversableTree:
                    SFX.Play(GatherWood);
                    break;
                case EntityKind.MudMine:
                    SFX.Play(GatherMud);
                    break;
            }
        }

        internal void PlayUnitCreatedSound()
        {
            SFX.Play(UnitCreated);
        }
        internal void PlayAttackSound()
        {
            SFX.LastUnitSelectedXTimes = 0;
            SFX.PlayRandom(AckAttack, AckAttack2, Ack1, Ack2);
        }
    }
}