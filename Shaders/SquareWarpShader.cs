using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class SquareWarpShader : OurShader
    {
        private Effect _squareWarpShader;
        private Texture2D _secondTex;

        public static readonly int MAX_DROPS = 20;
        public static readonly float POWER_CHANGE_VEL = 1.0f;
        public static readonly float MIN_POWER = 0.1f;

        private int _currDrop;
        private float _currOffset;
        private int _totalDrops;
        private Vector4[] _drops;
        private float _distPower;

        public SquareWarpShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            this._squareWarpShader = content.Load<Effect>("Shaders/SquareWarp");
            this._secondTex = content.Load<Texture2D>("tbound-screenshot-4");
        }

        public override string GetTexturePath()
        {
            return "tbound-screenshot-3";
        }

        public override void Update(float timeElapsed)
        {
            if (InputUtils.IsMouseOnPress()) {
                Vector2 relativeDropPoint = InputUtils.GetMousePos();

                AddDrop(
                    new Vector2(
                        relativeDropPoint.X * (float)Game1.RENDER_SCREEN_SIZE.X,
                        relativeDropPoint.Y * (float)Game1.RENDER_SCREEN_SIZE.Y
                    ),
                    _totalTime
                );
            }

            if (InputUtils.IsOneHeld()) {
                _distPower -= POWER_CHANGE_VEL * timeElapsed;
                // if (_distPower < MIN_POWER) {_distPower = MIN_POWER;}
            } else if (InputUtils.IsTwoHeld()) {
                _distPower += POWER_CHANGE_VEL * timeElapsed;
            }

            base.Update(timeElapsed);
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing two textures to render targets
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_2, _secondTex);

            // blending them with a cool shader effect
            graphicsDevice.SetRenderTarget(Game1.TARGET_3);

            _squareWarpShader.Parameters["SecondTexture"]?.SetValue(Game1.TARGET_2);
            _squareWarpShader.Parameters["time"].SetValue(_totalTime);
            _squareWarpShader.Parameters["numDrops"].SetValue(_totalDrops);
            _squareWarpShader.Parameters["drops"].SetValue(_drops);
            _squareWarpShader.Parameters["distPower"].SetValue(_distPower);

            spriteBatch.Begin(effect: _squareWarpShader, samplerState: SamplerState.PointWrap);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing the result to the screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_3);
        }

        private void AddDrop(Vector2 pos, float time)
        {
            if (_totalDrops == MAX_DROPS) {
                _totalDrops -= 2;
                _currDrop -= 2;

                for (int i = 0; i < (MAX_DROPS - 2); i++) {
                    _drops[i] = _drops[i + 2];
                }
            }

            _drops[_currDrop] = new Vector4(pos.X, pos.Y, time, _currOffset);
            _currDrop++;
            _currOffset *= -1.0f;
            _totalDrops++;
        }

        public override void Reset()
        {
            base.Reset();

            _drops = new Vector4[20];
            _currDrop = 0;
            _currOffset = 1.0f;
            _totalDrops = 0;
            _distPower = 2.0f;
        }
    }
}