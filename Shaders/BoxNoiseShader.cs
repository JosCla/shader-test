using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class BoxNoiseShader : OurShader
    {
        private Effect _boxNoiseShader;
        private Random _random;

        private float _movementMult;
        private float _gridWidth;

        public BoxNoiseShader() : base()
        {
            this._random = new Random();

            this._movementMult = 5.0f;
            this._gridWidth = 10.0f;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _boxNoiseShader = content.Load<Effect>("Shaders/BoxNoise");
            _boxNoiseShader.Parameters["movementMult"]?.SetValue(_movementMult);
            _boxNoiseShader.Parameters["gridWidth"]?.SetValue(_gridWidth);
        }

        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);

            if (InputUtils.IsMouseHeld()) {
                Vector2 relativeMousePos = InputUtils.GetMousePos();

                _movementMult = relativeMousePos.X * 10.0f;
                _gridWidth = relativeMousePos.Y * 20.0f;

                _boxNoiseShader.Parameters["movementMult"]?.SetValue(_movementMult);
                _boxNoiseShader.Parameters["gridWidth"]?.SetValue(_gridWidth);
            }
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing background to a texture
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

            // using boxNoise noise to wacky-ify that texture
            graphicsDevice.SetRenderTarget(Game1.TARGET_2);
            _boxNoiseShader.Parameters["texWidth"]?.SetValue(Game1.TARGET_1.Width);
            _boxNoiseShader.Parameters["texHeight"]?.SetValue(Game1.TARGET_1.Height);
            _boxNoiseShader.Parameters["time"]?.SetValue(_totalTime);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _boxNoiseShader);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing target to the screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_2);
        }
    }
}