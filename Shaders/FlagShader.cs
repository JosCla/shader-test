using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class FlagShader : OurShader
    {
        private PhysicsChain _chain;

        public FlagShader() {
            this._chain = new PhysicsChain(new Vector2(200.0f, 200.0f), 10, 20.0f);
        }

        public override void Update(float timeElapsed)
        {
            base.Update(timeElapsed);

            _chain.Update(timeElapsed);
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            System.Console.WriteLine("frame");
            foreach (Vector2 link in _chain.GetLinkPoses()) {
                System.Console.WriteLine(link);
                spriteBatch.Draw(
                    _texture,
                    link,
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
    }
}