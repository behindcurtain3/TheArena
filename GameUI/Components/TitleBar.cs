using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameUI.Components
{
    public class TitleBar : FrameComponent
    {
        public string Text { get; set; }

        public TitleBar(Rectangle position)
            : base(position)
        {
            SetAllPadding(4);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            base.Draw(spriteBatch, parent);

            spriteBatch.DrawString(Font, Text, new Vector2(ContentPane.X, ContentPane.Y), Color);
        }

    }
}
