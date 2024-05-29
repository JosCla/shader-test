using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace shader_test
{
    public class OurShader
    {
        protected Texture2D _texture;
        protected float _totalTime;

        public OurShader() {}

        public virtual void Initialize()
        {
            Reset();
        }

        public virtual void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>(GetTexturePath());
        }

        public virtual void Update(float timeElapsed)
        {
            _totalTime += timeElapsed;
        }

        public virtual void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_texture, Vector2.Zero, null, Color.White);
            spriteBatch.End();
        }

        public virtual string GetTexturePath()
        {
            return "tbound-screenshot-3";
        }

        public virtual void Reset()
        {
            _totalTime = 0.0f;
        }
    }
}