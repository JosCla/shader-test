using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class VertexWaveShader : OurShader
    {
        private Effect _vertexWaveEffect;
        
        public VertexWaveShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _vertexWaveEffect = content.Load<Effect>("Shaders/VertexWave");
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Matrix view = Matrix.Identity;

            int width = graphicsDevice.Viewport.Width;
            int height = graphicsDevice.Viewport.Height;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

            // System.Console.WriteLine(Vector2.Transform(new Vector2(0f, 0f), projection));
            // System.Console.WriteLine(Vector2.Transform(new Vector2(100f, 100f), projection));

            Matrix mvpMatrix = view * projection;

            _vertexWaveEffect.Parameters["mvpMatrix"]?.SetValue(mvpMatrix);
            spriteBatch.Begin(effect: _vertexWaveEffect);
            spriteBatch.Draw(_texture, new Vector2(-50.0f, -80.0f), null, Color.White);
            spriteBatch.End();
        }
    }
}