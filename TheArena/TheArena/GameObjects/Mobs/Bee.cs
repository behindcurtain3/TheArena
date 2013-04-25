using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheArena.GameObjects.Mobs
{
    public class Bee : Mob
    {
        public Bee()
            : base(Mob.BEE)
        {
        }

        public Bee(float x, float y)
            : base(x, y, Mob.BEE)
        {
            WorthGold = 1;
            
            MaxHP = 50;
            HP = MaxHP;
            Damage = 35;
        }

    }
}
