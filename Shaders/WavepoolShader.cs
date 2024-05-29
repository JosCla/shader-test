using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class WavepoolShader : OurShader
    {
        public Effect _wavepoolShader;

        public WavepoolShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            _wavepoolShader = content.Load<Effect>("Shaders/Wavepool");

            base.LoadContent(content);
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

            // drawing it again to a small screen
            graphicsDevice.SetRenderTarget(Game1.TARGET_2);

            spriteBatch.Begin();
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            // drawing a watery reflection!
            spriteBatch.Begin(effect: _wavepoolShader, samplerState: SamplerState.PointWrap);
            _wavepoolShader.Parameters["time"].SetValue(_totalTime);
            _wavepoolShader.Parameters["texOffsetMult"].SetValue(0.05f);
            _wavepoolShader.Parameters["period"].SetValue(128.0f);

            spriteBatch.Draw(
                Game1.TARGET_1,
                new Vector2(0.0f, (float)Game1.TARGET_2.Height / 2.0f),
                new Rectangle(0, 0, Game1.TARGET_2.Width, Game1.TARGET_2.Height / 2),
                Color.CornflowerBlue,
                0.0f,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.FlipVertically,
                0.0f
            );

            spriteBatch.End();

            // finally copying that all to the screen
            graphicsDevice.SetRenderTarget(null);

            Vector2 screenScale = new Vector2(
                (float)Game1.SCREEN_RECT.Width / (float)Game1.TARGET_2.Width,
                (float)Game1.SCREEN_RECT.Height / (float)Game1.TARGET_2.Height
            );
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(Game1.TARGET_2, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, screenScale, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}