using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shader_test;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _texture;
    private Effect _shader;
    private float _totalTime;

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _texture = this.Content.Load<Texture2D>("tbound-screenshot-2");
        _shader = this.Content.Load<Effect>("Shaders/Wavepool");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

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
        _spriteBatch.Begin(effect: _shader);
        _shader.Parameters["time"].SetValue(_totalTime);
        _shader.Parameters["period"].SetValue(32.0f);
        _shader.Parameters["texOffsetMult"].SetValue(0.01f);
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
}
