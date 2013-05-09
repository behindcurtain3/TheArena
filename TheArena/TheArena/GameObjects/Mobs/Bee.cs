using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheArena.GameObjects.Attacks;
using TheArena.GameObjects.Heroes;
using Microsoft.Xna.Framework;

namespace TheArena.GameObjects.Mobs
{
    public class Bee : Mob
    {

        public Bee()
            : base(NPC.CREATURES_BEE)
        {
            Construct();
        }

        public Bee(float x, float y)
            : base(x, y, NPC.CREATURES_BEE)
        {
            Construct();
        }

        private void Construct()
        {
            WorthGold = 1;

            MaxHP = 50;
            HP = MaxHP;
            Damage = 15;

            AttackDistance = 200;
            AggroDistance = 250;
        }

        public override void onAttack(GameEngine.TeeEngine engine, GameTime gameTime)
        {
            BeeProjectile attack = new BeeProjectile(Pos.X, Pos.Y, AttackTarget);
            attack.Damage = Damage;
            engine.AddEntity(attack);
            
            Stance = AttackStance.Retreating;
        }  

    }
}
