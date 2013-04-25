using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheArena.GameObjects.Heroes
{
    public class DeathKnight : Hero
    {

        public DeathKnight() : base()
        {
            //Head = NPC.PLATE_ARMOR_HEAD;
            Legs = NPC.PLATE_ARMOR_LEGS;
            Feet = NPC.PLATE_ARMOR_FEET;
            Shoulders = NPC.PLATE_ARMOR_SHOULDERS;
            Torso = NPC.PLATE_ARMOR_TORSO;
            Hands = NPC.PLATE_ARMOR_HANDS;
            Belt = null;
            Bracers = null;

            Weapon = NPC.WEAPON_LONGSWORD;

            // DEATH
            BaseRace = NPC.RACE_UNDEAD;

            // Base stats
            Lvl = 0;
            LevelUp();

            HP = MaxHP;
            Mana = MaxMana;
        }

        public override void LevelUp()
        {
            base.LevelUp();

            int mod = Lvl - 1;

            Strength = 12 + (mod * 3);
            Dexterity = 6 + (mod * 1);
            Wisdom = 12 + (mod * 2);

            UpdateMaxHP();
            UpdateMaxMana();
        }
    }
}
