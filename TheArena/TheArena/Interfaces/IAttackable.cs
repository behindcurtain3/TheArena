using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;

namespace TheArena.Interfaces
{
    public interface IAttackable
    {
        int HP { get; set; }

        void onHit(Entity source, int damage);

    }
}
