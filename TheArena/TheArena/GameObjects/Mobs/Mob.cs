using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using System;
using TheArena.GameObjects.Heroes;
using TheArena.GameObjects.Attacks;
using TheArena.GameObjects.Misc;

namespace TheArena.GameObjects.Mobs
{
    public class Mob : NPC
    {
        public enum AttackStance { NotAttacking, Preparing, Attacking, Retreating };
        
        private const int ATTACK_COUNTER_LIMIT = 40;

        public int Lvl { get; set; }
        public int Damage { get; set; }
        public int WorthGold { get; set; }

        public double AttackDistance { get; set; }
        public double AggroDistance { get; set; }
        public Entity AttackTarget { get; set; }
        public AttackStance Stance { get; set; }
        public string Prefix { get; set; }
        
        private bool _xpGiven = false;
        private bool _goldGiven = false;

        private int _attackCounter = 0;
        private Vector2 _attackHeight = Vector2.Zero;
        private double _attackAngle = 0;
        private double _randomModifier;
        private float _attackSpeed = 5.4f;
        private float _moveSpeed = 1.8f;
        private bool _attackHit = false;

        public Mob() : base(NPC.CREATURES_BAT)
        {
            Construct(0, 0, NPC.CREATURES_BAT);
        }

        public Mob(string type)
        {
            Construct(0, 0, type);
        }

        public Mob(float x, float y, string type)
            : base(type)
        {
            Construct(x, y, type);
        }

        public void Construct(float x, float y, string type)
        {
            Lvl = 1;
            HP = Lvl * 55;
            Damage = 25;
            Pos = new Vector2(x, y);
            BaseRace = type;
            Faction = "Creatures";
            Prefix = "Fly_";

            _randomModifier = randomGenerator.NextDouble();
            _xpGiven = false;
            _goldGiven = false;

            AttackDistance = 40;
            AggroDistance = 200;
            Stance = AttackStance.NotAttacking;
            EntityCollisionEnabled = false;
            TerrainCollisionEnabled = false;
        }

        public override void LoadContent(ContentManager content)
        {
            // Load in the animation
            DrawableSet.LoadDrawableSetXml(Drawables, BaseRace, content);

            CurrentDrawableState = Prefix + "Down";
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            // Get the Hero player for interaction purposes.
            Hero player = (Hero)engine.GetEntity("Player");
            Vector2 prevPos = Pos;
            AttackTarget = player;

            // Check if this Bat has died.
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
                // ATTACKING LOGIC.
                if (Stance == AttackStance.Attacking)
                {
                    this.Pos.X -= (float)(Math.Cos(_attackAngle) * _attackSpeed);
                    this.Pos.Y -= (float)(Math.Sin(_attackAngle) * _attackSpeed);
                    this._attackHeight.Y += 30.0f / ATTACK_COUNTER_LIMIT;
                    this.Drawables.SetGroupProperty("Body", "Offset", _attackHeight);

                    onAttack(engine, gameTime);

                }
                // ATTACK PREPERATION LOGIC.
                else if (Stance == AttackStance.Preparing)
                {
                    _attackHeight.Y -= 2;

                    if (_attackHeight.Y < -40)
                    {
                        _attackHeight.Y = -40;
                        _attackAngle = Math.Atan2(
                            this.Pos.Y - player.Pos.Y,
                            this.Pos.X - player.Pos.X
                            );
                        Stance = AttackStance.Attacking;
                        _attackCounter = 0;
                    }

                    Drawables.SetGroupProperty("Body", "Offset", _attackHeight);
                }
                // NON-ATTACKING LOGIC. PATROL AND APPROACH.
                else if (Stance == AttackStance.NotAttacking)
                {
                    double distance = Vector2.Distance(player.Pos, this.Pos);

                    if (distance < AggroDistance && player.HP > 0)
                    {
                        // Move towards the player for an attack move.
                        double angle = Math.Atan2(
                            player.Pos.Y - this.Pos.Y,
                            player.Pos.X - this.Pos.X
                            );

                        // Approach Function.
                        double moveValue;
                        if (distance < AttackDistance)
                        {
                            Stance = AttackStance.Preparing;
                            moveValue = 0;
                        }
                        else
                            moveValue = _moveSpeed;

                        Pos.X += (float)(Math.Cos(angle) * moveValue);
                        Pos.Y += (float)(Math.Sin(angle) * moveValue);
                    }
                    else
                    {
                        // Perform a standard patrol action.
                        Pos.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds - _randomModifier * 90) * 2);
                    }
                }
                else if (Stance == AttackStance.Retreating)
                {
                    double distance = Vector2.Distance(player.Pos, this.Pos);

                    if (distance < AggroDistance * 1.10)
                    {
                        if (_attackHeight.Y < -10)
                        {
                            _attackHeight.Y += 0.4f;
                            if (_attackHeight.Y > -10)
                                _attackHeight.Y = -10;

                            Drawables.SetGroupProperty("Body", "Offset", _attackHeight);
                        }

                        double angle = Math.Atan2(
                            player.Pos.Y - this.Pos.Y,
                            player.Pos.X - this.Pos.X
                            );

                        Pos.X += (float)(Math.Cos(angle) * -(_moveSpeed * 0.8f));
                        Pos.Y += (float)(Math.Sin(angle) * -(_moveSpeed * 0.8f));
                    }
                    else
                    {
                        Stance = AttackStance.NotAttacking;
                        if (_attackHeight.Y < -10) _attackHeight.Y = -10;
                        Drawables.SetGroupProperty("Body", "Offset", _attackHeight);
                    }
                }

                // Determine the animation based on the change in position.
                if (Math.Abs(prevPos.X - Pos.X) > Math.Abs(prevPos.Y - Pos.Y))
                {
                    if (prevPos.X < Pos.X)
                        this.CurrentDrawableState = Prefix + "Right";
                    if (prevPos.X > Pos.X)
                        this.CurrentDrawableState = Prefix + "Left";
                }
                else
                {
                    if (prevPos.Y < Pos.Y)
                        this.CurrentDrawableState = Prefix + "Down";
                    if (prevPos.Y > Pos.Y)
                        this.CurrentDrawableState = Prefix + "Up";
                }
            }


            base.Update(gameTime, engine);
        }

        public override void OnHit(Entity sender, int damage, GameTime gameTime, GameEngine.TeeEngine engine)
        {
            base.OnHit(sender, damage, gameTime, engine);

            if (HP <= 0 && !_xpGiven && sender is Hero)
            {
                ((Hero)sender).RewardXP(this, Lvl, gameTime, engine);
                _xpGiven = true;
            }
        }

        /// <summary>
        /// Do an attack
        /// </summary>
        public virtual void onAttack(GameEngine.TeeEngine engine, GameTime gameTime)
        {
            if (!_attackHit && Entity.IntersectsWith(this, "Shadow", AttackTarget, "Shadow", gameTime))
            {
                if (AttackTarget is NPC)
                {
                    NPC target = (NPC)AttackTarget;
                    target.OnHit(this, Damage, gameTime, engine);
                }
                _attackHit = true;
            }

            if (_attackCounter++ == ATTACK_COUNTER_LIMIT)
            {
                Stance = AttackStance.NotAttacking;
                _attackHit = false;
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
