using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            this.onMouseOver += new OnMouseOverEventHandler(Button_onMouseOver);
            this.onMouseOut += new OnMouseOutEventHandler(Button_onMouseOut);
            this.onMouseDown += new OnMouseDownEventHandler(Button_onMouseDown);
            this.onMouseUp += new OnMouseUpEventHandler(Button_onMouseUp);
        }            

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            base.Draw(spriteBatch, parent);

            if (Text != null)
            {
                float x = parent.X + ContentPane.X + (ContentPane.Width / 2) - (Font.MeasureString(Text).X / 2);
                float y = parent.Y + ContentPane.Y + (ContentPane.Height / 2) - (Font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(Font, Text, new Vector2(x + 1, y + 1), Color.Black);
                spriteBatch.DrawString(Font, Text, new Vector2(x, y), TextColor);
            }
        }

        void Button_onMouseOver(object sender)
        {
            // TODO: Take out magic numbers
            SourceOffset = new Rectangle(32, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);

            IsMouseOver = true;
        }

        void Button_onMouseOut(object sender)
        {
            SourceOffset = new Rectangle(0, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);

            IsMouseOver = false;
        }

        void Button_onMouseDown(Microsoft.Xna.Framework.Input.MouseState state)
        {
            SourceOffset = new Rectangle(64, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
        }

        void Button_onMouseUp(Microsoft.Xna.Framework.Input.MouseState state)
        {
            if(IsMouseOver)
                SourceOffset = new Rectangle(32, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
            else
                SourceOffset = new Rectangle(0, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
        }

    }
}
