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

    private Effect _dripdropShader;
    private Effect _wavepoolShader;

    public static readonly int MAX_DROPS = 20;
    private int _currDrop = 0;
    private int _totalDrops = 0;
    private Vector3[] _drops;
    private bool _mouseHeld = false;

    public Rectangle SCREEN_RECT
    {
        get { return new Rectangle(Point.Zero, Window.ClientBounds.Size); }
    }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.75f);
        _graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.75f);
        _graphics.ApplyChanges();

        _totalTime = 0.0f;
        _drops = new Vector3[20];

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _texture = this.Content.Load<Texture2D>("tbound-screenshot-2");
        _dripdropShader = this.Content.Load<Effect>("Shaders/Dripdrop");
        _wavepoolShader = this.Content.Load<Effect>("Shaders/Wavepool");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
            if (!_mouseHeld) {
                Point dropPoint = Mouse.GetState().Position;
                AddDrop(
                    new Vector2(dropPoint.X, SCREEN_RECT.Height - dropPoint.Y),
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
            (float)SCREEN_RECT.Width / (float)_texture.Width,
            (float)SCREEN_RECT.Height / (float)_texture.Height
        );

        GraphicsDevice.Clear(Color.CornflowerBlue);

        // drawing background
        RenderTarget2D customTarget = new RenderTarget2D(GraphicsDevice, SCREEN_RECT.Width, SCREEN_RECT.Height);
        GraphicsDevice.SetRenderTarget(customTarget);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        _spriteBatch.End();

        // drawing it again!
        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin();
        _spriteBatch.Draw(customTarget, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
        _spriteBatch.End();

        // drawing a watery reflection!
        _spriteBatch.Begin(effect: _dripdropShader);
        _dripdropShader.Parameters["time"].SetValue(_totalTime);
        _dripdropShader.Parameters["texOffsetMult"].SetValue(0.15f);
        _dripdropShader.Parameters["sharpness"].SetValue(0.04f);

        _dripdropShader.Parameters["numDrops"].SetValue(_totalDrops);
        _dripdropShader.Parameters["drops"].SetValue(_drops);

        /*
        _spriteBatch.Begin(effect: _wavepoolShader);
        _wavepoolShader.Parameters["time"].SetValue(_totalTime);
        _wavepoolShader.Parameters["texOffsetMult"].SetValue(0.01f);
        _wavepoolShader.Parameters["period"].SetValue(32.0f);
        */

        _spriteBatch.Draw(
            customTarget,
            new Vector2(0.0f, (float)SCREEN_RECT.Height / 2.0f),
            new Rectangle(0, 0, customTarget.Width, customTarget.Height / 2),
            Color.CornflowerBlue,
            0.0f,
            Vector2.Zero,
            Vector2.One,
            SpriteEffects.FlipVertically,
            0.0f
        );
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
