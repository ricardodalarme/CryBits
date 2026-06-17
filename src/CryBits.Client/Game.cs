using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Graphics;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Iguina;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Menu;
using CryBits.Client.Logic;
using CryBits.Client.Managers;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Offline;
using CryBits.Client.Systems;
using CryBits.Client.UI;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Persistence.Repositories;
using System.Diagnostics;
using static CryBits.Definitions.Globals;
using Scr = CryBits.Client.UI.GameState;

namespace CryBits.Client;

public sealed class Game : IDisposable
{
    private readonly bool _offline;
    private EmbeddedHostRunner? _hostRunner;
    private Connection? _connection;
    private RenderPipeline? _renderPipeline;
    private InputManager? _inputManager;
    private SystemScheduler? _scheduler;
    private IguinaUiRoot? _iguinaUi;
    private MenuScreen? _iguinaMenuScreen;
    private GameScreen? _iguinaGameScreen;
    private bool _working = true;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public static short Fps { get; private set; }
    public static bool Working
    {
        get => Instance?._working ?? false;
        set { Instance?._working = value; }
    }

    private static Game? Instance { get; set; }

    public Game(bool offline)
    {
        _offline = offline;
        Instance = this;
    }

    public void Run()
    {
        Initialize();
        Loop();
        Dispose();
    }

    private void Initialize()
    {
        Directories.Create();
        OptionsRepository.Read();
        Renderer.Instance.Init();

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

        _connection.Start(onDisconnected: OnDisconnected);

        Window.Instance.Bind();
        GameInput.Instance.Bind();

        var context = GameContext.Instance;
        var audioManager = AudioManager.Instance;
        var contentRepository = new ContentRepository();
        var cat = DefinitionCatalog.Instance;

        PacketDispatcher.Register(new AuthHandler(cat));
        PacketDispatcher.Register(new AccountHandler(audioManager, context, cat));
        PacketDispatcher.Register(new PlayerHandler(context, cat));
        PacketDispatcher.Register(new MapHandler(context, MapSender.Instance, audioManager, cat, contentRepository));
        PacketDispatcher.Register(new NpcHandler(context, cat));
        PacketDispatcher.Register(new CombatHandler(context));
        PacketDispatcher.Register(new ChatHandler(Chat.Instance));
        PacketDispatcher.Register(new PartyHandler(PartySender.Instance, context));
        PacketDispatcher.Register(new TradeHandler(TradeSender.Instance, context, cat));
        PacketDispatcher.Register(new ShopHandler(cat));
        PacketDispatcher.Register(new ClassHandler(cat));
        PacketDispatcher.Register(new ItemHandler(cat));
        AudioManager.Instance.LoadSounds();

        _scheduler = SystemScheduler.Instance;
        _scheduler.Initialize();
        _renderPipeline = RenderPipeline.Instance;
        _inputManager = InputManager.Instance;

        _iguinaUi = new IguinaUiRoot(Renderer.Instance, _inputManager);
        _iguinaMenuScreen = new MenuScreen(
            _iguinaUi.System,
            Renderer.Instance,
            CharacterRenderer.Instance,
            DefinitionCatalog.Instance);

        _renderPipeline.IguinaUi = _iguinaUi;

        _iguinaGameScreen = new GameScreen(
            _iguinaUi.System,
            Renderer.Instance,
            CharacterRenderer.Instance,
            EquipmentRenderer.Instance,
            ItemRenderer.Instance,
            GameContext.Instance,
            DefinitionCatalog.Instance,
            AudioManager.Instance,
            _inputManager);

        Window.Instance.OpenMenu();
        _iguinaMenuScreen?.ShowLogin();
    }

    private void Loop()
    {
        long timer1000 = 0;
        short fps = 0;

        while (_working)
        {
            try
            {
                _connection?.Poll();
                _renderPipeline?.Present();

                _inputManager?.BeginFrame();
                Renderer.Instance.RenderWindow.DispatchEvents();

                var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                // Show/hide Iguina screens based on current game screen state.
                if (Scr.CurrentScreen == ScreenType.Menu)
                {
                    if (_iguinaGameScreen != null && _iguinaGameScreen.IsVisible)
                        _iguinaGameScreen.Hide();
                }
                else
                {
                    if (_iguinaMenuScreen != null && _iguinaMenuScreen.IsVisible)
                        _iguinaMenuScreen.Hide();
                    if (_iguinaGameScreen != null && !_iguinaGameScreen.IsVisible)
                        _iguinaGameScreen.Show();
                }

                // Update Iguina UI after input events are dispatched.
                // Called after Show() so newly created entities get laid out immediately.
                _iguinaUi?.Update(deltaTime);

                _scheduler?.Update.BeforeUpdate(in deltaTime);
                _scheduler?.Update.Update(in deltaTime);
                _scheduler?.Update.AfterUpdate(in deltaTime);

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

    private void OnDisconnected()
    {
        GameContext.Instance.Reset();
        Window.Instance.OpenMenu();
        _iguinaMenuScreen?.ShowLogin();
    }

    public void Dispose()
    {
        _working = false;
        _scheduler?.Dispose();
        _hostRunner?.Stop();
        _hostRunner?.Dispose();
        _connection?.Disconnect();
    }
}
