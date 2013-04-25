using System;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using TheArena.GameObjects.Heroes;

namespace TheArena.GameObjects
{
    public enum CoinType { Gold, Silver, Copper };

    public class Coin : Entity
    {
        public int CoinValue { get; set; }

        public SoundEffect CoinSound { get; set; }
        public enum State { Spawning, Active };

        public State CoinState { get; set; }
        public Vector2 SpawnTarget { get; internal set; }

        public CoinType CoinType
        {
            get { return _coinType; }
            set
            {
                CurrentDrawableState = value.ToString();
                _coinType = value;
            }
        }

        private CoinType _coinType;

        public Coin(float x, float y, int coinValue, CoinType coinType, float targetX = 0, float targetY = 0)
            : base(x, y)
        {
            this.CoinType = coinType;
            this.ScaleX = 0.7f;
            this.ScaleY = 0.7f;
            this.CoinValue = coinValue;

            if (targetX != 0 && targetY != 0)
            {
                SpawnTarget = new Vector2(targetX, targetY);
                CoinState = State.Spawning;
            }
            else
            {
                CoinState = State.Active;
            }
        }

        public override void LoadContent(ContentManager content)
        {
            // Load the coin animation.
            Animation.LoadAnimationXML(this.Drawables, "Animations/Misc/coin.anim", content);

            CoinSound = content.Load<SoundEffect>("Sounds/Coins/coin1");
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            Hero player = (Hero)engine.GetEntity("Player");
            Vector2 target;
            if (CoinState == State.Spawning)
            {
                target = SpawnTarget;
                float distance = Vector2.Distance(Pos, SpawnTarget);

                if (distance < 2.5)
                {
                    CoinState = State.Active;
                    Drawables.SetGroupProperty("Body", "Offset", Vector2.Zero);
                }
            }
            else
            {
                target = player.Pos;
            }
                
            if (IsOnScreen)
            {
                float COIN_MOVE_SPEED = 5000;
                float TERMINAL_VELOCITY = 5;

                // Find the distance between the player and this coin.
                float distanceSquared = Vector2.DistanceSquared(Pos, target);

                float speed = COIN_MOVE_SPEED / distanceSquared;  // Mangitude of velocity.
                speed = Math.Min(speed, TERMINAL_VELOCITY);

                if (speed > 0.5)
                {
                    // Calculate the angle between the player and the coin.
                    double angle = Math.Atan2(
                        target.Y - this.Pos.Y,
                        target.X - this.Pos.X
                        );

                    this.Pos.X += (float)(Math.Cos(angle) * speed);        // x component.
                    this.Pos.Y += (float)(Math.Sin(angle) * speed);        // y component.

                    // Check to see if coin can be considered collected.
                    if (Entity.IntersectsWith(this, "Shadow", player, "Shadow", gameTime))
                    {
                        CoinSound.Play(0.05f, 0.0f, 0.0f);
                        player.Coins += this.CoinValue;
                        engine.RemoveEntity(this);
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format(
                "Coin: Name={0}, CoinValue={1}, CoinType={2}, Pos={3}",
                Name,
                CoinValue,
                CoinType,
                Pos);

        }
    }
}
