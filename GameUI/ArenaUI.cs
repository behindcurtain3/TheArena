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
        public List<Component> Components { get; set; }

        private InputState _input;


        public ArenaUI(Game game) : base(game)
        {
            Components = new List<Component>();

            _input = new InputState();
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _input.Update();

            foreach (Component comp in Components)
                comp.HandleInput(_input);
        }

        public void RenderUI(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Sort the components by their layer
            Components.Sort(delegate(Component a, Component b) { return a.Layer.CompareTo(b.Layer); });


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, null);

            // Draw each component
            foreach (Component comp in Components)
                comp.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
