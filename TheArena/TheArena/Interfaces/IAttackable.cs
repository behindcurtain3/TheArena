using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;

namespace TheArena.Interfaces
{
    public interface IAttackable
    {
        public int HP { get; set; }

        public void onHit(Entity source, int damage);

    }
}
