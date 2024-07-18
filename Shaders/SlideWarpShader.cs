using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class SlideWarpShader : OurShader
    {
        private Effect _slideWarpShader;
        private Texture2D _secondTex;

        private float _blendFactor;
        private float _intensity;

        public SlideWarpShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            this._slideWarpShader = content.Load<Effect>("Shaders/SlideWarp");
            this._secondTex = content.Load<Texture2D>("tbound-screenshot-4");
        }

        public override string GetTexturePath()
        {
            return "tbound-screenshot-3";
        }

        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);

            if (InputUtils.IsMouseHeld()) {
                Vector2 relativeMousePos = InputUtils.GetBoundedMousePos();

                _blendFactor = relativeMousePos.X;
                // _intensity = relativeMousePos.Y * 200.0f;
                _intensity = (float)Math.Pow(10.0f, relativeMousePos.Y * 3.0f) - 1.0f;
                // _intensity = (float)Math.Exp(27.0f * _blendFactor * (1 - _blendFactor)) - 1.0f;
            }
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing two textures to render targets
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_2, _secondTex);

            // blending them with a cool shader effect
            graphicsDevice.SetRenderTarget(Game1.TARGET_3);

            _slideWarpShader.Parameters["SecondTexture"]?.SetValue(Game1.TARGET_2);
            _slideWarpShader.Parameters["blendFactor"]?.SetValue(_blendFactor);
            _slideWarpShader.Parameters["intensity"]?.SetValue(_intensity);

            spriteBatch.Begin(effect: _slideWarpShader, samplerState: SamplerState.PointWrap);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing the result to the screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_3);
        }

        public override void Reset()
        {
            base.Reset();

            _blendFactor = 0.0f;
            _intensity = 0.0f;
        }
    }
}