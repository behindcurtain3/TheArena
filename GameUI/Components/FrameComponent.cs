using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameUI.Components
{
    public class FrameComponent : Component
    {
        public int FrameTop { get; set; }
        public int FrameBottom { get; set; }
        public int FrameLeft { get; set; }
        public int FrameRight { get; set; }

        public FrameComponent()
        {
            Construct();
        }

        public FrameComponent(Rectangle position)
            : base(position)
        {
            Construct();            
        }

        private void Construct()
        {
            FrameTop = 4;
            FrameBottom = 4;
            FrameLeft = 4;
            FrameRight = 4;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            if (!Visible)
                return;

            Rectangle absPosition;
            for (int i = 0; i <= 8; i++)
            {
                Rectangle target = GetTargetRectangle(i);
                absPosition = new Rectangle(parent.X + target.X, parent.Y + target.Y, target.Width, target.Height);
                spriteBatch.Draw(Texture, absPosition, GetSourceRectangle(i), Color);
            }

            base.Draw(spriteBatch, parent);
        }

        public Rectangle GetSourceRectangle(int index)
        {
            switch (index)
            {
                case 0:
                    return new Rectangle(0, 0, FrameLeft, FrameTop);
                case 1:
                    return new Rectangle(FrameLeft, 0, Texture.Width - (FrameLeft + FrameRight), FrameTop);
                case 2:
                    return new Rectangle(Texture.Width - FrameRight, 0, FrameRight, FrameTop);
                case 3:
                    return new Rectangle(0, FrameTop, FrameLeft, Texture.Height - (FrameTop + FrameBottom));
                case 4:
                    return new Rectangle(FrameLeft, FrameTop, Texture.Width - (FrameLeft + FrameRight), Texture.Height - (FrameTop + FrameBottom));
                case 5:
                    return new Rectangle(Texture.Width - FrameRight, FrameTop, FrameRight, Texture.Height - (FrameTop + FrameBottom));
                case 6:
                    return new Rectangle(0, Texture.Height - FrameBottom, FrameLeft, FrameBottom);
                case 7:
                    return new Rectangle(FrameLeft, Texture.Height - FrameBottom, Texture.Width - (FrameLeft + FrameRight), FrameBottom);
                case 8:
                    return new Rectangle(Texture.Width - FrameRight, Texture.Height - FrameBottom, FrameRight, FrameBottom);
            }

            return Position;
        }

        public Rectangle GetTargetRectangle(int index)
        {
            switch (index)
            {
                case 0:
                    return new Rectangle(Position.X, Position.Y, FrameLeft, FrameTop);
                case 1:
                    return new Rectangle(Position.X + FrameLeft, Position.Y, Position.Width - (FrameLeft + FrameRight), FrameTop);
                case 2:
                    return new Rectangle(Position.X + Position.Width - FrameRight, Position.Y, FrameRight, FrameTop);
                case 3:
                    return new Rectangle(Position.X, Position.Y + FrameTop, FrameLeft, Position.Height - (FrameTop + FrameBottom));
                case 4:
                    return new Rectangle(Position.X + FrameLeft, Position.Y + FrameTop, Position.Width - (FrameLeft + FrameRight), Position.Height - (FrameTop + FrameBottom));
                case 5:
                    return new Rectangle(Position.X + Position.Width - FrameRight, Position.Y + FrameTop, FrameRight, Position.Height - (FrameTop + FrameBottom));
                case 6:
                    return new Rectangle(Position.X, Position.Y + Position.Height - FrameBottom, FrameLeft, FrameBottom);
                case 7:
                    return new Rectangle(Position.X + FrameLeft, Position.Y + Position.Height - FrameBottom, Position.Width - (FrameLeft + FrameRight), FrameBottom);
                case 8:
                    return new Rectangle(Position.X + Position.Width - FrameRight, Position.Y + Position.Height - FrameBottom, FrameRight, FrameBottom);
            }

            return Position;
        }
    }
}
