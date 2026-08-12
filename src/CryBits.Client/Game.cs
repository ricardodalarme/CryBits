using CryBits.Client.Core;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Network;
using CryBits.Client.Input;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI;
using CryBits.Client.UI.Menu;
using CryBits.Definitions.Catalog;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Intents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static CryBits.Definitions.Globals;

namespace CryBits.Client;

public sealed class Game : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly Connection _connection;

    private SpriteBatch _spriteBatch = null!;
    private UiContext _uiContext = null!;
    private InputManager _inputManager = null!;
    private AudioManager _audioManager = null!;
    private DefinitionCatalog _catalog = null!;
    private MenuScreen _menuScreen = null!;
    private GameSession? _activeSession;

    private short _fpsCounter;
    private double _nextFpsReset;

    /// <summary>Latest measured frame rate.</summary>
    public short Fps { get; private set; }

    public Game(Connection connection)
    {
        _connection = connection;

        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = ScreenWidth,
            PreferredBackBufferHeight = ScreenHeight
        };
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        Window.Title = Config.GameName;
        Window.AllowUserResizing = false;
        IsFixedTimeStep = false;
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Touch the static font cache so it pre-loads Georgia.ttf before any UI starts.
        _ = Fonts.System;

        // Build the MonoGame SpriteBatch directly on the GraphicsDevice.
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Register the GraphicsDevice so the static Textures cache can lazy-load assets.
        Textures.Initialize(GraphicsDevice);

        var viewport = GraphicsDevice.Viewport;
        _uiContext = new UiContext(this, GraphicsDevice, viewport.Width, viewport.Height);
        _inputManager = new InputManager(_uiContext.Desktop);
        _audioManager = new AudioManager();
        _catalog = new DefinitionCatalog();

        RegisterIntentTypes();

        _menuScreen = new MenuScreen(
            _audioManager, _uiContext, new AuthSender(_connection),
            new AccountSender(_connection), new PortraitRenderer(_spriteBatch), _catalog, _connection);

        PacketDispatcher.Register(new AuthHandler(_catalog, _uiContext, _menuScreen));
        PacketDispatcher.Register(new AccountHandler(this, _menuScreen));
        PacketDispatcher.Register(new ContentHandler(_catalog, _menuScreen));

        _audioManager.LoadSounds();
        _menuScreen.Open();

        _connection.Start(onDisconnected: EndSession);

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _connection.Poll();
        _inputManager.Capture();
        _activeSession?.Scheduler.Update(_activeSession.World, deltaTime);

        _fpsCounter++;
        if (gameTime.TotalGameTime.TotalSeconds >= _nextFpsReset)
        {
            Fps = _fpsCounter;
            _fpsCounter = 0;
            _nextFpsReset = gameTime.TotalGameTime.TotalSeconds + 1.0;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _activeSession?.RenderPipeline.Present();

        _uiContext.Render();

        base.Draw(gameTime);
    }

    public void StartSession(long localPlayerId)
    {
        if (_activeSession != null) return;
        _activeSession = new GameSession(
            _uiContext, _spriteBatch, _inputManager, _audioManager, _catalog, _connection,
            () => Fps, localPlayerId
        );
    }

    public void OpenSessionScreen()
    {
        if (_activeSession == null) return;
        _menuScreen.Unbind();
        _activeSession.OpenScreen();
    }

    public void EndSession()
    {
        if (_activeSession == null) return;
        _activeSession.Dispose();
        _activeSession = null;
        _menuScreen.Open();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _activeSession?.Dispose();
            _spriteBatch?.Dispose();
        }

        base.Dispose(disposing);
    }

    private static void RegisterIntentTypes()
    {
        IntentRegistry.Register<MoveIntent>(1);
        IntentRegistry.Register<AttackIntent>(2);
        IntentRegistry.Register<AddPointIntent>(3);
        IntentRegistry.Register<CollectItemIntent>(4);
        IntentRegistry.Register<DropItemIntent>(5);
        IntentRegistry.Register<InventorySwapIntent>(6);
        IntentRegistry.Register<InventoryUseIntent>(7);
        IntentRegistry.Register<EquipmentRemoveIntent>(8);
        IntentRegistry.Register<HotbarAddIntent>(9);
        IntentRegistry.Register<HotbarSwapIntent>(10);
        IntentRegistry.Register<HotbarUseIntent>(11);
        IntentRegistry.Register<ChatMessageIntent>(12);
        IntentRegistry.Register<PartyInviteIntent>(13);
        IntentRegistry.Register<PartyAcceptIntent>(14);
        IntentRegistry.Register<PartyDeclineIntent>(15);
        IntentRegistry.Register<PartyLeaveIntent>(16);
        IntentRegistry.Register<TradeInviteIntent>(17);
        IntentRegistry.Register<TradeAcceptIntent>(18);
        IntentRegistry.Register<TradeDeclineIntent>(19);
        IntentRegistry.Register<TradeLeaveIntent>(20);
        IntentRegistry.Register<TradeOfferIntent>(21);
        IntentRegistry.Register<TradeOfferStateIntent>(22);
        IntentRegistry.Register<ShopBuyIntent>(23);
        IntentRegistry.Register<ShopSellIntent>(24);
        IntentRegistry.Register<ShopCloseIntent>(25);
    }
}
