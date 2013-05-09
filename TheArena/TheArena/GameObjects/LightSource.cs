using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using TheArena.Shaders;
using GameEngine.Interfaces;

namespace TheArena.GameObjects
{
    public class LightSource : Entity, ISizedEntity
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public LightPositionType PositionType { get; set; }

        public Color Color { get; set; }

        public LightSource()
        {
            Color = Color.White;
            PositionType = LightPositionType.Relative;
        }

        public override void PostCreate(GameTime gameTime, GameEngine.TeeEngine engine)
        {
            // Todo: this should technically NOT be here.
            this.Pos += new Vector2(Width / 2.0f, Height / 2.0f);

            LightShader lightShader = (LightShader)engine.GetPostGameShader("LightShader");
            lightShader.LightSources.Add(this);
        }
    }
}
