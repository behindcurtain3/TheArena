using System;
using System.Text;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Drawing.Text;

namespace TheArena.GameObjects.Misc
{
    public class StatusText : Entity
    {

        public string Text { get; private set;}

        public Color TextColor { get; private set; }

        private Vector2 _offset = Vector2.Zero;
        private float _targetY;
        private Random _randomGenerator;
        private Direction _direction;

        public StatusText(string text, Color color, Vector2 position, Direction direction = Direction.Up)
        {
            Construct(text, color, position, direction);
        }

        void Construct(string text, Color color, Vector2 position, Direction direction)
        {
            this.Text = text;
            this.TextColor = color;
            this.Pos = position;
            this._direction = direction;
            this._randomGenerator = new Random();

            _targetY = _randomGenerator.Next(15, 30);
        }

        public override void LoadContent(ContentManager content)
        {
            PlainText plainText = new PlainText(content.Load<SpriteFont>("Fonts/DefaultBold"), Text);
            plainText.Origin = new Vector2(0.5f, 1.0f);

            Drawables.Add("standard", plainText).Color = TextColor;
            CurrentDrawableState = "standard";
            
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            this.Drawables.SetStateProperty("standard", "Offset", this._offset);

            if (_direction == Direction.Up)
            {
                this._offset.Y -= (_targetY - Math.Abs(_offset.Y)) * 0.05f;

                if (Math.Abs(this._offset.Y) >= _targetY - 3)
                    engine.RemoveEntity(this);
            }
            else if (_direction == Direction.Down)
            {
                this._offset.Y += (_targetY - Math.Abs(_offset.Y)) * 0.05f;

                if (Math.Abs(this._offset.Y) >= _targetY - 3)
                    engine.RemoveEntity(this);
            }

            base.Update(gameTime, engine);
        }

    }
}
