using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class WavyRingShader : OurShader
    {
        private Effect _wavyRingShader;

        public WavyRingShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            _wavyRingShader = content.Load<Effect>("Shaders/WavyRing");

            base.LoadContent(content);
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 pos = (Game1.SCREEN_RECT.Size.ToVector2() * 0.5f) - _texture.Bounds.Size.ToVector2();

            graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _wavyRingShader);
            spriteBatch.Draw(_texture, pos, null, Color.White, 0.0f, Vector2.Zero, new Vector2(2.0f), SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}