using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheArena.Items;

namespace TheArena.GameObjects.Heroes
{
    public class DeathKnight : Hero
    {

        public DeathKnight() : base()
        {
            Equip(ItemRepository.GameItems["PlateBoots"]);
            Equip(ItemRepository.GameItems["PlateGloves"]);
            Equip(ItemRepository.GameItems["PlatePants"]);
            Equip(ItemRepository.GameItems["PlateChest"]);
            Equip(ItemRepository.GameItems["Longsword"]);

            BaseRace = NPC.RACE_UNDEAD;

            // LevelUp to lvl1
            Level = 0;
            LevelUp();
        }

        public override void LevelUp()
        {
            base.LevelUp();

            int mod = Level - 1;

            Strength = 12 + (mod * 3);
            Dexterity = 6 + (mod * 1);
            Wisdom = 12 + (mod * 2);

            UpdateMaxHP();
            UpdateMaxMana();
        }
    }
}
