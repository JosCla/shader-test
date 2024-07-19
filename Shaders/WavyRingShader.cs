using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class WavyRingShader : OurShader
    {
        public static int GRID_WIDTH = 5;
        public static int GRID_HEIGHT = 5;

        private Effect _wavyRingShader;

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

                // System.Console.WriteLine("sharp: " + _sharpness + ", mag: " + _magnitude);
            }

            base.Update(timeElapsed);
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // calculating some draw constants for wavy ring positioning
            Vector2 cellSize = Game1.SCREEN_RECT.Size.ToVector2() / new Vector2(GRID_WIDTH, GRID_HEIGHT);
            Vector2 texSize = _texture.Bounds.Size.ToVector2();
            Vector2 scaleVec = (cellSize * 0.9f) / texSize;
            float actualScale = Math.Min(scaleVec.X, scaleVec.Y);
            Vector2 relativePos = (cellSize * 0.5f) - (texSize * actualScale * 0.5f);

            int maxCellInd = GRID_WIDTH * GRID_HEIGHT - 1;

            // drawing all the wavy rings!
            graphicsDevice.Clear(Color.Black);

            _wavyRingShader.Parameters["perlinSharpness"]?.SetValue(_sharpness);
            _wavyRingShader.Parameters["perlinMagnitude"]?.SetValue(_magnitude);
            _wavyRingShader.Parameters["time"]?.SetValue(_totalTime);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _wavyRingShader);
            for (int row = 0; row < GRID_HEIGHT; row++) {
                for (int col = 0; col < GRID_WIDTH; col++) {
                    Vector2 currCell = cellSize * new Vector2(col, row);
                    Vector2 pos = currCell + relativePos;
                    pos.Floor();

                    int cellInd = row * GRID_WIDTH + col;
                    Color cellColor = new Color(1.0f, 1.0f, 1.0f, (float)cellInd / (float)maxCellInd);

                    spriteBatch.Draw(_texture, pos, null, cellColor, 0.0f, Vector2.Zero, new Vector2(actualScale), SpriteEffects.None, 0.0f);
                }
            }
            spriteBatch.End();
        }

        public override void Reset()
        {
            base.Reset();

            _sharpness = 3.71f;
            _magnitude = 0.147f;
        }
    }
}