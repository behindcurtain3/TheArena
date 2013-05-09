using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameUI.Input;

namespace GameUI.Components
{
    public class MouseListener
    {
        public delegate void OnMouseOverEventHandler(object sender);
        public delegate void OnMouseOutEventHandler(object sender);
        public delegate void OnMouseClickEventHandler(object sender);
        public delegate void OnDragEventHandler(object sender);
        public delegate void OnDragEndEventHandler(object sender);
        public delegate void OnPositionChangedEventHandler(Rectangle position);

        public event OnMouseOverEventHandler onMouseOver;
        public event OnMouseOutEventHandler onMouseOut;
        public event OnMouseClickEventHandler onMouseClick;
        public event OnDragEventHandler onDrag;
        public event OnDragEndEventHandler onDragEnd;
        public event OnPositionChangedEventHandler onPositionChanged;

        private Rectangle _position;
        public Rectangle Position
        {
            get { return _position; }
            set
            {
                _position = value;

                _position.Width = Math.Max(_position.Width, MinimumWidth);
                _position.Height = Math.Max(_position.Height, MinimumHeight);

                if (onPositionChanged != null)
                    onPositionChanged(_position);
            }
        }

        public int MinimumWidth { get; set; }
        public int MinimumHeight { get; set; }
        public Boolean IsMouseOver { get; set; }
        public Boolean IsMouseClick { get; set; }

        public MouseListener()
        {
            MinimumHeight = 16;
            MinimumWidth = 16;
        }

        public virtual void Update(GameTime dt)
        {

        }

        public virtual bool HandleInput(InputState input, Rectangle parent)
        {
            MouseState currentMouse = input.CurrentMouseState;
            MouseState prevMouse = input.LastMouseState;

            Rectangle absPosition = new Rectangle(Position.X + parent.X, Position.Y + parent.Y, Position.Width, Position.Height);

            // Check to see if the mouse is over
            if (!IsMouseOver)
            {
                if (currentMouse.X >= absPosition.X && currentMouse.X <= absPosition.X + absPosition.Width && currentMouse.Y >= absPosition.Y && currentMouse.Y <= absPosition.Y + absPosition.Height)
                {
                    IsMouseOver = true;

                    if (onMouseOver != null)
                        onMouseOver(this);
                }
            }
            // Check to see if the mouse is out
            else
            {
                if (currentMouse.X < absPosition.X || currentMouse.X > absPosition.X + absPosition.Width || currentMouse.Y < absPosition.Y || currentMouse.Y > absPosition.Y + absPosition.Height)
                {
                    IsMouseOver = false;

                    if (onMouseOut != null)
                        onMouseOut(currentMouse);
                }
            }

            // Check for new mouse clicks
            if (IsMouseOver)
            {
                if (currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    IsMouseClick = true;

                    if (onMouseClick != null)
                        onMouseClick(currentMouse);
                }
            }

            // Check for dragging && releasing the mouse
            if (IsMouseClick)
            {
                if (currentMouse.X != prevMouse.X || currentMouse.Y != prevMouse.Y)
                {
                    if (onDrag != null)
                        onDrag(currentMouse);
                }

                if (currentMouse.LeftButton != ButtonState.Pressed)
                {
                    IsMouseClick = false;

                    if (onDragEnd != null)
                        onDragEnd(currentMouse);
                }
            }

            return IsMouseOver;
        }
    }
}
