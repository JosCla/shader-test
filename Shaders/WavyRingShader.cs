using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class WavyRingShader : OurShader
    {
        private Effect _wavyRingShader;

        private float _scale;

        private float _sharpness;
        private float _magnitude;

        public WavyRingShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            _wavyRingShader = content.Load<Effect>("Shaders/WavyRing");

            base.LoadContent(content);
        }

        public override string GetTexturePath()
        {
            return "tbound-screenshot-5";
        }

        public override void Update(float timeElapsed)
        {
            if (InputUtils.IsMouseHeld()) {
                Vector2 relativeMousePos = InputUtils.GetBoundedMousePos();

                _sharpness = relativeMousePos.X * 10.0f;
                _magnitude = relativeMousePos.Y * 0.5f;
            }

            base.Update(timeElapsed);
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 actualTexSize = _texture.Bounds.Size.ToVector2() * _scale;
            Vector2 pos = (Game1.SCREEN_RECT.Size.ToVector2() * 0.5f) - (actualTexSize * 0.5f);
            pos.Floor();

            graphicsDevice.Clear(Color.Black);

            _wavyRingShader.Parameters["perlinSharpness"]?.SetValue(_sharpness);
            _wavyRingShader.Parameters["perlinMagnitude"]?.SetValue(_magnitude);
            _wavyRingShader.Parameters["time"]?.SetValue(_totalTime);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _wavyRingShader);
            spriteBatch.Draw(_texture, pos, null, Color.White, 0.0f, Vector2.Zero, new Vector2(_scale), SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }

        public override void Reset()
        {
            base.Reset();

            _scale = 2.0f;
            _sharpness = 2.3f;
            _magnitude = 0.15f;
        }
    }
}