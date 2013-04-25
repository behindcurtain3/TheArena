using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using System;

namespace TheArena.GameObjects.Mobs
{
    public class Mob : Entity
    {
        public const string BAT = @"Animations/Monsters/bat.anim";
        public const string BEE = @"Animations/Monsters/bee.anim";

        private static Random randomGenerator = new Random();

        public string Type { get; set; }
        public int Lvl { get; set; }
        public int HP { get; internal set; }
        public int MaxHP { get; internal set; }
        public int Damage { get; set; }
        public int WorthGold { get; set; }
        
        private bool _xpGiven = false;
        private bool _goldGiven = false;
        private double _randomModifier;

        public Mob()
        {
            Construct(0, 0, BAT);
        }

        public Mob(string type)
        {
            Construct(0, 0, type);
        }

        public Mob(float x, float y, string type)
            : base(x, y)
        {
            Construct(x, y, type);
        }

        public void Construct(float x, float y, string type)
        {
            Lvl = 1;
            Pos = new Vector2(x, y);
            Type = type;

            _randomModifier = randomGenerator.NextDouble();
            _xpGiven = false;
            _goldGiven = false;
        }

        public override void LoadContent(ContentManager content)
        {
            // Load in the animation
            Animation.LoadAnimationXML(Drawables, Type, content);

            CurrentDrawableState = "Down";

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            // Check if this Mob has died.
            if (HP <= 0)
            {
                this.Opacity -= 0.02f;
                this.Drawables.ResetState(CurrentDrawableState, gameTime);

                if (this.Opacity < 0)
                {
                    if (!_goldGiven) SpawnCoins(engine);
                    engine.RemoveEntity(this);
                }
            }
            else
            {
                // TODO write logic
                // Wander
                Pos.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds - _randomModifier * 90) * 2);
            }


            base.Update(gameTime, engine);
        }

        public void DealDamage(NPC source, int amount)
        {
            HP -= amount;

            if (HP <= 0 && !_xpGiven)
            {
                source.RewardXP(Lvl);
                _xpGiven = true;
            }
            
        }

        /// <summary>
        /// Spawns coins around the mob
        /// </summary>
        /// <param name="engine">The current engine, needed to add new coins entities to the game.</param>
        public void SpawnCoins(GameEngine.TeeEngine engine)
        {
            // Spawn gold
            float range = CurrentBoundingBox.Width * 2;
            int num = randomGenerator.Next(0, 6);
            float minX = Pos.X - range;
            float maxX = Pos.X + range;
            float minY = Pos.Y - range;
            float maxY = Pos.Y + range;

            for (int i = 0; i < num; i++)
            {
                int type = randomGenerator.Next(1, 10);
                float x = ((float)randomGenerator.NextDouble() * (maxX - minX) + minX);
                float y = ((float)randomGenerator.NextDouble() * (maxY - minY) + minY);
                Coin c;
                
                switch(type)
                {         
                    case 6:
                    case 7:
                    case 8:
                        c = new Coin(Pos.X, Pos.Y, WorthGold * 2, CoinType.Silver, x, y);
                        break;
                    case 9:
                        c = new Coin(Pos.X, Pos.Y, WorthGold * 3, CoinType.Gold, x, y);
                        break;
                    default:
                        c = new Coin(Pos.X, Pos.Y, WorthGold, CoinType.Copper, x, y);
                        break;
                }

                engine.AddEntity(c);
            }

            _goldGiven = true;
        }
    }
}
