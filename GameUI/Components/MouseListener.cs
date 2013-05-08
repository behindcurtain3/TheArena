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
        public delegate void OnMouseOverEventHandler(object sender, EventArgs e);
        public delegate void OnMouseOutEventHandler(object sender, EventArgs e);
        public delegate void OnMouseClickEventHandler(object sender, EventArgs e);
        public delegate void OnDragEventHandler(object sender, EventArgs e);
        public delegate void OnDragEndEventHandler(object sender, EventArgs e);
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

        public virtual bool HandleInput(InputState input)
        {
            MouseState currentMouse = input.CurrentMouseState;
            MouseState prevMouse = input.LastMouseState;

            // Check to see if the mouse is over
            if (!IsMouseOver)
            {
                if (currentMouse.X >= Position.X && currentMouse.X <= Position.X + Position.Width && currentMouse.Y >= Position.Y && currentMouse.Y <= Position.Y + Position.Height)
                {
                    IsMouseOver = true;

                    if (onMouseOver != null)
                        onMouseOver(this, null);
                }
            }
            // Check to see if the mouse is out
            else
            {
                if (currentMouse.X < Position.X || currentMouse.X > Position.X + Position.Width || currentMouse.Y < Position.Y || currentMouse.Y > Position.Y + Position.Height)
                {
                    IsMouseOver = false;

                    if (onMouseOut != null)
                        onMouseOut(currentMouse, null);
                }
            }

            // Check for new mouse clicks
            if (IsMouseOver)
            {
                if (currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    IsMouseClick = true;

                    if (onMouseClick != null)
                        onMouseClick(currentMouse, null);
                }
            }

            // Check for dragging && releasing the mouse
            if (IsMouseClick)
            {
                if (currentMouse.X != prevMouse.X || currentMouse.Y != prevMouse.Y)
                {
                    if (onDrag != null)
                        onDrag(currentMouse, null);
                }

                if (currentMouse.LeftButton != ButtonState.Pressed)
                {
                    IsMouseClick = false;

                    if (onDragEnd != null)
                        onDragEnd(currentMouse, null);
                }
            }

            return IsMouseOver;
        }
    }
}
