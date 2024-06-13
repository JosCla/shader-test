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

    private RenderTarget2D _target1;
    private RenderTarget2D _target2;
    private RenderTarget2D _target3;

    private List<OurShader> _allShaders;
    private int _currShader;
    private float MAX_SWAP_COOLDOWN = 0.5f;
    private float _swapCooldown;

    private static Game1 _instance;
    public static Game1 INSTANCE
    {
        get { return _instance; }
    }

    public static Rectangle SCREEN_RECT
    {
        get { return new Rectangle(Point.Zero, _instance.Window.ClientBounds.Size); }
    }
    public static Point RENDER_SCREEN_SIZE = new Point(162, 120);

    public static RenderTarget2D TARGET_1
    {
        get { return _instance._target1; }
    }
    public static RenderTarget2D TARGET_2
    {
        get { return _instance._target2; }
    }
    public static RenderTarget2D TARGET_3
    {
        get { return _instance._target3; }
    }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _instance = this;

        _allShaders = new List<OurShader>() {
            new DripdropShader(),
            new WavepoolShader(),
            new ColorOffsetShader(),
            new BrightWhiteShader(),
            new SimpleBlurShader(),
            new BloomShader(),
            new BloomBiasShader(),
            new BoxNoiseShader(),
        };
        _currShader = 0;
        _swapCooldown = 0.0f;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.75f * (27f / 20f));
        _graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.75f);
        _graphics.ApplyChanges();

        _target1 = new RenderTarget2D(GraphicsDevice, RENDER_SCREEN_SIZE.X, RENDER_SCREEN_SIZE.Y);
        _target2 = new RenderTarget2D(GraphicsDevice, RENDER_SCREEN_SIZE.X, RENDER_SCREEN_SIZE.Y);
        _target3 = new RenderTarget2D(GraphicsDevice, RENDER_SCREEN_SIZE.X, RENDER_SCREEN_SIZE.Y);

        foreach (OurShader shader in _allShaders)
            shader.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        foreach (OurShader shader in _allShaders)
            shader.LoadContent(Content);

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            SelectShader(_currShader + 1);
        else if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            SelectShader(_currShader - 1);

        _swapCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        _allShaders[_currShader].Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        GraphicsDevice.SetRenderTarget(null);

        _allShaders[_currShader].Draw(
            (float)gameTime.ElapsedGameTime.TotalSeconds,
            GraphicsDevice,
            _spriteBatch
        );

        base.Draw(gameTime);
    }

    private void SelectShader(int index)
    {
        if (_swapCooldown > 0.0f) return;

        index = index % _allShaders.Count;
        if (index < 0) index += _allShaders.Count;

        _currShader = index;
        _allShaders[index].Reset();

        _swapCooldown = MAX_SWAP_COOLDOWN;
    }
}
