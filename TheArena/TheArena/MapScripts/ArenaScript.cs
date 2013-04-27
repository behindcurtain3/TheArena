﻿using System;
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

namespace TheArena.MapScripts
{
    public class ArenaScript : IMapScript
    {
        private bool _northHit = false;

        public void MapLoaded(TeeEngine engine, TiledMap map, MapEventArgs args)
        {
            engine.GetPostGameShader("LightShader").Enabled = false;
        }

        public void MapUnloaded(TeeEngine engine, TiledMap map)
        {
            
        }

        public void Update(TeeEngine engine, GameTime gameTime)
        {
            
        }

        public void NorthBridge_MapZoneHit(MapZone sender, Entity entity, TeeEngine engine, GameTime gameTime)
        {
            // Return if the bridge is already destroyed
            if (_northHit || !(entity is Hero))
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

                _northHit = true;
            }
        }
    }
}
