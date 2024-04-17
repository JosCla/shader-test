using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace shader_test;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _texture;

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _texture = this.Content.Load<Texture2D>("tbound-screenshot");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // drawing background
        RenderTarget2D customTarget = new RenderTarget2D(GraphicsDevice, SCREEN_RECT.Width, SCREEN_RECT.Height);
        GraphicsDevice.SetRenderTarget(customTarget);

        _spriteBatch.Begin();

        Vector2 scale = new Vector2(
            (float)SCREEN_RECT.Width / (float)_texture.Width,
            (float)SCREEN_RECT.Height / (float)_texture.Height
        );
        for (int i = 0; i < 10; i++) {
            _spriteBatch.Draw(_texture, new Vector2(i * 100.0f), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        _spriteBatch.End();



        // drawing it again!

        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin();

        _spriteBatch.Draw(customTarget, new Vector2(100.0f, 100.0f), null, Color.White, 50.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
