using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using TheArena.Shaders;
using TheArena.GameObjects.Mobs;
using System.ComponentModel;
using GameEngine.Extensions;

namespace TheArena.GameObjects.Heroes
{
    public class Hero : NPC
    {
        private const int BASE_DMG = 35;
        private const int BASE_DMG_RANGE = 4;
        private const int INPUT_DELAY = 0;
        private const float MOVEMENT_SPEED = 2.9f;

        public LightSource LightSource { get; set; }

        /// <summary>
        /// Heroes strength, effects damage dealt for melee attack and hitpoints
        /// </summary>
        public float Strength 
        {
            get { return _strength; }
            set
            {
                _strength = value;
                NotifyPropertyChanged("Strength");
            }
        }
        private float _strength;

        /// <summary>
        /// Heroes dexterity, effects dodge chance (NOT IMPLEMENTED YET) and critical strike chance (NOT IMPLEMENTED YET)
        /// </summary>
        public float Dexterity 
        {
            get { return _dexterity; }
            set
            {
                _dexterity = value;
                NotifyPropertyChanged("Dexterity");
            }
        }
        private float _dexterity;

        /// <summary>
        /// Heroes wisdom, effects amount of mana hero has
        /// </summary>
        public float Wisdom 
        {
            get { return _wisdom; }
            set
            {
                _wisdom = value;
                NotifyPropertyChanged("Wisdom");
            }
        }
        private float _wisdom;

        /// <summary>
        /// The current amount of mana the Hero has to cast spells
        /// </summary>
        public int Mana { get; set; }

        /// <summary>
        /// The current maximum amount of mana the hero has.
        /// </summary>
        public int MaxMana { get; set; }
        
        /// <summary>
        /// The basic attack type of the hero: Thrust, Slash
        /// </summary>
        public string AttackType { get; set; }

        /// <summary>
        /// Intensity represents how much stress the hero is under. 
        /// A high intensity value will lower the spawn rate of mobs while a low intensity
        /// value will allow mobs to spawn at their normal rates.
        /// </summary>
        public int Intensity { get; set; }

        private SoundEffect[] _onHitSfx;
        private SoundEffect[] _onDeathSfx;

        // List of Entities hit during an attack cycle.
        List<Entity> _hitEntityList = new List<Entity>();

        public Hero() : base(NPC.RACE_HUMAN_MALE)
        {
            Construct(0, 0);
        }

        public Hero(string race)
            :base(race)
        {
            Construct(0, 0);
        }

        public Hero(float x, float y, string race) :
            base(x, y, race)
        {
            Construct(x, y);
        }

        private void Construct(float x, float y)
        {
            Strength = 10;
            Dexterity = 10;
            Wisdom = 10;

            UpdateMaxHP();
            HP = MaxHP;

            UpdateMaxMana();
            Mana = MaxMana;

            XP = 0;
            Faction = "Allies";

            LightSource = new LightSource();
            LightSource.Width = 32 * 8;
            LightSource.Height = 32 * 8;
            LightSource.Color = Color.White;
            LightSource.PositionType = LightPositionType.Relative;
            CollisionGroup = "Shadow";
        }

        public override void PostCreate(GameTime gameTime, TeeEngine engine)
        {
            engine.AddEntity(LightSource);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _onHitSfx = new SoundEffect[3];
            _onHitSfx[0] = content.Load<SoundEffect>("Sounds/Characters/Hit/Hit_14");
            _onHitSfx[1] = content.Load<SoundEffect>("Sounds/Characters/Hit/Hit_6");
            _onHitSfx[2] = content.Load<SoundEffect>("Sounds/Characters/Hit/Hit_11");

            _onDeathSfx = new SoundEffect[2];
            _onDeathSfx[0] = content.Load<SoundEffect>("Sounds/Characters/Death/revenge1");
            _onDeathSfx[1] = content.Load<SoundEffect>("Sounds/Characters/Death/death1");
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (HP <= 0)
            {
                // Hero is dead :(
                // Make sure the correct animation is playing
                if (!CurrentDrawableState.Equals("Hurt"))
                {
                    Drawables.ResetState("Hurt", gameTime);
                    CurrentDrawableState = "Hurt";
                }

                // Reset the opacity
                if(Opacity < 1) Opacity = 1f;

            }
            else
            {
                KeyboardState keyboardState = Keyboard.GetState();

                Vector2 movement = Vector2.Zero;
                float prevX = Pos.X;
                float prevY = Pos.Y;

                Tile prevTile = engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
                float moveSpeedModifier = prevTile.GetProperty<float>("MoveSpeed", 1.0f);

                // TODO: Improve, we are retrieving this twice because it is called again in the CollidableEntity loop.
                List<Entity> intersectingEntities = engine.Collider.GetIntersectingEntites(CurrentBoundingBox);

                if (CurrentDrawableState.Contains("Slash")
                    && !Drawables.IsStateFinished(CurrentDrawableState, gameTime))
                {
                    foreach (Entity entity in intersectingEntities)
                    {
                        if (this != entity && entity is NPC && !_hitEntityList.Contains(entity))
                        {
                            NPC entityNPC = (NPC)entity;
                            if (entityNPC.Faction != this.Faction)
                            {
                                _hitEntityList.Add(entityNPC);
                                entityNPC.OnHit(this, RollForDamage(), gameTime, engine);
                            }
                        }                        
                    }
                }
                else
                {
                    _hitEntityList.Clear();

                    if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.Space, engine, true))
                    {
                        CurrentDrawableState = "Slash_" + Direction;
                        Drawables.ResetState(CurrentDrawableState, gameTime);
                    }
                    else
                    {
                        // Interaction
                        if (KeyboardExtensions.GetKeyDownState(keyboardState, Keys.E, engine, true))
                        {
                            foreach (Entity entity in intersectingEntities)
                            {
                                if (entity != this && entity is NPC)
                                {
                                    NPC entityNPC = (NPC)entity;
                                    if (entityNPC.Faction == this.Faction)
                                        entityNPC.OnInteract(this, gameTime, engine);
                                }

                            }
                        }

                        // MOVEMENT BASED KEYBOARD EVENTS.
                        if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                        {
                            CurrentDrawableState = "Walk_Up";
                            Direction = Direction.Up;

                            movement.Y--;
                        }
                        if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                        {
                            CurrentDrawableState = "Walk_Down";
                            Direction = Direction.Down;

                            movement.Y++;
                        }
                        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                        {
                            CurrentDrawableState = "Walk_Left";
                            Direction = Direction.Left;

                            movement.X--;
                        }
                        if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                        {
                            CurrentDrawableState = "Walk_Right";
                            Direction = Direction.Right;

                            movement.X++;
                        }

                        // Set animation to idle of no movements where made.
                        if (movement.Length() == 0)
                            CurrentDrawableState = "Idle_" + Direction;
                        else
                        {
                            movement.Normalize();
                            Pos += movement * MOVEMENT_SPEED * moveSpeedModifier;
                        }

                        LightSource.Pos = this.Pos;
                    }
                }

                if (Opacity < 1.0f)
                    Opacity += 0.02f;

                if (Opacity > 1.0f)
                    Opacity = 1.0f;
            }

            base.Update(gameTime, engine);
        }

        public override void OnHit(Entity sender, int damage, GameTime gameTime, TeeEngine engine)
        {
            base.OnHit(sender, damage, gameTime, engine);

            if (HP <= 0)
            {
                if (!CurrentDrawableState.Contains("Hurt"))
                {
                    CurrentDrawableState = "Hurt";
                    Drawables.ResetState("Hurt", gameTime);
                    _onDeathSfx[randomGenerator.Next(0, _onDeathSfx.Length)].Play(0.5f, 0.0f, 0.0f);
                }
            }
            else
            {
                // Increase intensity based on how much damage is done
               Intensity += damage;

                // Gfx & Sfx
                Opacity = 0.5f;
                _onHitSfx[randomGenerator.Next(0, _onHitSfx.Length)].Play(0.5f, 0.0f, 0.0f);
            }
        }

        private int RollForDamage()
        {
            return randomGenerator.Next(BASE_DMG + (int)(Strength * 2) - BASE_DMG_RANGE, BASE_DMG + (int)(Strength * 2) + BASE_DMG_RANGE + 1);
        }

        public void UpdateMaxHP()
        {
            // Keep the HP at the same % of max hp after changing MaxHP
            float percentMax = (MaxHP == 0) ? 1f : ((float)HP / (float)MaxHP);
            MaxHP = 105 + (int)(Strength * 2);
            HP = (int)(MaxHP * percentMax);
        }

        public void UpdateMaxMana()
        {
            // Keep the Mana at the same % of max hp after changing MaxMana
            float percentMax = (MaxMana == 0) ? 1f : ((float)Mana / (float)MaxMana);
            MaxMana = (int)(Wisdom * 3);
            Mana = (int)(MaxMana * percentMax);
        }
    }
}
