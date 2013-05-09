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
        private Rectangle _prevScreenArea;

        public ArenaUI(Game game) : base(game)
        {
            _components = new Dictionary<string, Component>();

            _input = new InputState();
            _prevScreenArea = new Rectangle();
        }

        public void AddComponent(string name, Component comp)
        {
            if(!_components.ContainsKey(name))
                _components.Add(name, comp);
        }

        public Component GetComponent(string name)
        {
            // I split these because its faster to just look the component
            // is in the top level. Only traverse the "tree" if necessary
            if (_components.ContainsKey(name))
                return _components[name];
            else
            {
                // Search each component children recursively
                return GetComponent(name, _components.Values.ToList());
            }
        }

        private Component GetComponent(string name, List<Component> components)
        {
            foreach (Component c in components)
            {
                if (c.Name.Equals(name))
                    return c;

                else if (c.Children.Count > 0)
                {
                    Component comp = GetComponent(name, c.Children);

                    if (comp != null)
                        return comp;
                }
            }

            // Return null if not found
            return null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _input.Update();

            foreach (string comp in _components.Keys)
                // if the component handles the input stop trying to process more
                if (_components[comp].HandleInput(_input, _prevScreenArea))
                    break;

            foreach (string comp in _components.Keys)
                _components[comp].Update(this, gameTime);
        }

        public void RenderUI(SpriteBatch spriteBatch, GameTime gameTime, Rectangle screenArea)
        {
            // Sort the components by their layer
            _components.OrderBy(x => x.Value.Layer);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, null);

            // Draw each component
            foreach (string comp in _components.Keys)
                _components[comp].Draw(spriteBatch, screenArea);

            foreach (string comp in _components.Keys)
                if (_components[comp].DrawToolTip(spriteBatch, screenArea))
                    break;

            spriteBatch.End();

            _prevScreenArea = screenArea;
        }
    }
}
