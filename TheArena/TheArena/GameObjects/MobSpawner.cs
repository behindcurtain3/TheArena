﻿using System;
using System.Collections.Generic;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using Microsoft.Xna.Framework.Graphics;
using TheArena.GameObjects.Mobs;
using GameEngine.Interfaces;

namespace TheArena.GameObjects
{
    public class MobSpawner : Entity, ISizedEntity
    {
        public static Random randomGenerator = new Random();

        public float SpawnInterval { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Interval_Min { get; set; }
        public float Interval_Max { get; set; }
        public string Mobs 
        {
            get
            {
                return _splitMobs.ToString();
            }
            set
            {
                _splitMobs = new List<string>(value.Split(','));
            }
        }

        private float _deltaTime;
        private List<string> _splitMobs;

        public MobSpawner()
        {
            Construct(0, 0);
        }

        public MobSpawner(float x, float y, int w, int h, string types, float minInterval, float maxInterval) : base(x, y)
        {
            Construct(x, y, w, h, types, minInterval, maxInterval);                        
        }

        private void Construct(float x, float y, int width = 10, int height = 10, string types = "", float minInterval = 10, float maxInterval = 25)
        {
            Mobs = types;
            Interval_Max = maxInterval;
            Interval_Min = minInterval;
            Width = width;
            Height = height;

            _deltaTime = 0;
            SetSpawnInterval();
        }

        public override void PostInitialize(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            // Make sure to reset the spawn interval again, this ensures the min & max have been set by the engine
            SetSpawnInterval();

            base.PostInitialize(gameTime, engine);
        }

        public override void LoadContent(ContentManager content)
        {
            StaticImage image = new StaticImage(
                content.Load<Texture2D>("Misc/Zone"),
                new Rectangle(0, 0, Width, Height));
            image.Origin = new Vector2(0, 0);

            GameDrawableInstance instance = Drawables.Add("Standard", image, "Body", 0);
            instance.Color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            instance.Visible = false;

            CurrentDrawableState = "Standard";
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            _deltaTime += gameTime.ElapsedGameTime.Milliseconds;

            if (_deltaTime >= SpawnInterval)
            {
                string type = _splitMobs[randomGenerator.Next(0, _splitMobs.Count)];
                Vector2 pos = new Vector2(Pos.X + (CurrentBoundingBox.Width * (float)randomGenerator.NextDouble()), Pos.Y + (CurrentBoundingBox.Height * (float)randomGenerator.NextDouble()));
                Mob mob;

                switch (type)
                {
                    case "Bat":
                        mob = new Bat(pos.X, pos.Y);
                        break;
                    case "Bee":
                        mob = new Bee(pos.X, pos.Y);
                        break;
                    default:
                        mob = new Bee(pos.X, pos.Y);
                        break;
                }

                engine.AddEntity(mob);
                _deltaTime = 0;
                SetSpawnInterval();
            }

            base.Update(gameTime, engine);
        }

        private void SetSpawnInterval()
        {
            SpawnInterval = ((float)randomGenerator.NextDouble() * (Interval_Max - Interval_Min) + Interval_Min) * 1000;
        }

    }
}
