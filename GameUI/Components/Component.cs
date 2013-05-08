using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameUI.Components
{
    public class Component : MouseListener
    {
        #region Properties

        public bool Visible { get; set; }

        public string Name { get; set; }

        public Rectangle ContentPane { get; private set; }

        public Color Color { get; set; }

        public SpriteFont Font { get; set; }

        public Texture2D Texture { get; set; }

        public int Layer { get; set; }

        public int PaddingTop { get; set; }
        public int PaddingLeft { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingBottom { get; set; }

        #endregion

        public Component(Rectangle position)
        {
            Visible = true;
            Position = position;
            Color = Color.White;

            PaddingTop = PaddingBottom = PaddingLeft = PaddingRight = 4;

            ResetContentPane();

            base.onPositionChanged += new OnPositionChangedEventHandler(Component_onPositionChanged);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            if (!Visible)
                return;
        }

        /// <summary>
        /// Set the padding for all four sides of the component
        /// </summary>
        /// <param name="padding">Number of pixels to pad each side of the content pane from the component border.</param>
        public void SetAllPadding(int padding)
        {
            PaddingTop = PaddingBottom = PaddingLeft = PaddingRight = padding;

            ResetContentPane();
        }

        private void ResetContentPane()
        {
            ContentPane = new Rectangle(
                                Position.X + PaddingLeft,
                                Position.Top + PaddingTop,
                                Position.Width - (PaddingRight + PaddingLeft),
                                Position.Height - (PaddingBottom + PaddingTop)
                            );
        }

        private void Component_onPositionChanged(Rectangle position)
        {
            ResetContentPane();
        }

    }
}
