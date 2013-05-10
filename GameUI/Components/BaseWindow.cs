using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameUI.Extensions;
using System.Reflection;

namespace GameUI.Components
{
    /// <summary>
    /// The base "window" class that will display a UI window.
    /// This class inherits from Component but also contains a list of components
    /// that belong to it.
    /// </summary>
    public class BaseWindow : FrameComponent
    {
        public BaseWindow() : base(new Rectangle(0, 0, 64, 64))
        {
            Construct();
        }

        public BaseWindow(Rectangle position) : base(position)
        {
            Construct();
        }

        private void Construct()
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            base.Draw(spriteBatch, parent);            
        }
    }
}
