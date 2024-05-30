using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shader_test
{
    public class BloomShader : OurShader
    {
        private Effect _bloomShader;
        private float _threshold;
        private float _movementMult;

        public BloomShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _bloomShader = content.Load<Effect>("Shaders/Bloom");
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

                _threshold = relativeMousePos.X;
                _movementMult = relativeMousePos.Y * 1.3f;
            }
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing texture to a render target
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

            // extracting brightness
            graphicsDevice.SetRenderTarget(Game1.TARGET_2);
            graphicsDevice.Clear(Color.Transparent);

            _bloomShader.Parameters["brightThreshold"].SetValue(_threshold);
            _bloomShader.CurrentTechnique = _bloomShader.Techniques["ExtractBright"];
            spriteBatch.Begin(effect: _bloomShader, blendState: BlendState.Opaque);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // blurring
            _bloomShader.Parameters["texWidth"].SetValue(Game1.TARGET_1.Width);
            _bloomShader.Parameters["texHeight"].SetValue(Game1.TARGET_1.Height);
            _bloomShader.Parameters["movementMult"].SetValue(_movementMult);

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

            _threshold = 0.75f;
            _movementMult = 0.5f;
        }
    }
}