using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameUI.Components
{
    public class TitleBar : FrameComponent
    {
        public string Text { get; set; }
        private Vector2 _mousePosition;

        public TitleBar() : base(Rectangle.Empty)
        {
            SetAllPadding(4);
        }

        public TitleBar(Rectangle position)
            : base(position)
        {
            SetAllPadding(4);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent, GameTime gameTime)
        {
            base.Draw(spriteBatch, parent, gameTime);

            spriteBatch.DrawString(Font, Text, new Vector2(ContentPane.X + parent.X, ContentPane.Y + parent.Y), Color);
        }

        public void TitleBar_onMouseDown(MouseState state)
        {
            _mousePosition = new Vector2(state.X - Parent.Position.X, state.Y - Parent.Position.Y - Position.Y);            
        }

        public void TitleBar_onDragEnd(object sender)
        {
        }

        public void TitleBar_onDrag(object sender)
        {
            MouseState mouse = (MouseState)sender;

            int x = (int)(mouse.X - _mousePosition.X);
            int y = (int)(mouse.Y - _mousePosition.Y);

            if (Parent != null)
            {
                Parent.Position = new Rectangle(x, y + Position.Height, Parent.Position.Width, Parent.Position.Height);
            }
        }
    }
}
