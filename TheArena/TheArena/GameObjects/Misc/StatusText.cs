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


        public StatusText(string text, Color color, Vector2 position)
        {
            Construct(text, color, position);
        }

        void Construct(string text, Color color, Vector2 position)
        {
            this.Text = text;
            this.TextColor = color;
            this.Pos = position;
            this._randomGenerator = new Random();

            _targetY = _randomGenerator.Next(25, 40);
        }

        public override void LoadContent(ContentManager content)
        {
            PlainText plainText = new PlainText(content.Load<SpriteFont>("Fonts/Default"), Text);
            plainText.Origin = new Vector2(0.5f, 1.0f);

            Drawables.Add("standard", plainText).Color = TextColor;
            CurrentDrawableState = "standard";
        }

        public override void Update(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            //this.Opacity -= 0.02f;
            this.Drawables.SetStateProperty("standard", "Offset", this._offset);
            this._offset.Y -= (_targetY - Math.Abs(_offset.Y)) * 0.05f;

            if (Math.Abs(this._offset.Y) >= _targetY - 5)
                engine.RemoveEntity(this);

            base.Update(gameTime, engine);
        }

    }
}
