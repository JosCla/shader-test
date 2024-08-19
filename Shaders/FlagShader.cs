using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class FlagShader : OurShader
    {
        public static readonly float WAVE_PROPORTION = 0.08f;
        public static readonly int WAVE_NUM_SEGS = 15;

        public static readonly float WAVE_END_TIME_MULT = 3.6f;
        public static readonly float WAVE_INNER_TIME_MULT = 4.5f;

        public static readonly float FLAG_HEIGHT = 30.0f;

        public static readonly Vector2 FLAG_ANCHOR_POS = new Vector2(12.0f, 12.0f);

        private Effect _flagShader;
        private Texture2D _flagTex;

        private Vector2 _windDir;

        public FlagShader() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _flagShader = content.Load<Effect>("Shaders/FlagShader");
            _flagTex = content.Load<Texture2D>("sample-banner");
        }

        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);

            if (InputUtils.IsMouseHeld()) {
                Vector2 mouseDir = new Vector2(
                    InputUtils.GetBoundedMousePos().X,
                    InputUtils.GetBoundedMousePos().Y * 2.0f - 1.0f
                );
                mouseDir.Normalize();

                float texAspectRatio = (float)_flagTex.Width / (float)_flagTex.Height;
                _windDir = mouseDir * FLAG_HEIGHT * texAspectRatio;
            }
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing background
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

            // creating a rasterizer state that doesn't do back-face culling
            RasterizerState flagRastState = new RasterizerState();
            flagRastState.CullMode = CullMode.None;

            // drawing flag on top of it
            Matrix view = Matrix.Identity;

            int width = Game1.TARGET_1.Width;
            int height = Game1.TARGET_1.Height;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

            Matrix mvpMatrix = view * projection;

            _flagShader.Parameters["mvpMatrix"]?.SetValue(mvpMatrix);

            for (int i = 0; i < WAVE_NUM_SEGS; i++) {
                int leftUPixel = (int)((float)_flagTex.Width * (float)i / (float)WAVE_NUM_SEGS);
                int rightUPixel = (int)((float)_flagTex.Width * (float)(i + 1) / (float)WAVE_NUM_SEGS);
                float leftU = (float)leftUPixel / (float)_flagTex.Width;
                float rightU = (float)rightUPixel / (float)_flagTex.Width;

                Vector2 pos = Vector2.Floor(GetFlagPos(
                    FLAG_ANCHOR_POS,
                    _windDir,
                    leftU
                ));

                Vector2 nextPos = Vector2.Floor(GetFlagPos(
                    FLAG_ANCHOR_POS,
                    _windDir,
                    rightU
                ));

                _flagShader.Parameters["rightSideOffset"]?.SetValue(nextPos.Y - pos.Y);
                _flagShader.Parameters["leftSideU"]?.SetValue(leftU);
                _flagShader.Parameters["rightSideU"]?.SetValue(rightU);

                Vector2 scale = new Vector2(
                    (float)(nextPos.X - pos.X) / (float)(rightUPixel - leftUPixel),
                    FLAG_HEIGHT / _flagTex.Height
                );

                Rectangle sourceRect = new Rectangle(
                    leftUPixel, 0,
                    (rightUPixel - leftUPixel), _flagTex.Height
                );

                spriteBatch.Begin(effect: _flagShader, samplerState: SamplerState.PointClamp, rasterizerState: flagRastState);
                spriteBatch.Draw(
                    _flagTex,
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

            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_1);
        }

        private Vector2 GetFlagPos(Vector2 anchorPos, Vector2 lengthVec, float proportion)
        {
            // finding the size of flag waves
            float waveLen = lengthVec.Length() * WAVE_PROPORTION;

            // making end point wave around a bit
            Vector2 endPos = anchorPos + lengthVec;
            endPos += new Vector2(
                (float)Math.Cos(WAVE_END_TIME_MULT * _totalTime),
                // (float)Math.Sin(WAVE_END_TIME_MULT * _totalTime)
                0.0f
            ) * waveLen;

            // making vertices wave around a bit
            Vector2 orthogonalVec = new Vector2(-lengthVec.Y, lengthVec.X);
            orthogonalVec.Normalize();
            orthogonalVec *= waveLen;
            orthogonalVec *= (float)Math.Sqrt(proportion);
            Vector2 waveOffset = (float)Math.Sin(proportion * 5.0f + _totalTime * WAVE_INNER_TIME_MULT) * orthogonalVec;

            // finding final position
            return (anchorPos * (1.0f - proportion))
                + (endPos * proportion)
                + waveOffset;
        }

        public override string GetTexturePath()
        {
            return "tbound-screenshot-4";
        }

        public override void Reset()
        {
            base.Reset();

            _windDir = new Vector2(
                FLAG_HEIGHT * 4.685f,
                FLAG_HEIGHT * 1.0f
            );
        }
    }
}