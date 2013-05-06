using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using System;
using TheArena.Interfaces;
using TheArena.Items;
using System.Collections.Generic;

namespace TheArena.GameObjects
{
    public enum Direction { Up, Right, Down, Left };

    public class NPC : CollidableEntity, IAttackable
    {
        
        public static Random randomGenerator = new Random();

        public const string RACE_HUMAN_MALE = @"Animations/Characters/male_npc.anim";
        public const string RACE_HUMAN_FEMALE = @"Animations/Characters/female_npc.anim";
        public const string RACE_UNDEAD = @"Animations/Characters/skeleton.anim";

        public Direction Direction { get; set; }

        public List<Item> Backpack { get; set; }
        public Dictionary<ItemType, Item> Equiped { get; set; }
        public string BaseRace { get; set; }

        // Stats
        public int Lvl { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int XP { get; set; }
        public int XPToNextLevel { get; set; }
        public int Gold { get; set; }

        public NPC()
        {
            Construct(0, 0, RACE_HUMAN_MALE);
        }

        public NPC(string baseRace)
        {
            Construct(0, 0, baseRace);
        }

        public NPC(float x, float y, string baseRace)
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
            this.Gold = 0;

            this.Pos = new Vector2(x, y);
            this.ScaleX = 1.0f;
            this.ScaleY = 1.0f;
            this.CollisionGroup = "Shadow";
            this.Backpack = new List<Item>();
            this.Equiped = new Dictionary<ItemType, Item>();

            this.Direction = Direction.Right;
            this.BaseRace = baseRace;
        }

        public override void LoadContent(ContentManager content)
        {
            DrawableSet.LoadDrawableSetXml(Drawables, BaseRace, content);
            CurrentDrawableState = "Idle_Right";
        }

        public virtual void onHit(Entity source, int damage, GameTime gameTime)
        {
            HP -= damage;
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

        #region Equipment Methods

        public void Equip(Item item)
        {
            Unequip(item.ItemType);

            Equiped[item.ItemType] = item;
            Drawables.Union(item.Drawables);
        }

        public void Unequip(Item item)
        {
            Unequip(item.ItemType);
        }

        public void Unequip(ItemType itemType)
        {
            if (Equiped.ContainsKey(itemType))
                Drawables.Remove(Equiped[itemType].Drawables);

            Equiped.Remove(itemType);
        }

        public bool IsEquiped(Item item)
        {
            if (Equiped.ContainsKey(item.ItemType))
                return Equiped[item.ItemType] == item;
            else
                return false;
        }

        #endregion
    }
}
