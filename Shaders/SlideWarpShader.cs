using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                Point mousePos = Mouse.GetState().Position;
                Vector2 relativeMousePos = new Vector2(
                    Math.Clamp((float)mousePos.X / (float)Game1.SCREEN_RECT.Width, 0.0f, 1.0f),
                    Math.Clamp((float)mousePos.Y / (float)Game1.SCREEN_RECT.Height, 0.0f, 1.0f)
                );

                _blendFactor = relativeMousePos.X;
                // _intensity = relativeMousePos.Y * 200.0f;
                _intensity = (float)Math.Pow(10.0f, relativeMousePos.Y * 3.0f) - 1.0f;
            }
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing background to a texture
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_2, _secondTex);

            // drawing it again but blurry
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
    }
}