using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class BloomShader : OurShader
    {
        private Effect _bloomShader;

        public BloomShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _bloomShader = content.Load<Effect>("Shaders/Bloom");
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing texture to a render target
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

            // extracting brightness
            graphicsDevice.SetRenderTarget(Game1.TARGET_2);
            graphicsDevice.Clear(Color.Transparent);

            _bloomShader.CurrentTechnique = _bloomShader.Techniques["ExtractBright"];
            spriteBatch.Begin(effect: _bloomShader, blendState: BlendState.Opaque);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // blurring
            _bloomShader.Parameters["texWidth"].SetValue(Game1.TARGET_1.Width);
            _bloomShader.Parameters["texHeight"].SetValue(Game1.TARGET_1.Height);

            graphicsDevice.SetRenderTarget(Game1.TARGET_3);
            _bloomShader.CurrentTechnique = _bloomShader.Techniques["HorizBlur"];
            spriteBatch.Begin(effect: _bloomShader, blendState: BlendState.Opaque);
            spriteBatch.Draw(Game1.TARGET_2, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(Game1.TARGET_2);
            _bloomShader.CurrentTechnique = _bloomShader.Techniques["VertBlur"];
            spriteBatch.Begin(effect: _bloomShader, blendState: BlendState.Opaque);
            spriteBatch.Draw(Game1.TARGET_3, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing result back to screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_1);
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_2, false);
        }
    }
}