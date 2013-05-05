using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheArena.GameObjects.Heroes
{
    public class Monk : Hero
    {

        public Monk() : base()
        {
            Head = null;// NPC.ARMOR_ROBE_HEAD;
            Legs = NPC.ARMOR_ROBE_LEGS;
            Feet = null;
            Torso = NPC.ARMOR_ROBE_TORSO;
            Hands = null;
            Shoulders = null;
            Belt = NPC.ARMOR_ROBE_BELT;
            Bracers = null;

            Weapon = NPC.WEAPON_STAFF;
            AttackType = "Thrust_";

            // LevelUp to lvl 1
            Lvl = 0;
            LevelUp();
        }

        public override void LevelUp()
        {
            base.LevelUp();

            int mod = Lvl - 1;

            Strength = 8 + (mod * 1);
            Dexterity = 14 + (mod * 3);
            Wisdom = 8 + (mod * 2);

            UpdateMaxHP();
            UpdateMaxMana();
        }
    }
}
