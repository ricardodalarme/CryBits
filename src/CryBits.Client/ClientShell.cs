using CryBits.Client.Core;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Input;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Offline;
using CryBits.Client.Rendering;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI;
using CryBits.Client.UI.Menu;
using CryBits.Definitions.Catalog;
using CryBits.Protocol.Serialization;
using CryBits.Simulation;
using CryBits.Simulation.Intents;
using SFML.Graphics;
using System.Diagnostics;
using static CryBits.Definitions.Globals;

namespace CryBits.Client;

public sealed class ClientShell(bool offline) : IDisposable
{
    private readonly bool _offline = offline;
    private EmbeddedHostRunner? _hostRunner;
    private Connection? _connection;
    private UiContext _uiContext = null!;
    private SpriteBatch _spriteBatch = null!;
    private InputManager? _inputManager;
    private MenuScreen _menuScreen = null!;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private DefinitionCatalog _catalog = null!;
    private AudioManager _audioManager = null!;

    // Transient session management
    private GameSession? _activeSession;

    public static short Fps { get; private set; }

    public void Run()
    {
        Initialize();
        Loop();
        Dispose();
    }

    private void Initialize()
    {
        RegisterComponentTypes();
        Directories.Create();
        OptionsRepository.Read();

        // ── Create all infrastructure instances FIRST ──
        _audioManager = new AudioManager();
        _uiContext = new UiContext();
        _inputManager = new InputManager();
        _spriteBatch = new SpriteBatch(_inputManager);
        _catalog = new DefinitionCatalog();

        // ── Initialize in dependency order ──
        _spriteBatch.Init(_uiContext);
        _uiContext.Initialize((uint)_spriteBatch.RenderWindow.Size.X, (uint)_spriteBatch.RenderWindow.Size.Y, _spriteBatch.RenderWindow);
        _inputManager.Initialize(_uiContext.UISystem!, _spriteBatch.RenderWindow);

        // ── Network ──
        if (_offline)
        {
            _hostRunner = new EmbeddedHostRunner();
            _hostRunner.Start();
            _connection = _hostRunner.ClientConnection;
        }
        else
        {
            var clientTransport = new UdpClientTransport();
            clientTransport.Connect("localhost", Config.Port, Config.GameName);
            _connection = new Connection(clientTransport);
        }
        _spriteBatch.Connection = _connection;
        _connection.Start(onDisconnected: OnDisconnected);

        // ── Intent registrations ──
        RegisterIntentTypes();

        _menuScreen = new MenuScreen(_audioManager, _uiContext, new AuthSender(_connection), new AccountSender(_connection, _catalog), new PortraitRenderer(_spriteBatch), _catalog, _connection);

        // ── Shell Packet handlers ──
        PacketDispatcher.Register(new AuthHandler(_catalog, _uiContext, _menuScreen));
        PacketDispatcher.Register(new AccountHandler(this, _menuScreen));
        PacketDispatcher.Register(new ContentHandler(_catalog, _menuScreen));

        _audioManager.LoadSounds();
        _menuScreen.Open();
    }

    public void StartSession(long localPlayerId)
    {
        if (_activeSession != null) return;
        _activeSession = new GameSession(
            _uiContext, _spriteBatch, _inputManager!, _audioManager, _catalog, _connection!, _menuScreen, localPlayerId
        );
    }

    public void OpenSessionScreen()
    {
        _activeSession?.OpenScreen();
    }

    public void EndSession()
    {
        if (_activeSession == null) return;
        _activeSession.Dispose();
        _activeSession = null;
        _menuScreen.Open();
    }

    private void Loop()
    {
        long timer1000 = 0;
        short fps = 0;

        while (_spriteBatch.RenderWindow.IsOpen)
        {
            try
            {
                _connection?.Poll();

                _inputManager?.BeginFrame();
                _spriteBatch.RenderWindow.DispatchEvents();

                var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                if (_activeSession != null)
                {
                    _activeSession.Update(deltaTime);
                    _activeSession.RenderPipeline.Present();
                }
                else
                {
                    _uiContext.Update(deltaTime);
                    PresentMenu();
                }

                if (timer1000 < Environment.TickCount64)
                {
                    Fps = fps;
                    fps = 0;
                    timer1000 = Environment.TickCount64 + 1000;
                }
                else
                {
                    fps++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Main loop threw an exception: {ex}");
            }
        }
    }

    private void PresentMenu()
    {
        var window = _spriteBatch.RenderWindow;
        window.Clear(Color.Black);

        // Reset view for UI drawing
        var originalView = window.DefaultView;
        window.SetView(originalView);

        _uiContext.Draw();

        var uiTarget = _uiContext.Target;
        if (uiTarget != null)
        {
            var sprite = new Sprite(uiTarget.Texture);
            window.Draw(sprite);
        }

        _uiContext.PostDraw?.Invoke();

        window.Display();
    }

    private void OnDisconnected()
    {
        EndSession();
    }

    private static void RegisterComponentTypes()
    {
        ComponentTypes.RegisterDefault();
        ComponentTypeRegistry.Register<Components.NetworkId>(18);
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

    public void Dispose()
    {
        _activeSession?.Dispose();
        _hostRunner?.Stop();
        _hostRunner?.Dispose();
        _connection?.Disconnect();
    }
}
