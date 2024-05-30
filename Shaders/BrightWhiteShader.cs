using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shader_test
{
    public class BrightWhiteShader : OurShader
    {
        private Effect _brightWhiteShader;
        private float _threshold;

        public BrightWhiteShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _brightWhiteShader = content.Load<Effect>("Shaders/BrightWhite");
        }

        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                Point mousePos = Mouse.GetState().Position;
                Vector2 relativeMousePos = new Vector2(
                    (float)mousePos.X / (float)Game1.SCREEN_RECT.Width,
                    (float)mousePos.Y / (float)Game1.SCREEN_RECT.Height
                );

                _threshold = relativeMousePos.X;
            }
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing background to a texture
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

            graphicsDevice.SetRenderTarget(Game1.TARGET_3);
            graphicsDevice.Clear(Color.Transparent);

            // drawing it, again, to a simple output and a brightness output
            RenderTargetBinding[] bindings = new RenderTargetBinding[2] {
                new RenderTargetBinding(Game1.TARGET_2),
                new RenderTargetBinding(Game1.TARGET_3)
            };
            graphicsDevice.SetRenderTargets(bindings);

            _brightWhiteShader.Parameters["brightThreshold"].SetValue(_threshold);
            spriteBatch.Begin(effect: _brightWhiteShader, blendState: BlendState.Opaque);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing the bright version to the screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_3);
        }

        public override void Reset()
        {
            base.Reset();

            _threshold = 0.75f;
        }
    }
}