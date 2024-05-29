using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class ColorOffsetShader : OurShader
    {
        private Effect _colorOffsetShader;

        public ColorOffsetShader() : base() {}

        public override void LoadContent(ContentManager content)
        {
            _colorOffsetShader = content.Load<Effect>("Shaders/Coloroffset");

            base.LoadContent(content);
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(
                (float)Game1.SCREEN_RECT.Width / (float)_texture.Width,
                (float)Game1.SCREEN_RECT.Height / (float)_texture.Height
            );

            spriteBatch.Begin(effect: _colorOffsetShader, samplerState: SamplerState.PointWrap);
            float time = Math.Max(0.0f, _totalTime - 1.0f);
            float intensity = time * time * time;
            _colorOffsetShader.Parameters["redOffset"].SetValue(new Vector3(0.0f, 0.01f, 0.0f) * intensity);
            _colorOffsetShader.Parameters["greenOffset"].SetValue(new Vector3(-0.006f, -0.006f, 0.0f) * intensity);
            _colorOffsetShader.Parameters["blueOffset"].SetValue(new Vector3(0.006f, -0.006f, 0.0f) * intensity);

            spriteBatch.Draw(
                _texture,
                Vector2.Zero,
                null,
                Color.White,
                0.0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.0f
            );
            spriteBatch.End();
        }

        public override string GetTexturePath()
        {
            return "tbound-screenshot-2";
        }
    }
}