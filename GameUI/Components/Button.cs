using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameUI.Components
{
    public class Button : FrameComponent
    {

        public string Text { get; set; }
        public Color TextColor { get; set; }

        public Button()
        {
            Construct();
        }

        public Button(Rectangle position)
            : base(position)
        {
            Construct();
        }

        private void Construct()
        {            
            FrameTop = 5;
            FrameBottom = 5;
            FrameLeft = 5;
            FrameRight = 5;
            SourceOffset = new Rectangle(0, 0, 32, 32);
            TextColor = Color.White;

            this.onMouseOver += new MouseEventHandler(Button_onMouseOver);
            this.onMouseOut += new MouseEventHandler(Button_onMouseOut);
            this.onMouseDown += new MouseEventHandler(Button_onMouseDown);
            this.onMouseUp += new MouseEventHandler(Button_onMouseUp);
        }            

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent, GameTime gameTime)
        {
            if (!Visible) return;

            base.Draw(spriteBatch, parent, gameTime);

            if (Text != null)
            {
                float x = parent.X + ContentPane.X + (ContentPane.Width / 2) - (Font.MeasureString(Text).X / 2);
                float y = parent.Y + ContentPane.Y + (ContentPane.Height / 2) - (Font.MeasureString(Text).Y / 2);
                x = (float)Math.Round(x);
                y = (float)Math.Round(y);

                spriteBatch.DrawString(Font, Text, new Vector2(x + 1, y + 1), Color.Black);
                spriteBatch.DrawString(Font, Text, new Vector2(x, y), TextColor);
            }
        }

        void Button_onMouseOver(Component sender, MouseState mouse)
        {
            // TODO: Take out magic numbers
            SourceOffset = new Rectangle(32, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
        }

        void Button_onMouseOut(Component sender, MouseState mouse)
        {
            SourceOffset = new Rectangle(0, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
        }

        void Button_onMouseDown(Component sender, MouseState mouse)
        {
            SourceOffset = new Rectangle(64, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
        }

        void Button_onMouseUp(Component sender, MouseState mouse)
        {
            if(IsMouseOver)
                SourceOffset = new Rectangle(32, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
            else
                SourceOffset = new Rectangle(0, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
        }

    }
}
