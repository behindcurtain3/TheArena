using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheArena.GameObjects.Mobs
{
    public class Bat : Mob
    {

        public Bat(float x, float y)
            : base(x, y, NPC.CREATURES_BAT)
        {
            WorthGold = 1;
            MaxHP = 100;
            HP = MaxHP;
            Damage = 10;
        }
    }
}
