using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace shader_test
{
    public class PerlinShader : OurShader
    {
        private Effect _perlinShader;
        private Random _random;

        public PerlinShader() : base()
        {
            this._random = new Random();
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            _perlinShader = content.Load<Effect>("Shaders/Perlin");

            GenerateLookupTable();
        }

        public override void Draw(float timeElapsed, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            // drawing background to a texture
            DrawTexInTarget(graphicsDevice, spriteBatch, Game1.TARGET_1);

            // using perlin noise to wacky-ify that texture
            graphicsDevice.SetRenderTarget(Game1.TARGET_2);
            _perlinShader.Parameters["texWidth"].SetValue(Game1.TARGET_1.Width);
            _perlinShader.Parameters["texHeight"].SetValue(Game1.TARGET_1.Height);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _perlinShader);
            spriteBatch.Draw(Game1.TARGET_1, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            // drawing target to the screen
            DrawTargetToScreen(graphicsDevice, spriteBatch, Game1.TARGET_2);
        }

        private void GenerateLookupTable()
        {
            // putting numbers 0 to 127 in an array
            List<int> lookup = new List<int>();
            for (int i = 0; i < 128; i++) {
                lookup.Add(i);
            }

            // fisher-yates shuffling the numbers
            for (int i = 0; i < 128; i++) {
                // at each step, choose a random number after this index to swap with
                int swapInd = (int)_random.NextInt64(0, 128 - i) + i;
                int temp = lookup[i];
                lookup[i] = lookup[swapInd];
                lookup[swapInd] = temp;
            }

            // duplicating a small amount of the lookup table to prevent index overflow
            for (int i = 0; i < 12; i++) {
                lookup.Add(lookup[i]);
            }

            // passing that all along to the shader
            _perlinShader.Parameters["lookup"].SetValue(lookup.ToArray());
        }
    }
}