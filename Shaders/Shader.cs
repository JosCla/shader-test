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

        protected void DrawTexInTarget(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D target)
        {
            Vector2 scale = new Vector2(
                (float)Game1.RENDER_SCREEN_SIZE.X / (float)_texture.Width,
                (float)Game1.RENDER_SCREEN_SIZE.Y / (float)_texture.Height
            );

            graphicsDevice.SetRenderTarget(target);

            spriteBatch.Begin();
            spriteBatch.Draw(_texture, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }

        protected void DrawTargetToScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D target)
        {
            graphicsDevice.SetRenderTarget(null);

            Vector2 screenScale = new Vector2(
                (float)Game1.SCREEN_RECT.Width / (float)target.Width,
                (float)Game1.SCREEN_RECT.Height / (float)target.Height
            );
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(target, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, screenScale, SpriteEffects.None, 0.0f);
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