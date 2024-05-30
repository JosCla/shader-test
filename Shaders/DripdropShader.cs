using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shader_test
{
    public class DripdropShader : OurShader
    {
        public static readonly int MAX_DROPS = 20;

        private int _currDrop;
        private int _totalDrops;
        private Vector3[] _drops;
        private bool _mouseHeld;

        private Effect _dripdropShader;

        public DripdropShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            _dripdropShader = content.Load<Effect>("Shaders/Dripdrop");

            base.LoadContent(content);
        }

        public override void Update(float timeElapsed)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                if (!_mouseHeld) {
                    Point dropPoint = Mouse.GetState().Position;
                    Vector2 relativeDropPoint = new Vector2(
                        (float)dropPoint.X / (float)Game1.SCREEN_RECT.Width,
                        (float)dropPoint.Y / (float)Game1.SCREEN_RECT.Height
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
            // drawing background to a texture
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

            // drawing it again to a small screen
            graphicsDevice.SetRenderTarget(Game1.TARGET_2);

            spriteBatch.Begin();
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            // drawing a watery reflection!
            spriteBatch.Begin(effect: _dripdropShader, samplerState: SamplerState.PointWrap);
            _dripdropShader.Parameters["time"].SetValue(_totalTime);
            _dripdropShader.Parameters["timeScaleFactor"].SetValue(10.0f);
            _dripdropShader.Parameters["texOffsetMult"].SetValue(0.1f);
            _dripdropShader.Parameters["sharpness"].SetValue(0.08f);

            _dripdropShader.Parameters["numDrops"].SetValue(_totalDrops);
            _dripdropShader.Parameters["drops"].SetValue(_drops);

            spriteBatch.Draw(
                Game1.TARGET_1,
                new Vector2(0.0f, (float)Game1.TARGET_2.Height / 2.0f),
                new Rectangle(0, 0, Game1.TARGET_2.Width, Game1.TARGET_2.Height / 2),
                Color.CornflowerBlue,
                0.0f,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.FlipVertically,
                0.0f
            );

            spriteBatch.End();

            // finally copying that all to the screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_2);
        }

        private void AddDrop(Vector2 pos, float time)
        {
            _drops[_currDrop] = new Vector3(pos.X, pos.Y, time);
            _currDrop = (_currDrop + 1) % MAX_DROPS;
            if (_totalDrops < 20) {_totalDrops++;}
        }

        public override void Reset()
        {
            base.Reset();

            _drops = new Vector3[20];
            _currDrop = 0;
            _totalDrops = 0;
            _mouseHeld = false;
        }
    }
}