using CryBits.Client.Components;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Graphics;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Logic;
using CryBits.Client.Offline;
using CryBits.Client.Systems;
using CryBits.Client.UI;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.UI.Menu;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Persistence.Repositories;
using CryBits.Protocol.Serialization;
using CryBits.Simulation;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using System.Diagnostics;
using static CryBits.Definitions.Globals;

namespace CryBits.Client;

public sealed class Game : IDisposable
{
    private readonly bool _offline;
    private EmbeddedHostRunner? _hostRunner;
    private Connection? _connection;
    private UiContext _uiContext = null!;
    private Renderer _renderer = null!;
    private RenderPipeline? _renderPipeline;
    private InputManager? _inputManager;
    private SystemScheduler? _scheduler;
    private GameContext _context = null!;
    private MenuScreen _menu = null!;
    private GameScreen? _gameScreen;
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
        RegisterComponentTypes();
        Directories.Create();
        OptionsRepository.Read();

        // ── Create all infrastructure instances FIRST ──
        var audio = new AudioManager();
        var uiContext = new UiContext(); _uiContext = uiContext;
        var input = new InputManager();
        var renderer = new Renderer(input); _renderer = renderer;
        var cat = new DefinitionCatalog();
        var context = new GameContext(cat); _context = context;

        // ── Initialize in dependency order ──
        _renderer.Init(uiContext);
        _uiContext.Initialize((uint)_renderer.RenderWindow.Size.X, (uint)_renderer.RenderWindow.Size.Y, _renderer.RenderWindow);
        input.Initialize(_uiContext.UISystem!, _renderer.RenderWindow);

        var camera = new CameraManager(renderer.RenderWindow);
        var mapRenderer = new MapRenderer(renderer, context, camera);
        var characterRenderer = new CharacterRenderer(renderer);
        var itemRenderer = new ItemRenderer(renderer);
        var equipmentRenderer = new EquipmentRenderer(renderer, context, cat);

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
        _renderer.Connection = _connection;
        _connection.Start(onDisconnected: OnDisconnected);

        var intentSender = new IntentSender(_connection);
        var authSender = new AuthSender(_connection);
        var accountSender = new AccountSender(_connection, cat);
        var contentSender = new ContentSender(_connection);

        // ── Intent registrations ──
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

        // ── Scheduler ──
        var scheduler = new SystemScheduler(context, input, intentSender, audio, camera, renderer, uiContext);
        _scheduler = scheduler;

        // ── Chat ──
        var chat = new Chat(intentSender, uiContext);

        // ── Views that other views depend on ──
        var tooltipView = new TooltipView(uiContext, itemRenderer, cat);
        var shopView = new ShopView(uiContext, intentSender, itemRenderer, cat, tooltipView);

        // ── Screens ──
        _menu = new MenuScreen(audio, uiContext, authSender, accountSender, characterRenderer, context, cat, _connection);
        var gameInput = new GameInput(intentSender, chat, input, uiContext);
        _gameScreen = new GameScreen(uiContext, context, intentSender, renderer, itemRenderer, equipmentRenderer,
            characterRenderer, input, audio, cat, tooltipView, shopView, _menu, chat, gameInput);

        // ── System initialization ──
        scheduler.Initialize();
        var renderPipeline = new RenderPipeline(renderer, camera, mapRenderer, scheduler, uiContext);
        _renderPipeline = renderPipeline;
        _inputManager = input;

        // ── Packet handlers ──
        var contentRepo = new ContentRepository();
        var mapRepo = new MapRepository();
        PacketDispatcher.Register(new AuthHandler(cat, uiContext, _menu));
        PacketDispatcher.Register(new AccountHandler(context, _menu, _gameScreen));
        PacketDispatcher.Register(new MapHandler(context, contentSender, audio, mapRepo));
        PacketDispatcher.Register(new KeyframeHandler(new Replication.SnapshotApplier(context.World, context, cat)));
        PacketDispatcher.Register(new ChatHandler(chat));
        PacketDispatcher.Register(new PartyHandler(intentSender, context, _gameScreen));
        PacketDispatcher.Register(new TradeHandler(intentSender, context, _gameScreen));
        PacketDispatcher.Register(new ContentHandler(cat, _menu));
        PacketDispatcher.Register(new ShopHandler(cat, _gameScreen));

        audio.LoadSounds();
        _menu.Open();
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
                _renderer.RenderWindow.DispatchEvents();

                var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                _uiContext.Update(deltaTime);

                _scheduler?.Simulation.Update(deltaTime);

                var ctx = _context;
                if (ctx.LocalPlayer.Entity is { } playerEntityId)
                {
                    var level = ctx.World.Get<LevelComponent>(playerEntityId);
                    var attrs = ctx.World.Get<AttributesComponent>(playerEntityId);
                    if (level != null && attrs != null)
                    {
                        short total = 0;
                        foreach (var v in attrs.Values) total += v;
                        if (level.TotalAttributes != total)
                            ctx.World.Set(playerEntityId, level with { TotalAttributes = total });
                    }
                    if (_gameScreen != null && _uiContext.CurrentScreen == ScreenType.Game)
                        _gameScreen.CharacterView.Update();
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

    private void OnDisconnected()
    {
        _context.Reset();
        _gameScreen?.Unbind();
        _menu.Open();
    }

    private static void RegisterComponentTypes()
    {
        ComponentTypes.RegisterDefault();
        ComponentTypeRegistry.Register<NetworkId>(18);
    }

    public void Dispose()
    {
        _working = false;
        _hostRunner?.Stop();
        _hostRunner?.Dispose();
        _connection?.Disconnect();
    }
}
