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
        private Component _prevFocus;
        public Component ActiveComponent
        {
            get { return _prevFocus; }
        }

        private Component _clickFocus;
        private bool _isDragActive = false;

        public ArenaUI(Game game) : base(game)
        {
            _components = new Dictionary<string, Component>();

            _input = new InputState();
            _prevScreenArea = new Rectangle();

            game.Components.Add(this);
        }

        public void AddComponent(string name, Component comp)
        {
            if(!_components.ContainsKey(name))
                _components.Add(name, comp);
        }

        public bool RemoveComponent(string name)
        {
            if (_components.ContainsKey(name))
            {
                Component comp = _components[name];

                if (_prevFocus == comp)
                {
                    _prevFocus = null;
                    _isDragActive = false;
                }

                if (_clickFocus == comp)
                    _clickFocus = null;
            }

            return _components.Remove(name);
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

            MouseState currentMouse = _input.CurrentMouseState;
            MouseState prevMouse = _input.LastMouseState;           
            
            Component currentFocus = null;
            foreach (string node in _components.Keys)
            {
                currentFocus = _components[node].IsFocused(currentMouse.X, currentMouse.Y, _prevScreenArea);
                if (currentFocus != null)
                    break;
            }

            // If the focus changed, inject mouse over/out events
            if (_prevFocus != currentFocus && !_isDragActive)
            {
                if (_prevFocus != null)
                    _prevFocus.InjectMouseOut(this, currentMouse);

                if (currentFocus != null)
                    currentFocus.InjectMouseOver(this, currentMouse);
            }
            // They are the same, check for mouse move events
            else
            {
                if (currentFocus != null && (currentMouse.X != prevMouse.X || currentMouse.Y != prevMouse.Y) && !_isDragActive)
                    currentFocus.InjectMouseMove(this, currentMouse);
            }

            // Check for mouse down, on a mouse down event set _clickFocus to the current focused component
            if (currentFocus != null && currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                _clickFocus = currentFocus;
                _clickFocus.InjectMouseDown(this, currentMouse);
                _isDragActive = true;
            }

            // Check for mouse up / click events
            if (currentMouse.LeftButton == ButtonState.Released && prevMouse.LeftButton != ButtonState.Released)
            {
                if (_isDragActive)
                {
                    _prevFocus.InjectDragEnd(this, currentMouse);
                    _isDragActive = false;
                }

                // If the component clicked is still the current focus send click event
                if (_clickFocus != null && _clickFocus == currentFocus)
                    _clickFocus.InjectMouseClick(this, currentMouse);

                if (_clickFocus != currentFocus)
                {
                    if (_clickFocus != null)
                        _clickFocus.InjectMouseUp(this, currentMouse);
                }

                if (currentFocus != null)
                    currentFocus.InjectMouseUp(this, currentMouse);
            }

            if (_isDragActive)
            {
                _prevFocus.InjectDrag(this, currentMouse);
            }
            else
            {
                _prevFocus = currentFocus;
            }

            foreach (string comp in _components.Keys)
                _components[comp].Update(this, gameTime, _input);
        }

        public void RenderUI(SpriteBatch spriteBatch, GameTime gameTime, Rectangle screenArea)
        {
            // Sort the components by their layer
            _components.OrderBy(x => x.Value.Layer);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, null);

            // Draw each component
            foreach (string comp in _components.Keys)
                _components[comp].Draw(spriteBatch, screenArea, gameTime);

            foreach (string comp in _components.Keys)
                if (_components[comp].DrawToolTip(spriteBatch, screenArea, gameTime))
                    break;

            spriteBatch.End();

            _prevScreenArea = screenArea;
        }
    }
}
