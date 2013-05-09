using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using System;
using TheArena.Items;
using System.Collections.Generic;
using System.ComponentModel;
using GameEngine;

namespace TheArena.GameObjects
{
    public enum Direction { Up, Right, Down, Left };

    public class NPC : CollidableEntity, INotifyPropertyChanged
    {
        
        public static Random randomGenerator = new Random();

        public const string RACE_HUMAN_MALE = @"Animations/Characters/male_npc.anim";
        public const string RACE_HUMAN_FEMALE = @"Animations/Characters/female_npc.anim";
        public const string RACE_UNDEAD = @"Animations/Characters/skeleton.anim";

        public const string CREATURES_DUMMY = @"Animations/Monsters/combat_dummy.anim";
        public const string CREATURES_BAT = @"Animations/Monsters/bat.anim";
        public const string CREATURES_BEE = @"Animations/Monsters/bee.anim";

        public Direction Direction { get; set; }

        public List<Item> Backpack { get; set; }
        public Dictionary<ItemType, Item> Equiped { get; set; }
        public string BaseRace { get; set; }
        public string Faction { get; set; }

        // Stats
        private int _level;
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                NotifyPropertyChanged("Level");
            }
        }


        public int HP 
        {
            get { return _hp; }
            set
            {
                _hp = value;

                if (_hp <= 0)
                    _hp = 0;
            }
        }
        private int _hp = 100;


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
            this.Level = 1;
            this.HP = 100;
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

        /// <summary>
        /// Rewards the NPC with XP based on the level of the Mob killed.
        /// </summary>
        /// <param name="mobLevel">The level of the mob that is awarding XP.</param>
        public void RewardXP(int mobLevel)
        {
            if (mobLevel == Level)
            {
                XP += (Level * 5) + 45;
            }
            else if (mobLevel < Level)
            {
                XP += ((Level * 5) + 45) * (1 - (Level - mobLevel) / 5);
            }
            else // mobLevel > Level
            {
                XP += ((Level * 5) + 45) * (int)(1 + 0.05 * (mobLevel - Level));
            }

            // Check for lvl up condition
            if (XP >= XPToNextLevel)
            {
                LevelUp();
            }
        }

        public int GetNeededXpAmount()
        {
            return 40 * (Level * Level) + 360 * Level;
        }

        public virtual void LevelUp()
        {
            Level++;
            XPToNextLevel = GetNeededXpAmount();
        }

        #region Interaction Methods

        /// <summary>
        /// Method called when the NPC has been hit by some Entity residing within the
        /// game engine. Override this method in order to perform custom functionality
        /// during a Hit event.
        /// </summary>
        public virtual void OnHit(Entity sender, GameTime gameTime, TeeEngine engine)
        {
        }

        /// <summary>
        /// Method called when the NPC has been interacted with through some medium by
        /// another Entity object residing within the same engine. Override this method in
        /// order to allow interactions to occur with this entity.
        /// </summary>
        public virtual void OnInteract(Entity sender, GameTime gameTime, TeeEngine engine)
        {
            //SpeechBubble speech = new SpeechBubble(this, TimeSpan.FromSeconds(10));

            //engine.AddEntity(speech);
        }

        #endregion

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
