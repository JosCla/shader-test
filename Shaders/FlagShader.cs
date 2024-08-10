using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class FlagShader : OurShader
    {
        public static float WAVE_PROPORTION = 0.06f;
        public static int WAVE_NUM_SEGS = 15;

        public static float WAVE_END_TIME_MULT = 1.2f;
        public static float WAVE_INNER_TIME_MULT = 1.5f;

        public FlagShader() {}

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int i = 0; i <= WAVE_NUM_SEGS; i++) {
                Vector2 pos = GetFlagPos(
                    new Vector2(200.0f, 200.0f),
                    new Vector2(200.0f, 50.0f),
                    (float)i / (float)WAVE_NUM_SEGS
                );

                spriteBatch.Draw(
                    _texture,
                    pos,
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    new Vector2(0.1f, 0.1f),
                    SpriteEffects.None,
                    0.0f
                );

                spriteBatch.Draw(
                    _texture,
                    pos + new Vector2(0.0f, 100.0f),
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    new Vector2(0.1f, 0.1f),
                    SpriteEffects.None,
                    0.0f
                );
            }
            spriteBatch.End();
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

            // making vertex wave around a bit
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