using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shader_test;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _texture;
    private float _totalTime;

    private RenderTarget2D _renderTarget;
    private RenderTarget2D _tempTarget;

    private Effect _dripdropShader;
    private Effect _wavepoolShader;
    private Effect _colorOffsetShader;

    public static readonly int MAX_DROPS = 20;
    private int _currDrop = 0;
    private int _totalDrops = 0;
    private Vector3[] _drops;
    private bool _mouseHeld = false;

    public Rectangle SCREEN_RECT
    {
        get { return new Rectangle(Point.Zero, Window.ClientBounds.Size); }
    }
    public static Point RENDER_SCREEN_SIZE = new Point(162, 120);

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.75f * (27f / 20f));
        _graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.75f);
        _graphics.ApplyChanges();

        _renderTarget = new RenderTarget2D(GraphicsDevice, RENDER_SCREEN_SIZE.X, RENDER_SCREEN_SIZE.Y);
        _tempTarget = new RenderTarget2D(GraphicsDevice, RENDER_SCREEN_SIZE.X, RENDER_SCREEN_SIZE.Y);

        _totalTime = 0.0f;
        _drops = new Vector3[20];

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _texture = this.Content.Load<Texture2D>("tbound-screenshot-3");
        _dripdropShader = this.Content.Load<Effect>("Shaders/Dripdrop");
        _wavepoolShader = this.Content.Load<Effect>("Shaders/Wavepool");
        _colorOffsetShader = this.Content.Load<Effect>("Shaders/Coloroffset");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
            if (!_mouseHeld && _totalTime > 5.0f) {
                Point dropPoint = Mouse.GetState().Position;
                Vector2 relativeDropPoint = new Vector2(
                    (float)dropPoint.X / (float)SCREEN_RECT.Width,
                    (float)dropPoint.Y / (float)SCREEN_RECT.Height
                );
                AddDrop(
                    new Vector2(
                        relativeDropPoint.X * (float)RENDER_SCREEN_SIZE.X,
                        relativeDropPoint.Y * (float)RENDER_SCREEN_SIZE.Y
                    ),
                    _totalTime
                );
            }
            _mouseHeld = true;
        } else {
            _mouseHeld = false;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Vector2 scale = new Vector2(
            (float)RENDER_SCREEN_SIZE.X / (float)_texture.Width,
            (float)RENDER_SCREEN_SIZE.Y / (float)_texture.Height
        );

        GraphicsDevice.Clear(Color.CornflowerBlue);

        // drawing background to a texture
        GraphicsDevice.SetRenderTarget(_tempTarget);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        _spriteBatch.End();

        // drawing it again to a small screen
        GraphicsDevice.SetRenderTarget(_renderTarget);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_tempTarget, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
        _spriteBatch.End();

        // drawing a watery reflection!
        _spriteBatch.Begin(effect: _dripdropShader, samplerState: SamplerState.PointWrap);
        _dripdropShader.Parameters["time"].SetValue(_totalTime);
        _dripdropShader.Parameters["timeScaleFactor"].SetValue(10.0f);
        _dripdropShader.Parameters["texOffsetMult"].SetValue(0.1f);
        _dripdropShader.Parameters["sharpness"].SetValue(0.08f);

        _dripdropShader.Parameters["numDrops"].SetValue(_totalDrops);
        _dripdropShader.Parameters["drops"].SetValue(_drops);

        /*
        _spriteBatch.Begin(effect: _wavepoolShader);
        _wavepoolShader.Parameters["time"].SetValue(_totalTime);
        _wavepoolShader.Parameters["texOffsetMult"].SetValue(0.01f);
        _wavepoolShader.Parameters["period"].SetValue(32.0f);
        */

        _spriteBatch.Draw(
            _tempTarget,
            new Vector2(0.0f, (float)_renderTarget.Height / 2.0f),
            new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height / 2),
            Color.CornflowerBlue,
            0.0f,
            Vector2.Zero,
            Vector2.One,
            SpriteEffects.FlipVertically,
            0.0f
        );

        _spriteBatch.End();

        /*
        _spriteBatch.Begin(effect: _colorOffsetShader, samplerState: SamplerState.PointWrap);
        float time = Math.Max(0.0f, _totalTime - 3.0f);
        float intensity = time * time * time;
        _colorOffsetShader.Parameters["redOffset"].SetValue(new Vector3(0.0f, 0.01f, 0.0f) * intensity);
        _colorOffsetShader.Parameters["greenOffset"].SetValue(new Vector3(-0.006f, -0.006f, 0.0f) * intensity);
        _colorOffsetShader.Parameters["blueOffset"].SetValue(new Vector3(0.006f, -0.006f, 0.0f) * intensity);

        _spriteBatch.Draw(
            _renderTarget,
            Vector2.Zero,
            null,
            Color.White,
            0.0f,
            Vector2.Zero,
            Vector2.One,
            SpriteEffects.None,
            0.0f
        );
        _spriteBatch.End();
        */

        // finally copying that all to the screen
        GraphicsDevice.SetRenderTarget(null);

        Vector2 screenScale = new Vector2(
            (float)SCREEN_RECT.Width / (float)_renderTarget.Width,
            (float)SCREEN_RECT.Height / (float)_renderTarget.Height
        );
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_renderTarget, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, screenScale, SpriteEffects.None, 0.0f);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void AddDrop(Vector2 pos, float time)
    {
        _drops[_currDrop] = new Vector3(pos.X, pos.Y, time);
        _currDrop = (_currDrop + 1) % MAX_DROPS;
        if (_totalDrops < 20) {_totalDrops++;}
    }
}
