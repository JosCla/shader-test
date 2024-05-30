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
            // drawing background to a texture
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

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
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_2);
        }
    }
}