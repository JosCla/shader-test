using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shader_test
{
    public class RingWarpShader : OurShader
    {
        private Effect _ringWarpShader;
        private Texture2D _secondTex;

        public static readonly int MAX_DROPS = 20;

        private int _currDrop;
        private float _currOffset;
        private int _totalDrops;
        private Vector4[] _drops;
        private bool _mouseHeld;

        public RingWarpShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            this._ringWarpShader = content.Load<Effect>("Shaders/RingWarp");
            this._secondTex = content.Load<Texture2D>("tbound-screenshot-4");
        }

        public override string GetTexturePath()
        {
            return "tbound-screenshot-3";
        }

        public override void Update(float timeElapsed)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                if (!_mouseHeld) {
                    Point dropPoint = Mouse.GetState().Position;
                    Vector2 relativeDropPoint = new Vector2(
                        Math.Clamp((float)dropPoint.X / (float)Game1.SCREEN_RECT.Width, 0.0f, 1.0f),
                        Math.Clamp((float)dropPoint.Y / (float)Game1.SCREEN_RECT.Height, 0.0f, 1.0f)
                    );
                    AddDrop(
                        new Vector2(
                            relativeDropPoint.X * (float)Game1.RENDER_SCREEN_SIZE.X,
                            relativeDropPoint.Y * (float)Game1.RENDER_SCREEN_SIZE.Y
                        ),
                        _totalTime
                    );
                }
                _mouseHeld = true;
            } else {
                _mouseHeld = false;
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

            _ringWarpShader.Parameters["SecondTexture"]?.SetValue(Game1.TARGET_2);
            _ringWarpShader.Parameters["time"].SetValue(_totalTime);
            _ringWarpShader.Parameters["numDrops"].SetValue(_totalDrops);
            _ringWarpShader.Parameters["drops"].SetValue(_drops);

            spriteBatch.Begin(effect: _ringWarpShader, samplerState: SamplerState.PointWrap);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing the result to the screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_3);
        }

        private void AddDrop(Vector2 pos, float time)
        {
            _drops[_currDrop] = new Vector4(pos.X, pos.Y, time, _currOffset);
            _currDrop = (_currDrop + 1) % MAX_DROPS;
            _currOffset *= -1.0f;
            if (_totalDrops < 20) {_totalDrops++;}
        }

        public override void Reset()
        {
            base.Reset();

            _drops = new Vector4[20];
            _currDrop = 0;
            _currOffset = 1.0f;
            _totalDrops = 0;
            _mouseHeld = false;
        }
    }
}