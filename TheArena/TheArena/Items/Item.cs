﻿using GameEngine.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace TheArena.Items
{
    public enum ItemType { HeadGear, Vest, Gloves, Pants, Boots, Weapon }

    public class Item
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public float Weight { get; set; }

        public ItemType ItemType { get; set; }

        public DrawableSet Drawables { get; set; }

        public Texture2D Icon { get; set; }

        public Item()
        {
            Drawables = new DrawableSet();
        }
    }
}
