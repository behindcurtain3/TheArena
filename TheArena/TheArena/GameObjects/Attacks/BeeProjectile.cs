using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;

namespace TheArena.GameObjects.Attacks
{
    public class BeeProjectile : Entity
    {
        public float Speed { get; set; }
        public Entity Target { get; set; }
        public Vector2 MoveTo { get; set; }
        public int Damage { get; set; }

        private double _moveAngle;
        private double _attackDistance = 300;

        public BeeProjectile(float x, float y, Entity target) : base(x, y)
        {
            this.Target = target;
            this.MoveTo = target.Pos;

            // Shoot in the direction of the target entity, but make sure the projectile travels _attackDistance
            // in case it misses
            _moveAngle =  Math.Atan2(
                            target.Pos.Y - this.Pos.Y,
                            target.Pos.X - this.Pos.X
                            );

            double distance = Vector2.Distance(target.Pos, Pos);
            Vector2 realTarget = MoveTo;

            realTarget.X += (float)(Math.Cos(_moveAngle) * (_attackDistance - distance));
            realTarget.Y += (float)(Math.Sin(_moveAngle) * (_attackDistance - distance));

            MoveTo = realTarget;

            Speed = 6f;
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            DrawableSet.LoadDrawableSetXml(Drawables, "Animations/Monsters/bee_attack_projectile.draw", content);
            CurrentDrawableState = "Fly";           
            Drawables.SetGroupProperty("Body", "Rotation", (float)_moveAngle);

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            if (Entity.IntersectsWith(this, "Body", Target, "Body", gameTime))
            {
                // Target is hit
                if (Target is NPC)
                {
                    NPC target = (NPC)Target;
                    target.OnHit(this, Damage, gameTime, engine);
                }

                engine.RemoveEntity(this);
            }
            else
            {
                double distance = Vector2.Distance(Pos, MoveTo);

                if (distance < 10)
                {
                    // TODO check for attack succeeding
                    engine.RemoveEntity(this);
                }
                else
                {
                    Pos.X += (float)(Math.Cos(_moveAngle) * Speed);
                    Pos.Y += (float)(Math.Sin(_moveAngle) * Speed);
                }
            }
        }
    }
}
