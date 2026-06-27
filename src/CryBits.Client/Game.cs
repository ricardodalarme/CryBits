using CryBits.Client.Components;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Graphics;
using CryBits.Client.Logic;
using CryBits.Client.Managers;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
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
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Client;

public sealed class Game : IDisposable
{
    private readonly bool _offline;
    private EmbeddedHostRunner? _hostRunner;
    private Connection? _connection;
    private RenderPipeline? _renderPipeline;
    private InputManager? _inputManager;
    private SystemScheduler? _scheduler;
    private bool _working = true;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private long _textboxTimer;

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
        ToolsRepository.Instance.Read();
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

        new MenuScreen().Bind();
        new GameScreen().Bind();
        Window.Instance.Bind();
        GameInput.Instance.Bind();

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

        var context = GameContext.Instance;
        var audioManager = AudioManager.Instance;
        var contentRepository = new ContentRepository();
        var cat = DefinitionCatalog.Instance;

        PacketDispatcher.Register(new AuthHandler(cat));
        PacketDispatcher.Register(new AccountHandler(audioManager, context));
        PacketDispatcher.Register(new MapHandler(context, ContentSender.Instance, audioManager, contentRepository));
        PacketDispatcher.Register(new KeyframeHandler(new Replication.SnapshotApplier(context.World, context)));
        PacketDispatcher.Register(new ChatHandler(Chat.Instance));
        PacketDispatcher.Register(new PartyHandler(IntentSender.Instance, context));
        PacketDispatcher.Register(new TradeHandler(IntentSender.Instance, context));
        PacketDispatcher.Register(new ContentHandler(cat));
        PacketDispatcher.Register(new ShopHandler(cat));
        AudioManager.Instance.LoadSounds();

        _scheduler = SystemScheduler.Instance;
        _scheduler.Initialize();
        _renderPipeline = RenderPipeline.Instance;
        _inputManager = InputManager.Instance;

        Window.Instance.OpenMenu();
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

                UpdateTextBox();

                var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                _scheduler?.Simulation.Update(deltaTime);

                var ctx = GameContext.Instance;
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
                    BarsView.Update();
                    CharacterView.Update();
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

    private void UpdateTextBox()
    {
        if (_textboxTimer < Environment.TickCount64)
        {
            _textboxTimer = Environment.TickCount64 + 500;
            TextBox.BlinkSignal = !TextBox.BlinkSignal;
            TextBox.Focus();
        }
    }

    private void OnDisconnected()
    {
        GameContext.Instance.Reset();
        Window.Instance.OpenMenu();
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
