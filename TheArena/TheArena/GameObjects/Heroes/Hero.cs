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
using TheArena.Interfaces;
//using GameEngine.Pathfinding;

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
        public float Strength { get; set; }

        /// <summary>
        /// Heroes dexterity, effects dodge chance (NOT IMPLEMENTED YET) and critical strike chance (NOT IMPLEMENTED YET)
        /// </summary>
        public float Dexterity { get; set; }

        /// <summary>
        /// Heroes wisdom, effects amount of mana hero has
        /// </summary>
        public float Wisdom { get; set; }

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

        private List<Entity> _prevIntersectingEntities;
        private List<Entity> _prevAttackedEntities;
        private SoundEffect[] _onHitSfx;
        private SoundEffect[] _onDeathSfx;

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
            _prevAttackedEntities = new List<Entity>();

            Strength = 10;
            Dexterity = 10;
            Wisdom = 10;

            UpdateMaxHP();
            HP = MaxHP;

            UpdateMaxMana();
            Mana = MaxMana;

            XP = 0;

            LightSource = new LightSource();
            LightSource.Width = 32 * 8;
            LightSource.Height = 32 * 8;
            LightSource.Color = Color.White;
            LightSource.PositionType = LightPositionType.Relative;
        }

        public override void PostInitialize(GameTime gameTime, TeeEngine engine)
        {
            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.LightSources.Add(LightSource);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _onHitSfx = new SoundEffect[3];
            _onHitSfx[0] = content.Load<SoundEffect>("Sounds/Characters/Hit_Hurt14");
            _onHitSfx[1] = content.Load<SoundEffect>("Sounds/Characters/Hit_Hurt6");
            _onHitSfx[2] = content.Load<SoundEffect>("Sounds/Characters/Hit_Hurt11");

            _onDeathSfx = new SoundEffect[2];
            _onDeathSfx[0] = content.Load<SoundEffect>("Sounds/Characters/Death/revenge1");
            _onDeathSfx[1] = content.Load<SoundEffect>("Sounds/Characters/Death/death1");
        }

        // TODO REMOVE.
        private bool ContainsItem(string[] array, string item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == item) return true;

            return false;
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
                MouseState mouseState = Mouse.GetState();
                KeyboardState keyboardState = Keyboard.GetState();

                Vector2 movement = Vector2.Zero;
                float prevX = Pos.X;
                float prevY = Pos.Y;

                Tile prevTile = engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
                float moveSpeedModifier = prevTile.GetProperty<float>("MoveSpeed", 1.0f);

                // Restore opacity after a hit
                if (Opacity < 1)
                {
                    Opacity += 0.02f;
                    if (Opacity > 1) Opacity = 1;
                }

                // ATTACK KEY.
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    bool reset = !CurrentDrawableState.StartsWith(AttackType);
                    CurrentDrawableState = AttackType + Direction;

                    if (reset) Drawables.ResetState(CurrentDrawableState, gameTime);
                }
                else
                {
                    // MOVEMENT BASED KEYBOARD EVENTS.
                    if (keyboardState.IsKeyDown(Keys.Up))
                    {
                        CurrentDrawableState = "Walk_Up";
                        Direction = Direction.Up;

                        movement.Y--;
                    }
                    if (keyboardState.IsKeyDown(Keys.Down))
                    {
                        CurrentDrawableState = "Walk_Down";
                        Direction = Direction.Down;

                        movement.Y++;
                    }
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        CurrentDrawableState = "Walk_Left";
                        Direction = Direction.Left;

                        movement.X--;
                    }
                    if (keyboardState.IsKeyDown(Keys.Right))
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
                }
                
                // Change the radius of the LightSource overtime using a SINE wave pattern.
                LightSource.Pos = Pos;
                LightSource.Width = (int)(32 * (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3)));
                LightSource.Height = (int)(32 * (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3)));
                  
                _prevIntersectingEntities = engine.Collider.GetIntersectingEntites(this.CurrentBoundingBox);

                foreach (Entity entity in _prevIntersectingEntities)
                {
                    if (!CurrentDrawableState.Contains(AttackType) && _prevAttackedEntities.Contains(entity))
                    {
                        _prevAttackedEntities.Remove(entity);
                    }
                    if (entity is IAttackable && entity != this)
                    {
                        if (CurrentDrawableState.Contains(AttackType) &&
                            !_prevAttackedEntities.Contains(entity) &&
                            Entity.IntersectsWith(this, "Weapon", entity, "Body", gameTime))
                        {
                            ((IAttackable)entity).onHit(this, RollForDamage(), gameTime);
                            _prevAttackedEntities.Add(entity);
                        }
                    }
                }
            }

            base.Update(gameTime, engine);
        }

        public override void onHit(Entity source, int damage, GameTime gameTime)
        {
            if (HP <= 0) return;

            base.onHit(source, damage, gameTime);

            if (HP <= 0)
            {
                HP = 0;
                CurrentDrawableState = "Hurt";
                Drawables.ResetState("Hurt", gameTime);

                _onDeathSfx[randomGenerator.Next(0, _onDeathSfx.Length)].Play(0.2f, 0.0f, 0.0f);
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
            float percentMax = (MaxHP == 0) ? 1f : (HP / MaxHP);
            MaxHP = 105 + (int)(Strength * 2);
            HP = (int)(MaxHP * percentMax);
        }

        public void UpdateMaxMana()
        {
            // Keep the Mana at the same % of max hp after changing MaxMana
            float percentMax = (MaxMana == 0) ? 1f : (Mana / MaxMana);
            MaxMana = (int)(Wisdom * 3);
            Mana = (int)(MaxMana * percentMax);
        }
    }
}
