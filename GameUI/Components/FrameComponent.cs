using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameUI.Components
{
    public struct WindowFrame
    {
        public readonly int Top;
        public readonly int Bottom;
        public readonly int Left;
        public readonly int Right;

        public WindowFrame(int t, int b, int l, int r)
        {
            Top = t;
            Bottom = b;
            Left = l;
            Right = r;
        }
    }

    public class FrameComponent : Component
    {
        public WindowFrame Frame { get; set; }

        public FrameComponent(Rectangle position)
            : base(position)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent)
        {
            if (!Visible)
                return;

            for (int i = 0; i <= 8; i++)
                spriteBatch.Draw(Texture, GetTargetRectangle(i), GetSourceRectangle(i), Color);

        }

        public Rectangle GetSourceRectangle(int index)
        {
            switch (index)
            {
                case 0:
                    return new Rectangle(0, 0, Frame.Left, Frame.Top);
                case 1:
                    return new Rectangle(Frame.Left, 0, Texture.Width - (Frame.Left + Frame.Right), Frame.Top);
                case 2:
                    return new Rectangle(Texture.Width - Frame.Right, 0, Frame.Right, Frame.Top);
                case 3:
                    return new Rectangle(0, Frame.Top, Frame.Left, Texture.Height - (Frame.Top + Frame.Bottom));
                case 4:
                    return new Rectangle(Frame.Left, Frame.Top, Texture.Width - (Frame.Left + Frame.Right), Texture.Height - (Frame.Top + Frame.Bottom));
                case 5:
                    return new Rectangle(Texture.Width - Frame.Right, Frame.Top, Frame.Right, Texture.Height - (Frame.Top + Frame.Bottom));
                case 6:
                    return new Rectangle(0, Texture.Height - Frame.Bottom, Frame.Left, Frame.Bottom);
                case 7:
                    return new Rectangle(Frame.Left, Texture.Height - Frame.Bottom, Texture.Width - (Frame.Left + Frame.Right), Frame.Bottom);
                case 8:
                    return new Rectangle(Texture.Width - Frame.Right, Texture.Height - Frame.Bottom, Frame.Right, Frame.Bottom);
            }

            return Position;
        }

        public Rectangle GetTargetRectangle(int index)
        {
            switch (index)
            {
                case 0:
                    return new Rectangle(Position.X, Position.Y, Frame.Left, Frame.Top);
                case 1:
                    return new Rectangle(Position.X + Frame.Left, Position.Y, Position.Width - (Frame.Left + Frame.Right), Frame.Top);
                case 2:
                    return new Rectangle(Position.X + Position.Width - Frame.Right, Position.Y, Frame.Right, Frame.Top);
                case 3:
                    return new Rectangle(Position.X, Position.Y + Frame.Top, Frame.Left, Position.Height - (Frame.Top + Frame.Bottom));
                case 4:
                    return new Rectangle(Position.X + Frame.Left, Position.Y + Frame.Top, Position.Width - (Frame.Left + Frame.Right), Position.Height - (Frame.Top + Frame.Bottom));
                case 5:
                    return new Rectangle(Position.X + Position.Width - Frame.Right, Position.Y + Frame.Top, Frame.Right, Position.Height - (Frame.Top + Frame.Bottom));
                case 6:
                    return new Rectangle(Position.X, Position.Y + Position.Height - Frame.Bottom, Frame.Left, Frame.Bottom);
                case 7:
                    return new Rectangle(Position.X + Frame.Left, Position.Y + Position.Height - Frame.Bottom, Position.Width - (Frame.Left + Frame.Right), Frame.Bottom);
                case 8:
                    return new Rectangle(Position.X + Position.Width - Frame.Right, Position.Y + Position.Height - Frame.Bottom, Frame.Right, Frame.Bottom);
            }

            return Position;
        }
    }
}
