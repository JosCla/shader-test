using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class SimpleBlurShader : OurShader
    {
        private Effect _simpleBlurShader;

        public SimpleBlurShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            this._simpleBlurShader = content.Load<Effect>("Shaders/SimpleBlur");
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(
                (float)Game1.RENDER_SCREEN_SIZE.X / (float)_texture.Width,
                (float)Game1.RENDER_SCREEN_SIZE.Y / (float)_texture.Height
            );

            // drawing background to a texture
            graphicsDevice.SetRenderTarget(Game1.TARGET_1);

            spriteBatch.Begin();
            spriteBatch.Draw(_texture, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            // drawing it again but blurry
            graphicsDevice.SetRenderTarget(Game1.TARGET_2);

            spriteBatch.Begin(effect: _simpleBlurShader, samplerState: SamplerState.LinearClamp);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing the result to the screen
            graphicsDevice.SetRenderTarget(null);

            Vector2 screenScale = new Vector2(
                (float)Game1.SCREEN_RECT.Width / (float)Game1.TARGET_3.Width,
                (float)Game1.SCREEN_RECT.Height / (float)Game1.TARGET_3.Height
            );
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(Game1.TARGET_2, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, screenScale, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}