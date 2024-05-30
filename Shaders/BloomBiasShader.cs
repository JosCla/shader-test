using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shader_test
{
    public class BloomBiasShader : OurShader
    {
        public static readonly int NUM_BLUR_PASSES = 2;

        private Effect _bloomShader;
        private float _biasX;
        private float _biasY;

        public BloomBiasShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _bloomShader = content.Load<Effect>("Shaders/BloomBias");
        }

        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                Point mousePos = Mouse.GetState().Position;
                Vector2 relativeMousePos = new Vector2(
                    (float)mousePos.X / (float)Game1.SCREEN_RECT.Width,
                    (float)mousePos.Y / (float)Game1.SCREEN_RECT.Height
                );

                _biasX = relativeMousePos.X - 0.5f;
                _biasY = relativeMousePos.Y - 0.5f;
            }
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
            _bloomShader.Parameters["biasX"].SetValue(_biasX);
            _bloomShader.Parameters["biasY"].SetValue(_biasY);

            for (int i = 1; i <= NUM_BLUR_PASSES; i++)
            {
                graphicsDevice.SetRenderTarget(Game1.TARGET_3);
                _bloomShader.CurrentTechnique = _bloomShader.Techniques["HorizBlur"];
                spriteBatch.Begin(effect: _bloomShader, blendState: BlendState.Opaque, samplerState: SamplerState.LinearClamp);
                spriteBatch.Draw(Game1.TARGET_2, Vector2.Zero, null, Color.White);
                spriteBatch.End();

                graphicsDevice.SetRenderTarget(Game1.TARGET_2);
                _bloomShader.CurrentTechnique = _bloomShader.Techniques["VertBlur"];
                spriteBatch.Begin(effect: _bloomShader, blendState: BlendState.Opaque, samplerState: SamplerState.LinearClamp);
                spriteBatch.Draw(Game1.TARGET_3, Vector2.Zero, null, Color.White);
                spriteBatch.End();
            }

            // drawing result back to screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_1);
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_2, false);
        }

        public override string GetTexturePath()
        {
            return "tbound-screenshot-4";
        }

        public override void Reset()
        {
            base.Reset();

            _biasX = 0.0f;
            _biasY = 0.0f;
        }
    }
}