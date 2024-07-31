using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class VertexWaveShader : OurShader
    {
        private Effect _vertexWaveEffect;
        private float _intensity;
        private int _numSegs;
        
        public VertexWaveShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _vertexWaveEffect = content.Load<Effect>("Shaders/VertexWave");
        }

        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);

            if (InputUtils.IsMouseHeld())
            {
                _intensity = InputUtils.GetBoundedMousePos().X * 0.3f;
                _numSegs = (int)Math.Floor(InputUtils.GetBoundedMousePos().Y * 10.0f) + 1;
            }
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Matrix view = Matrix.Identity;

            int width = graphicsDevice.Viewport.Width;
            int height = graphicsDevice.Viewport.Height;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

            Matrix mvpMatrix = view * projection;

            _vertexWaveEffect.Parameters["mvpMatrix"]?.SetValue(mvpMatrix);
            _vertexWaveEffect.Parameters["time"]?.SetValue(_totalTime);
            _vertexWaveEffect.Parameters["intensity"].SetValue(_intensity);
            spriteBatch.Begin(effect: _vertexWaveEffect);

            float relWidth = 1.0f / (float)_numSegs;
            for (int i = 0; i < _numSegs; i++)
            {
                Point currPos = new Point((int)(relWidth * i * _texture.Width), 0);
                Point nextPos = new Point((int)(relWidth * (i + 1) * _texture.Width), 0);

                Rectangle sourceRect = new Rectangle(currPos, new Point(nextPos.X - currPos.X, _texture.Height));

                spriteBatch.Draw(_texture, currPos.ToVector2(), sourceRect, Color.White);

                // below has texture tearing
                /*
                Vector2 pos = new Vector2(
                    relWidth * i * _texture.Width,
                    0.0f
                ).ToPoint().ToVector2();
                Rectangle sourceRect = new Rectangle(
                    pos.ToPoint(),
                    new Vector2(relWidth * _texture.Width, _texture.Height).ToPoint()
                );

                spriteBatch.Draw(_texture, pos, sourceRect, Color.White);
                */
            }

            spriteBatch.End();
        }

        public override void Reset()
        {
            base.Reset();

            _intensity = 0.1f;
            _numSegs = 3;
        }
    }
}