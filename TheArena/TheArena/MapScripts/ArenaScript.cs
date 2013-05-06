using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Interfaces;
using GameEngine;
using GameEngine.Tiled;
using Microsoft.Xna.Framework;
using GameEngine.GameObjects;
using GameEngine.Extensions;
using Microsoft.Xna.Framework.Input;
using TheArena.GameObjects.Heroes;
using TheArena.GameObjects.Mobs;
using TheArena.GameObjects;

namespace TheArena.MapScripts
{
    public class ArenaScript : IMapScript
    {
        public static Random randomGenerator = new Random();

        private int _intensityReductionTimer = 0;
        private int _intensityReductionDuration = 500;
        private int _mobSpawnTimer = 0;
        private int _mobSpawnDelay = 4000;
        private List<MobSpawner> _spawners;
        private bool _mapLoaded = false;

        public void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs args)
        {
            engine.GetPostGameShader("LightShader").Enabled = false;

            // The player should start with zero intensity
            ((Hero)engine.GetEntity("Player")).Intensity = 0;

            _spawners = new List<MobSpawner>();

            List<Entity> entities = new List<Entity>(engine.GetEntities());
            foreach (Entity e in entities)
            {
                if (e is MobSpawner)
                    _spawners.Add((MobSpawner)e);
            }

            _mapLoaded = true;
        }

        public void MapUnloaded(TeeEngine engine, TiledMap map)
        {
            
        }

        public void Update(TeeEngine engine, GameTime gameTime)
        {
            if (!_mapLoaded) return;

            Hero player = (Hero)engine.GetEntity("Player");
            bool reduceIntensity = true;
            
            foreach (Entity entity in engine.EntitiesOnScreen)
            {
                if (entity is Mob)
                {
                    Mob mob = (Mob)entity;

                    // If any mob on screen is attacking the player, don't reduce their intensity value
                    if (mob.Stance == Mob.AttackStance.Attacking)
                    {
                        reduceIntensity = false;
                        break;
                    }
                }
            }

            if (reduceIntensity)
            {
                _intensityReductionTimer += gameTime.ElapsedGameTime.Milliseconds;

                // Intensity should reduce at a rate of 90pts per 30 sec or 3pts per second
                if (_intensityReductionTimer >= _intensityReductionDuration)
                {
                    player.Intensity--;
                    if (player.Intensity < 0) player.Intensity = 0;

                    _intensityReductionTimer = 0;
                }
            }

            // Check to spawn new mobs
            _mobSpawnTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (_mobSpawnTimer >= _mobSpawnDelay && player.Intensity < 30)
            {
                // Spawn a new mob on a random mob spawner
                int spawner = randomGenerator.Next(0, _spawners.Count);

                _spawners[spawner].SpawnMob(engine);
                _mobSpawnTimer = 0;

            }
        }

        public void NorthBridge_MapZoneHit(MapZone sender, Entity entity, TeeEngine engine, GameTime gameTime)
        {
            // Return if the bridge is already destroyed
            if (!(entity is Hero))
                return;

            // If it was the player that hit the zone, destroy the bridge
            
            TileLayer layer = engine.Map.GetLayerByName("Bridges and Items");

            if (layer != null)
            {
                layer[22, 10] = 58;
                layer[22, 11] = 59;
                layer[22, 12] = 60;
                layer[22, 13] = 59;
                Tile tile = engine.Map.GetTxTopMostTile(22, 13);
                tile.SetProperty("Impassable", "true");

                // Remove the zone that triggers this event
                engine.RemoveEntity("NorthBridgeExit");

                // Rebuild the pathfinding nodes
                //engine.Pathfinding.RebuildNeighbors(engine.Map);
            }
        }
    }
}
