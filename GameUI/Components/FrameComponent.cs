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

        public Rectangle SourceOffset { get; set; }

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

            SourceOffset = Rectangle.Empty;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle parent, GameTime gameTime)
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

            base.Draw(spriteBatch, parent, gameTime);
        }

        public Rectangle GetSourceRectangle(int index)
        {
            if (SourceOffset == Rectangle.Empty)
                SourceOffset = new Rectangle(0, 0, Texture.Width, Texture.Height);

            switch (index)
            {
                case 0:
                    return new Rectangle(
                                        SourceOffset.X, 
                                        SourceOffset.Y, 
                                        FrameLeft, 
                                        FrameTop
                                    );
                case 1:
                    return new Rectangle(
                                        SourceOffset.X + FrameLeft, 
                                        SourceOffset.Y, 
                                        SourceOffset.Width - (FrameLeft + FrameRight), 
                                        FrameTop
                                    );
                case 2:
                    return new Rectangle(
                                        SourceOffset.X + SourceOffset.Width - FrameRight, 
                                        SourceOffset.Y, 
                                        FrameRight, 
                                        FrameTop
                                    );
                case 3:
                    return new Rectangle(
                                        SourceOffset.X,
                                        SourceOffset.Y + FrameTop, 
                                        FrameLeft, 
                                        Texture.Height - (FrameTop + FrameBottom)
                                    );
                case 4:
                    return new Rectangle(
                                        SourceOffset.X + FrameLeft,
                                        SourceOffset.Y + FrameTop, 
                                        SourceOffset.Width - (FrameLeft + FrameRight), 
                                        Texture.Height - (FrameTop + FrameBottom)
                                    );
                case 5:
                    return new Rectangle(
                                        SourceOffset.X + SourceOffset.Width - FrameRight,
                                        SourceOffset.Y + FrameTop, 
                                        FrameRight, 
                                        Texture.Height - (FrameTop + FrameBottom)
                                    );
                case 6:
                    return new Rectangle(
                                        SourceOffset.X,
                                        SourceOffset.Y + Texture.Height - FrameBottom, 
                                        FrameLeft, 
                                        FrameBottom
                                    );
                case 7:
                    return new Rectangle(
                                        SourceOffset.X + FrameLeft,
                                        SourceOffset.Y + Texture.Height - FrameBottom, 
                                        SourceOffset.Width - (FrameLeft + FrameRight), 
                                        FrameBottom
                                    );
                case 8:
                    return new Rectangle(
                                        SourceOffset.X + SourceOffset.Width - FrameRight, 
                                        SourceOffset.Y + Texture.Height - FrameBottom, 
                                        FrameRight, 
                                        FrameBottom
                                    );
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

        protected override void ResetContentPane()
        {
            ContentPane = new Rectangle(
                                Position.X + PaddingLeft + FrameLeft,
                                Position.Y + PaddingTop + FrameTop,
                                Position.Width - (PaddingRight + FrameRight + PaddingLeft + FrameLeft),
                                Position.Height - (PaddingBottom + FrameBottom + PaddingTop + FrameTop)
                            );
        }
    }
}
