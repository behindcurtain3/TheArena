using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using System;

namespace TheArena.GameObjects
{
    public enum Direction { Up, Right, Down, Left };

    public class NPC : Entity
    {
        public static Random randomGenerator = new Random();

        public const string RACE_HUMAN_MALE = @"Animations/Characters/male_npc.anim";
        public const string RACE_HUMAN_FEMALE = @"Animations/Characters/female_npc.anim";
        public const string RACE_UNDEAD = @"Animations/Characters/skeleton.anim";

        // Armor animations
        public const string PLATE_ARMOR_LEGS = @"Animations/Plate Armor/plate_armor_legs.anim";
        public const string PLATE_ARMOR_TORSO = @"Animations/Plate Armor/plate_armor_torso.anim";
        public const string PLATE_ARMOR_FEET = @"Animations/Plate Armor/plate_armor_feet.anim";
        public const string PLATE_ARMOR_HANDS = @"Animations/Plate Armor/plate_armor_hands.anim";
        public const string PLATE_ARMOR_HEAD = @"Animations/Plate Armor/plate_armor_head.anim";
        public const string PLATE_ARMOR_SHOULDERS = @"Animations/Plate Armor/plate_armor_shoulders.anim";

        public const string ARMOR_ROBE_FEET = @"Animations/Armor/robe_feet.anim";
        public const string ARMOR_ROBE_HEAD = @"Animations/Armor/robe_head.anim";
        public const string ARMOR_ROBE_LEGS = @"Animations/Armor/robe_legs.anim";
        public const string ARMOR_ROBE_TORSO = @"Animations/Armor/robe_torso.anim";
        public const string ARMOR_ROBE_BELT = @"Animations/Armor/robe_belt.anim";

        public const string WEAPON_DAGGER = @"Animations/Weapons/dagger.anim";
        public const string WEAPON_LONGSWORD = @"Animations/Weapons/longsword.anim";

        public Direction Direction { get; set; }

        public string Weapon { get; set; }
        public string Head { get; set; }
        public string Torso { get; set; }
        public string Feet { get; set; }
        public string Shoulders { get; set; }
        public string Legs { get; set; }
        public string Hands { get; set; }
        public string Belt { get; set; }
        public string Bracers { get; set; }
        public string BaseRace { get; set; }

        public int Lvl { get; set; }
        
        public int HP { get; set; }
        public int MaxHP { get; set; }

        public int XP { get; set; }
        public int XPToNextLevel { get; set; }
        
        public int Coins { get; set; }

        public NPC()
        {
            Construct(0, 0, RACE_HUMAN_MALE);
        }

        public NPC(string baseRace)
        {
            Construct(0, 0, baseRace);
        }

        public NPC(float x, float y, string baseRace) :
            base(x, y, 1.0f, 1.0f)
        {
            Construct(x, y, baseRace);
        }

        private void Construct(float x, float y, string baseRace)
        {
            this.Lvl = 1;
            this.HP = 0;
            this.MaxHP = 0;
            this.XP = 0;
            this.XPToNextLevel = GetNeededXpAmount();
            this.Coins = 0;

            this.Direction = Direction.Right;
            this.BaseRace = baseRace;
        }

        public override void LoadContent(ContentManager content)
        {
            if (Weapon != null) Animation.LoadAnimationXML(Drawables, Weapon, content);
            if (Shoulders != null) Animation.LoadAnimationXML(Drawables, Shoulders, content);
            if (Head != null) Animation.LoadAnimationXML(Drawables, Head, content);
            if (Hands != null) Animation.LoadAnimationXML(Drawables, Hands, content);
            if (Feet != null) Animation.LoadAnimationXML(Drawables, Feet, content);
            if (Torso != null) Animation.LoadAnimationXML(Drawables, Torso, content);
            if (Legs != null) Animation.LoadAnimationXML(Drawables, Legs, content);
            if (Belt != null) Animation.LoadAnimationXML(Drawables, Belt, content);
            if (Bracers != null) Animation.LoadAnimationXML(Drawables, Bracers, content);
            Animation.LoadAnimationXML(Drawables, BaseRace, content);

            CurrentDrawableState = "Walk_Right";
        }

        public override void UnloadContent()
        {

        }

        /// <summary>
        /// Rewards the NPC with XP based on the level of the Mob killed.
        /// </summary>
        /// <param name="mobLevel">The level of the mob that is awarding XP.</param>
        public void RewardXP(int mobLevel)
        {
            if (mobLevel == Lvl)
            {
                XP += (Lvl * 5) + 45;
            }
            else if (mobLevel < Lvl)
            {
                XP += ((Lvl * 5) + 45) * (1 - (Lvl - mobLevel) / 5);
            }
            else // mobLevel > Lvl
            {
                XP += ((Lvl * 5) + 45) * (int)(1 + 0.05 * (mobLevel - Lvl));
            }

            // Check for lvl up condition
            if (XP >= XPToNextLevel)
            {
                LevelUp();
            }
        }

        public int GetNeededXpAmount()
        {
            return 40 * (Lvl * Lvl) + 360 * Lvl;
        }

        public virtual void LevelUp()
        {
            Lvl++;
            XPToNextLevel = GetNeededXpAmount();
        }
    }
}
