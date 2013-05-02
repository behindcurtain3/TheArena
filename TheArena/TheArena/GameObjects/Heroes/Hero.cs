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

        public bool CollisionDetection { get; set; }
        public LightSource LightSource { get; set; }

        // Hero stats
        public float Strength { get; set; }
        public float Dexterity { get; set; }
        public float Wisdom { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }

        // Meta information used by game
        public int Intensity { get; set; }

        private List<Entity> _prevIntersectingEntities;
        private List<Entity> _prevAttackedEntities;
        private SoundEffect[] _onHitSfx;
        private SoundEffect[] _onDeathSfx;

        //public Path CurrentPath { get; set; }

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

            CollisionDetection = true;
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
                    CurrentDrawableState = "Hurt";

                // Reset the opacity
                if(Opacity < 1) Opacity = 1f;

            }
            else
            {
                MouseState mouseState = Mouse.GetState();
                KeyboardState keyboardState = Keyboard.GetState();
                /*
                if (mouseState.RightButton == ButtonState.Pressed)
                {
                    int tileX = (int)mouseState.X / engine.Map.TileWidth;
                    int tileY = (int)mouseState.Y / engine.Map.TileHeight;
                    ANode target = engine.Map.Nodes[tileX, tileY];

                    tileX = (int)Pos.X / engine.Map.TileWidth;
                    tileY = (int)Pos.Y / engine.Map.TileHeight;
                    ANode start = engine.Map.Nodes[tileX, tileY];

                    CurrentPath = AStar.GeneratePath(start, target);
                } */

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
                    bool reset = !CurrentDrawableState.StartsWith("Slash");
                    CurrentDrawableState = "Slash_" + Direction;

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

                // Prevent from going out of range.
                if (Pos.X < 0) Pos.X = 0;
                if (Pos.Y < 0) Pos.Y = 0;
                if (Pos.X >= engine.Map.pxWidth - 1) Pos.X = engine.Map.pxWidth - 1;
                if (Pos.Y >= engine.Map.pxHeight - 1) Pos.Y = engine.Map.pxHeight - 1;

                if (CollisionDetection)
                {
                    // Iterate through each layer and determine if the tile is passable.
                    int tileX = (int)Pos.X / engine.Map.TileWidth;
                    int tileY = (int)Pos.Y / engine.Map.TileHeight;

                    int pxTileX = tileX * engine.Map.TileWidth;
                    int pxTileY = tileY * engine.Map.TileHeight;
                    int pxTileWidth = engine.Map.TileWidth;
                    int pxTileHeight = engine.Map.TileHeight;

                    Tile currentTile = engine.Map.GetPxTopMostTile(Pos.X, Pos.Y);
                    bool impassable = currentTile.HasProperty("Impassable");

                    // CORRECT ENTRY AND EXIT MOVEMENT BASED ON TILE PROPERTIES
                    // TODO
                    // to improve structure
                    // Current very very ineffecient way of checking Entry
                    string[] entryPoints = currentTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] exitPoints = prevTile.GetProperty("Entry", "Top Bottom Left Right").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    bool top = prevY < pxTileY;
                    bool bottom = prevY > pxTileY + pxTileHeight;
                    bool left = prevX < pxTileX;
                    bool right = prevX > pxTileX + pxTileWidth;

                    // Ensure entry points.
                    impassable |= top && !ContainsItem(entryPoints, "Top");
                    impassable |= bottom && !ContainsItem(entryPoints, "Bottom");
                    impassable |= left && !ContainsItem(entryPoints, "Left");
                    impassable |= right && !ContainsItem(entryPoints, "Right");

                    // Ensure exit points.
                    impassable |= top && !ContainsItem(exitPoints, "Bottom");
                    impassable |= bottom && !ContainsItem(exitPoints, "Top");
                    impassable |= left && !ContainsItem(exitPoints, "Right");
                    impassable |= right && !ContainsItem(exitPoints, "Left");

                    // IF THE MOVEMENT WAS DEEMED IMPASSABLE, CORRECT IT.
                    // if impassable, adjust X and Y accordingly.
                    float padding = 0.001f;
                    if (impassable)
                    {
                        if (prevY <= pxTileY && Pos.Y > pxTileY)
                            Pos.Y = pxTileY - padding;
                        else
                            if (prevY >= pxTileY + pxTileHeight && Pos.Y < pxTileY + pxTileHeight)
                                Pos.Y = pxTileY + pxTileHeight + padding;

                        if (prevX <= pxTileX && Pos.X > pxTileX)
                            Pos.X = pxTileX - padding;
                        else
                            if (prevX >= pxTileX + pxTileWidth && Pos.X < pxTileX + pxTileWidth)
                                Pos.X = pxTileX + pxTileWidth + padding;
                    }

                    // Change the radius of the LightSource overtime using a SINE wave pattern.
                    LightSource.Pos = Pos;
                    LightSource.Width = (int)(32 * (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3)));
                    LightSource.Height = (int)(32 * (8.0f + 0.5 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3)));

                    // EXAMPLE OF HOW THE QUAD TREE INTERSECTING ENTITIES FUNCTION CAN WORK
                    // TODO: Add PER PIXEL collision detection to each one of these entities
                    //if (prevIntersectingEntities != null)
                    //    foreach (Entity entity in prevIntersectingEntities)
                    //        entity.Opacity = 1.0f;

                    _prevIntersectingEntities = engine.Collider.GetIntersectingEntites(this.CurrentBoundingBox);

                    foreach (Entity entity in _prevIntersectingEntities)
                    {
                        if (!CurrentDrawableState.Contains("Slash") && _prevAttackedEntities.Contains(entity))
                        {
                            _prevAttackedEntities.Remove(entity);
                        }
                        if (entity is IAttackable && entity != this)
                        {
                            if (CurrentDrawableState.Contains("Slash") &&
                                !_prevAttackedEntities.Contains(entity) &&
                                Entity.IntersectsWith(this, "Weapon", entity, "Body", gameTime))
                            {
                                ((IAttackable)entity).onHit(this, RollForDamage(), gameTime);
                                _prevAttackedEntities.Add(entity);
                            }
                        }
                    }
                }
            }
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
