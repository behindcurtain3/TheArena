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
        public enum ButtonState { Normal, Selected, Disabled }


        public string Text { get; set; }
        public Color TextColor { get; set; }

        private ButtonState _state;
        public ButtonState State 
        {
            get { return _state; }
            set
            {
                _state = value;

                if (_state == ButtonState.Disabled)
                {
                    SourceOffset = new Rectangle(128, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
                    Enabled = false;
                }
                else if (_state == ButtonState.Selected)
                {
                    SourceOffset = new Rectangle(96, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
                    Enabled = true;
                }
                else
                {
                    if(IsMouseOver)
                        SourceOffset = new Rectangle(32, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
                    else
                        SourceOffset = new Rectangle(0, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
                    Enabled = true;
                }
            }
        }

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
            State = ButtonState.Normal;

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
            if(State != ButtonState.Disabled)
                SourceOffset = new Rectangle(32, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
        }

        void Button_onMouseOut(Component sender, MouseState mouse)
        {
            switch (State)
            {
                case ButtonState.Normal:
                    SourceOffset = new Rectangle(0, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
                    break;
                case ButtonState.Selected:
                    SourceOffset = new Rectangle(96, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
                    break;
                case ButtonState.Disabled:
                    SourceOffset = new Rectangle(128, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
                    break;
            }
        }

        void Button_onMouseDown(Component sender, MouseState mouse)
        {
            if (State != ButtonState.Disabled)
                SourceOffset = new Rectangle(64, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
        }

        void Button_onMouseUp(Component sender, MouseState mouse)
        {
            if (State != ButtonState.Disabled)
            {
                if (IsMouseOver)
                    SourceOffset = new Rectangle(32, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
                else
                    SourceOffset = new Rectangle(0, SourceOffset.Y, SourceOffset.Width, SourceOffset.Height);
            }
        }

    }
}
