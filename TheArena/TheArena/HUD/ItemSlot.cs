using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameUI.Components;
using TheArena.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Drawing;

namespace TheArena.HUD
{
    public class ItemSlot : Component
    {
        public delegate void OnItemReleasedEventHandler(ItemSlot slot, Vector2 position);
        public event OnItemReleasedEventHandler onItemReleased;

        public Item Item { get; set; }

        private Rectangle _sourceRect;
        private bool _followMouse = false;
        private Vector2 _mousePosition;

        public ItemSlot()
            : base(new Rectangle(0, 0, 32, 32))
        {
            Construct();
        }

        public ItemSlot(Rectangle position)
            : base(position)
        {
            Construct();
        }

        private void Construct()
        {
            // For now all item icons are 32x32, the slots are 34x34 to allow for padding
            _sourceRect = new Rectangle(0, 0, 34, 34);

            Item = null;
            SetAllPadding(1);

            onMouseOver += new OnMouseOverEventHandler(ItemSlot_onMouseOver);
            onMouseOut += new OnMouseOutEventHandler(ItemSlot_onMouseOut);
            onMouseClick += new OnMouseClickEventHandler(ItemSlot_onMouseClick);
            onDrag += new OnDragEventHandler(ItemSlot_onDrag);
            onDragEnd += new OnDragEndEventHandler(ItemSlot_onDragEnd);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            if (!Visible)
                return;

            Rectangle drawAt = new Rectangle(Position.X + parent.X, Position.Y + parent.Y, Position.Width, Position.Height);

            spriteBatch.Draw(Texture, drawAt, _sourceRect, Color);

            if (Item != null)
            {
                if (_followMouse)
                    spriteBatch.Draw(Item.Icon, _mousePosition - new Vector2(16, 16), Color);
                else
                    spriteBatch.Draw(Item.Icon, ContentPane, Color);
            }
        }


        private void ItemSlot_onMouseOver(object sender)
        {
            _sourceRect.X = 34;
        }

        private void ItemSlot_onMouseOut(object sender)
        {
            _sourceRect.X = 0;
        }

        private void ItemSlot_onMouseClick(object sender)
        {
            _followMouse = true;
        }

        private void ItemSlot_onDrag(object sender)
        {
            MouseState mouse = (MouseState)sender;

            _mousePosition = new Vector2(mouse.X, mouse.Y);
        }

        private void ItemSlot_onDragEnd(object sender)
        {
            _followMouse = false;

            // Signal that the item has been released
            if (Item != null)
            {
                if (!IsMouseOver)
                {
                    if (onItemReleased != null)
                        onItemReleased(this, _mousePosition);
                }
            }

            _mousePosition = new Vector2(Position.X + 16, Position.Y + 16);
        }
    }
}
