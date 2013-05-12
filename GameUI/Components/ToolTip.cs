using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameUI.Input;

namespace GameUI.Components
{
    public class ToolTip : FrameComponent
    {

        public string Text { get; set; }
        public string FlavorText { get; set; }
        public Color FlavorColor { get; set; }
        public SpriteFont FlavorFont { get; set; }

        private int _mouseOverDelay = 500; // in milliseconds
        private int _durationOfMouseOver = 0;
        private Vector2 _mousePosition;
        private bool _showFlavorText = false;
        private int _mouseOverFlavorDelay = 1200;
        private int _durationOfFlavor = 0;

        public ToolTip()
        {
            Visible = false;
            PaddingTop = 10;
            PaddingLeft = 10;
            PaddingRight = 10;
            PaddingBottom = 10;

            FlavorColor = new Color(180, 180, 180);
        }

        public override void Update(ArenaUI hud, GameTime dt, InputState input)
        {
            if (Parent == null || Text == null || Text.Equals(String.Empty))
                return;

            if (Parent.IsMouseOver)
            {
                _durationOfMouseOver += dt.ElapsedGameTime.Milliseconds;

                if(FlavorText != null && !FlavorText.Equals(String.Empty))
                    _durationOfFlavor += dt.ElapsedGameTime.Milliseconds;

                if (_durationOfMouseOver >= _mouseOverDelay)
                    Visible = true;

                if (_durationOfFlavor >= _mouseOverFlavorDelay)
                    _showFlavorText = true;
            }
            else
            {
                Visible = false;
                _durationOfMouseOver = 0;

                _showFlavorText = false;
                _durationOfFlavor = 0;
            }

            _mousePosition.X = input.CurrentMouseState.X;
            _mousePosition.Y = input.CurrentMouseState.Y;

            int width;
            int height;

            if (_showFlavorText)
            {
                width = Math.Max((int)Font.MeasureString(Text).X, (int)FlavorFont.MeasureString(FlavorText).X);
                width = Math.Max(100, width + FrameLeft + FrameRight + PaddingLeft + PaddingRight);

                height = Math.Max(15, (int)Font.MeasureString(Text).Y + (int)FlavorFont.MeasureString(FlavorText).Y + 4 + PaddingBottom + FrameTop + FrameBottom);
            }
            else
            {
                width = Math.Max(100, (int)Font.MeasureString(Text).X + 20);
                height = Math.Max(15, (int)Font.MeasureString(Text).Y + PaddingBottom + FrameTop + FrameBottom);
            }

            Position = new Rectangle((int)_mousePosition.X + 10, (int)_mousePosition.Y, width, height);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            if (Parent == null || !Visible || Text == null || Text.Equals(String.Empty))
                return;

            base.Draw(spriteBatch, parent);
            spriteBatch.DrawString(Font, Text, new Vector2(ContentPane.X, ContentPane.Y), Color);

            if (_showFlavorText)
                spriteBatch.DrawString(FlavorFont, FlavorText, new Vector2(ContentPane.X, ContentPane.Y + Font.MeasureString(Text).Y + 4), FlavorColor);
        }
    }
}
