using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameUI.Components
{
    public class Label : Component
    {
        public string Text { get; set; }
        public string Data { get; set; }

        public Label()
        {
            SetAllPadding(0);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            base.Draw(spriteBatch, parent);

            spriteBatch.DrawString(Font, Text, new Vector2(ContentPane.X + parent.X, ContentPane.Y + parent.Y), Color);

            // Draw data, aligned to the right
            if (Data != null)
            {
                Vector2 dataPositon = new Vector2(ContentPane.X + parent.X + ContentPane.Width - Font.MeasureString(Data).X, ContentPane.Y + parent.Y);
                spriteBatch.DrawString(Font, Data, dataPositon, Color);
            }
        }

    }
}
