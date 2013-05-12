using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameEngine.Drawing;
using GameEngine.Extensions;
using System.Collections.Generic;
using GameEngine;
using Microsoft.Xna.Framework.Audio;
using System;

namespace TheArena.GameObjects.Mobs
{
    public class CombatDummy : NPC
    {
        Random _randomGenerator;
        SoundEffect[] _hitSounds;

        public CombatDummy()
        {
            Construct();
        }

        void Construct()
        {
            this.BaseRace = NPC.CREATURES_DUMMY;
            this.Direction = Direction.Right;
            this._randomGenerator = new Random();
        }

        public override void LoadContent(ContentManager content)
        {
            _hitSounds = new SoundEffect[3];
            _hitSounds[0] = content.Load<SoundEffect>("Sounds/Characters/Hit/Hit_11");
            _hitSounds[1] = content.Load<SoundEffect>("Sounds/Characters/Hit/Hit_14");
            _hitSounds[2] = content.Load<SoundEffect>("Sounds/Characters/Hit/Hit_6");

            CurrentDrawableState = "Idle_" + Direction;

            base.LoadContent(content);
        }

        public override void OnHit(Entity sender, int damage, GameTime gameTime, TeeEngine engine)
        {
            HP = 10000; // Combat dummy always has tons of HP

            base.OnHit(sender, damage, gameTime, engine);

            if (!CurrentDrawableState.Contains("Spin"))
            {
                CurrentDrawableState = "Spin_" + Direction;
                Drawables.ResetState(CurrentDrawableState, gameTime);
            }

            // Play random Hit Sound.
            int index = _randomGenerator.Next(3);
            _hitSounds[index].Play();
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            if (CurrentDrawableState.Contains("Spin") && Drawables.IsStateFinished(CurrentDrawableState, gameTime))
            {
                Direction = (Direction)((int)(Direction + 1) % 4);
                CurrentDrawableState = "Idle_" + Direction;
            }
        }
    }
}
