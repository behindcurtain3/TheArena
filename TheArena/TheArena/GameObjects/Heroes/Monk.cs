using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheArena.Items;

namespace TheArena.GameObjects.Heroes
{
    public class Monk : Hero
    {

        public Monk() : base(NPC.RACE_HUMAN_MALE)
        {
            Equip(ItemRepository.GameItems["RobeSkirt"]);
            Equip(ItemRepository.GameItems["RobeShirt"]);
            Equip(ItemRepository.GameItems["WoodenStaff"]);
            AttackType = "Thrust_";

            // LevelUp to lvl 1
            Level = 0;
            LevelUp();
        }

        public override void LevelUp()
        {
            base.LevelUp();

            int mod = Level - 1;

            Strength = 8 + (mod * 1);
            Dexterity = 14 + (mod * 3);
            Wisdom = 8 + (mod * 2);

            UpdateMaxHP();
            UpdateMaxMana();
        }
    }
}
