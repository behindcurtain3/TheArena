using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameUI.Components;
using GameUI.Input;

namespace GameUI
{
    public class ArenaUI : GameComponent
    {
        private Dictionary<string, Component> _components;

        private InputState _input;


        public ArenaUI(Game game) : base(game)
        {
            _components = new Dictionary<string, Component>();

            _input = new InputState();
        }

        public void AddComponent(string name, Component comp)
        {
            if(!_components.ContainsKey(name))
                _components.Add(name, comp);
        }

        public Component GetComponent(string name)
        {
            return _components[name];
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _input.Update();

            foreach (string comp in _components.Keys)
                _components[comp].HandleInput(_input);
        }

        public void RenderUI(SpriteBatch spriteBatch, GameTime gameTime, Rectangle screenArea)
        {
            // Sort the components by their layer
            _components.OrderBy(x => x.Value.Layer);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, null);

            // Draw each component
            foreach (string comp in _components.Keys)
                _components[comp].Draw(spriteBatch, screenArea);

            spriteBatch.End();
        }
    }
}
