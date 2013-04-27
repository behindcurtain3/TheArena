using GameEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace TheArena.Interfaces
{
    public interface IAttackable
    {
        int HP { get; set; }

        void onHit(Entity source, int damage, GameTime gameTime);

    }
}
