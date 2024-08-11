using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class FlagShader : OurShader
    {
        public static float WAVE_PROPORTION = 0.06f;
        public static int WAVE_NUM_SEGS = 15;

        public static float WAVE_END_TIME_MULT = 1.2f;
        public static float WAVE_INNER_TIME_MULT = 1.5f;

        public static float FLAG_HEIGHT = 100.0f;

        private Effect _flagShader;

        public FlagShader() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _flagShader = content.Load<Effect>("Shaders/FlagShader");
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Matrix view = Matrix.Identity;

            int width = graphicsDevice.Viewport.Width;
            int height = graphicsDevice.Viewport.Height;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

            Matrix mvpMatrix = view * projection;

            _flagShader.Parameters["mvpMatrix"]?.SetValue(mvpMatrix);

            for (int i = 0; i < WAVE_NUM_SEGS; i++) {
                int leftUPixel = (int)((float)_texture.Width * (float)i / (float)WAVE_NUM_SEGS);
                int rightUPixel = (int)((float)_texture.Width * (float)(i + 1) / (float)WAVE_NUM_SEGS);
                float leftU = (float)leftUPixel / (float)_texture.Width;
                float rightU = (float)rightUPixel / (float)_texture.Width;

                Vector2 pos = Vector2.Floor(GetFlagPos(
                    new Vector2(200.0f, 200.0f),
                    new Vector2(200.0f, 50.0f),
                    leftU
                ));

                Vector2 nextPos = Vector2.Floor(GetFlagPos(
                    new Vector2(200.0f, 200.0f),
                    new Vector2(200.0f, 50.0f),
                    rightU
                ));

                _flagShader.Parameters["rightSideOffset"]?.SetValue(nextPos.Y - pos.Y);
                _flagShader.Parameters["leftSideU"]?.SetValue(leftU);
                _flagShader.Parameters["rightSideU"]?.SetValue(rightU);

                float fullWidth = (nextPos.X - pos.X) * (float)WAVE_NUM_SEGS;
                Vector2 scale = new Vector2(
                    fullWidth / _texture.Width,
                    FLAG_HEIGHT / _texture.Height
                );

                Rectangle sourceRect = new Rectangle(
                    leftUPixel, 0,
                    (rightUPixel - leftUPixel), _texture.Height
                );

                spriteBatch.Begin(effect: _flagShader);
                spriteBatch.Draw(
                    _texture,
                    pos,
                    sourceRect,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.0f
                );

                spriteBatch.End();
            }
        }

        private Vector2 GetFlagPos(Vector2 anchorPos, Vector2 lengthVec, float proportion)
        {
            // finding the size of flag waves
            float waveLen = lengthVec.Length() * WAVE_PROPORTION;

            // making end point wave around a bit
            Vector2 endPos = anchorPos + lengthVec;
            endPos += new Vector2(
                (float)Math.Cos(WAVE_END_TIME_MULT * _totalTime),
                (float)Math.Sin(WAVE_END_TIME_MULT * _totalTime)
            ) * waveLen;

            // making vertices wave around a bit
            Vector2 orthogonalVec = new Vector2(-lengthVec.Y, lengthVec.X);
            orthogonalVec.Normalize();
            orthogonalVec *= waveLen;
            Vector2 waveOffset = (float)Math.Sin(proportion * 5.0f + _totalTime * WAVE_INNER_TIME_MULT) * orthogonalVec;

            // finding final position
            return (anchorPos * (1.0f - proportion))
                + (endPos * proportion)
                + waveOffset;
        }
    }
}