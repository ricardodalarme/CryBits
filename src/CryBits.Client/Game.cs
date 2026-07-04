using CryBits.Client.Components;
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
using CryBits.Client.Rendering.Camera;
using CryBits.Client.Rendering.Effects;
using CryBits.Client.Rendering.Entities;
using CryBits.Client.Rendering.Items;
using CryBits.Client.Rendering.Map;
using CryBits.Client.Rendering.UI;
using CryBits.Client.Systems;
using CryBits.Client.Systems.Character;
using CryBits.Client.Systems.Combat;
using CryBits.Client.Systems.Core;
using CryBits.Client.Systems.Map;
using CryBits.Client.Systems.Movement;
using CryBits.Client.Systems.Player;
using CryBits.Client.UI;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Client.UI.Menu;
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
    private SpriteBatch _spriteBatch = null!;
    private RenderPipeline? _renderPipeline;
    private InputManager? _inputManager;
    private SystemScheduler? _scheduler;
    private GameContext _context = null!;
    private MenuScreen _menuScreen = null!;
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
        var audioManager = new AudioManager();
        var uiContext = new UiContext(); _uiContext = uiContext;
        var inputManager = new InputManager();
        var spriteBatch = new SpriteBatch(inputManager); _spriteBatch = spriteBatch;
        var catalog = new DefinitionCatalog();
        var context = new GameContext(catalog); _context = context;

        // ── Initialize in dependency order ──
        _spriteBatch.Init(uiContext);
        _uiContext.Initialize((uint)_spriteBatch.RenderWindow.Size.X, (uint)_spriteBatch.RenderWindow.Size.Y, _spriteBatch.RenderWindow);
        inputManager.Initialize(_uiContext.UISystem!, _spriteBatch.RenderWindow);

        var cameraManager = new CameraManager(_spriteBatch.RenderWindow);

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

        // Rendering
        var groundRenderers = new List<IRenderer>
        {
            new GroundSpriteRenderer(context.World, _spriteBatch),
            new EntitySpriteRenderer(context.World, _spriteBatch)
        };
        var fringeRenderers = new List<IRenderer>
        {
            new HealthBarRenderer(context.World, _spriteBatch),
            new WeatherParticleRenderer(context.World, _spriteBatch),
            new FogRenderer(context.World, _spriteBatch)
        };

        var tilemapRenderer = new TilemapRenderer(_spriteBatch, context, cameraManager);
        var portraitRenderer = new PortraitRenderer(_spriteBatch);
        var itemIconRenderer = new ItemIconRenderer(_spriteBatch);
        var equipmentSlotRenderer = new EquipmentSlotRenderer(_spriteBatch, context, catalog);
        var renderPipeline = new RenderPipeline(_spriteBatch, cameraManager, tilemapRenderer, uiContext, groundRenderers, fringeRenderers);

        _renderPipeline = renderPipeline;
        _inputManager = inputManager;

        var intentSender = new IntentSender(_connection);
        var authSender = new AuthSender(_connection);
        var accountSender = new AccountSender(_connection, catalog);
        var contentSender = new ContentSender(_connection);

        // ── Intent registrations ──
        RegisterIntentTypes();

        var chat = new Chat(intentSender, uiContext);
        var gameInput = new GameInput(intentSender, chat, inputManager, uiContext);

        // UI
        var tooltipView = new TooltipView(uiContext, itemIconRenderer, catalog);
        var tradeViewModel = new TradeViewModel(context, intentSender);
        var partyViewModel = new PartyViewModel(context);
        var inventoryViewModel = new InventoryViewModel(context, intentSender, catalog);
        var hotbarViewModel = new HotbarViewModel(context, intentSender, catalog);
        var shopViewModel = new ShopViewModel(intentSender, catalog);
        var characterViewModel = new CharacterViewModel(context, intentSender, catalog);
        var statsViewModel = new StatsViewModel(context);
        _menuScreen = new MenuScreen(audioManager, uiContext, authSender, accountSender, portraitRenderer, context, catalog, _connection);
        _gameScreen = new GameScreen(uiContext, context, intentSender, _spriteBatch, itemIconRenderer, equipmentSlotRenderer,
            portraitRenderer, inputManager, audioManager, catalog, tooltipView, _menuScreen, chat, gameInput, tradeViewModel, partyViewModel, inventoryViewModel, hotbarViewModel, shopViewModel, characterViewModel, statsViewModel);

        // ── System initialization ──
        _scheduler = new SystemScheduler();
        _scheduler
            .AddSimulation(new FadeSystem(context.World))
            .AddSimulation(new FogSystem(context.World))
            .AddSimulation(new WeatherSimulationSystem(context))
            .AddSimulation(new WeatherSpawnSystem(context))
            .AddSimulation(new LightningSystem(context, audioManager))
            .AddSimulation(new MovementInputSystem(context, inputManager, intentSender))
            .AddSimulation(new ItemPickupSystem(context, inputManager, intentSender))
            .AddSimulation(new MovementSystem(context.World))
            .AddSimulation(new CameraSystem(context, cameraManager))
            .AddSimulation(new CharacterAnimationSystem(context.World))
            .AddSimulation(new AttackHitSystem(context))
            .AddSimulation(new AttackSystem(context, inputManager, intentSender, uiContext))
            .AddSimulation(new DamageDecaySystem(context.World));

        // ── Packet handlers ──
        var mapRepo = new MapRepository();
        PacketDispatcher.Register(new AuthHandler(catalog, uiContext, _menuScreen));
        PacketDispatcher.Register(new AccountHandler(context, _menuScreen, _gameScreen));
        PacketDispatcher.Register(new MapHandler(context, contentSender, audioManager, mapRepo));
        PacketDispatcher.Register(new KeyframeHandler(new Replication.SnapshotApplier(context.World, context, catalog)));
        PacketDispatcher.Register(new ChatHandler(chat));
        PacketDispatcher.Register(new PartyHandler(intentSender, _gameScreen, partyViewModel));
        PacketDispatcher.Register(new TradeHandler(intentSender, _gameScreen, tradeViewModel));
        PacketDispatcher.Register(new ContentHandler(catalog, _menuScreen));
        PacketDispatcher.Register(new ShopHandler(catalog, _gameScreen.ShopView));

        audioManager.LoadSounds();
        _menuScreen.Open();
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
                _spriteBatch.RenderWindow.DispatchEvents();

                var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                _uiContext.Update(deltaTime);

                _scheduler?.Update(deltaTime);

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
        _menuScreen.Open();
    }

    private static void RegisterComponentTypes()
    {
        ComponentTypes.RegisterDefault();
        ComponentTypeRegistry.Register<NetworkId>(18);
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
        _working = false;
        _hostRunner?.Stop();
        _hostRunner?.Dispose();
        _connection?.Disconnect();
    }
}
