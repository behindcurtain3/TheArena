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
using TheArena.GameObjects.Misc;
using GameUI;
using GameUI.Components;
using TheArena.Items;
using TheArena.GameObjects;
using TheArena.HUD;

namespace TheArena.MapScripts
{
    public class ArenaScript : IMapScript
    {
        public static Random randomGenerator = new Random();

        public ArenaUI Hud;

        private int _intensityReductionTimer = 0;
        private int _intensityReductionDuration = 500;
        private int _mobSpawnTimer = 0;
        private int _mobSpawnDelay = 4000;
        private List<MobSpawner> _spawners;
        private bool _mapLoaded = false;
        private bool _isGameOver = false;
        private bool _gameOverLoaded = false;

        private Hero _player;

        public void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs args)
        {
            engine.GetPostGameShader("LightShader").Enabled = false;

            _player = (Hero)engine.GetEntity("Player");

            // The player should start with zero intensity
            _player.Intensity = 0;
            _spawners = new List<MobSpawner>();

            List<Entity> entities = new List<Entity>(engine.GetEntities());
            foreach (Entity e in entities)
            {
                if (e is MobSpawner)
                    _spawners.Add((MobSpawner)e);
            }

            // Setup and load the UI
            Hud = new ArenaUI(engine.Game);

            List<Component> hudComponents = Component.LoadComponentsFromXml("HUD/Elements/Components.ui", engine.Game.Content);
            foreach (Component c in hudComponents)
                Hud.AddComponent(c.Name, c);

            // Bind data to components
            if (_player != null)
            {
                Label label = (Label)Hud.GetComponent("HeroLevel");
                if (label != null)
                    label.SetDataBinding("Level", _player);

                label = (Label)Hud.GetComponent("HeroStrength");
                if (label != null)
                    label.SetDataBinding("Strength", _player);

                label = (Label)Hud.GetComponent("HeroDexterity");
                if (label != null)
                    label.SetDataBinding("Dexterity", _player);

                label = (Label)Hud.GetComponent("HeroWisdom");
                if (label != null)
                    label.SetDataBinding("Wisdom", _player);

                Button cheat = (Button)Hud.GetComponent("CheatButton");
                if(cheat != null)
                    cheat.onMouseClick += new Component.MouseEventHandler(delegate(Component sender, MouseState mouse)
                    {
                        _player.LevelUp();
                        _player.HP = _player.MaxHP;
                    });

                _player.onItemEquipped += new NPC.OnItemEquippedEventHandler(NPC_onItemEquiped);
                _player.onItemUnEquipped += new NPC.OnItemUnEquippedEventHandler(NPC_onItemUnEquiped);
                _player.onDeath += new NPC.OnDeathEventHandler(NPC_onDeath);

                // Load the currently equipped items
                foreach (KeyValuePair<ItemType, Item> pair in _player.Equiped)
                {
                    NPC_onItemEquiped(_player, pair.Value);
                }
            }

            _mapLoaded = true;
        }

        public void MapUnloaded(TeeEngine engine, TiledMap map)
        {
            
        }

        public void Update(TeeEngine engine, GameTime gameTime)
        {
            if (!_mapLoaded) return;

            if (_isGameOver)
            {
                if (!_gameOverLoaded)
                {
                    List<Component> hudComponents = Component.LoadComponentsFromXml("HUD/Elements/GameOver.ui", engine.Game.Content);
                    foreach (Component c in hudComponents)
                        Hud.AddComponent(c.Name, c);

                    _gameOverLoaded = true;

                    Button restart = (Button)Hud.GetComponent("RestartButton");
                    restart.onMouseClick += new Component.MouseEventHandler(delegate(Component sender, MouseState mouse)
                    {
                        engine.ClearEntities();
                        engine.LoadMap("Content/Maps/arena.tmx");
                    });

                    Button exit = (Button)Hud.GetComponent("QuitButton");
                    exit.onMouseClick += new Component.MouseEventHandler(delegate(Component sender, MouseState mouse)
                    {
                        engine.Game.Exit();
                    });
                }
            }
            else
            {
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
                    List<Entity> entities = new List<Entity>(engine.GetEntities());
                    List<Entity> mobs = entities.FindAll(delegate(Entity e) { return e is Mob; });

                    // Only have up to x mobs at once
                    if (mobs.Count < 15)
                    {

                        // Spawn a new mob on a random mob spawner
                        int spawner = randomGenerator.Next(0, _spawners.Count);

                        _spawners[spawner].SpawnMob(engine);
                    }

                    // Reset the timer regardless, just check again later
                    _mobSpawnTimer = 0;
                }
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
                engine.Pathfinding.RebuildNeighbors(engine.Map);
            }
        }

        public void NPC_onItemEquiped(NPC sender, Item item)
        {
            ItemSlot slot = (ItemSlot)Hud.GetComponent("Equipped" + item.ItemType);

            if (slot != null)
            {
                slot.Item = item;
                slot.ToolTip.Text = item.FriendlyName;
                slot.ToolTip.FlavorText = item.Description;
            }
        }

        public void NPC_onItemUnEquiped(NPC sender, Item item)
        {
            ItemSlot slot = (ItemSlot)Hud.GetComponent("Equipped" + item.ItemType);

            if (slot != null)
            {
                slot.Item = null;
                slot.ToolTip.Text = null;
                slot.ToolTip.FlavorText = null;
            }
        }

        public void NPC_onDeath(NPC sender)
        {
            _isGameOver = true;
        }
    }
}
