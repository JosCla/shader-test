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
            // drawing background to a texture
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

            // drawing it again but blurry
            graphicsDevice.SetRenderTarget(Game1.TARGET_2);

            spriteBatch.Begin(effect: _simpleBlurShader, samplerState: SamplerState.LinearClamp);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing the result to the screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_2);
        }
    }
}