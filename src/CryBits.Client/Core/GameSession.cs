using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Network;
using CryBits.Client.Input;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering;
using CryBits.Client.Rendering.Camera;
using CryBits.Client.Rendering.Effects;
using CryBits.Client.Rendering.Entities;
using CryBits.Client.Rendering.Map;
using CryBits.Client.Rendering.Items;
using CryBits.Client.Rendering.UI;
using CryBits.Client.Systems;
using CryBits.Client.Systems.Character;
using CryBits.Client.UI;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.UI.Menu;
using CryBits.Definitions.Catalog;
using CryBits.Persistence.Repositories;
using CryBits.Simulation.Components;
using CryBits.Client.Systems.Core;
using CryBits.Client.Systems.Movement;
using CryBits.Client.Systems.Player;
using CryBits.Client.Systems.Combat;
using CryBits.Client.Systems.Map;

namespace CryBits.Client.Core;

internal sealed class GameSession : IDisposable
{
    public GameContext Context { get; }
    public UiContext UiContext { get; }
    public GameScreen Screen { get; }
    public SystemScheduler Scheduler { get; }
    public RenderPipeline RenderPipeline { get; }
    public CameraManager CameraManager { get; }

    public IntentSender IntentSender { get; }
    private readonly MapHandler _mapHandler;
    private readonly KeyframeHandler _keyframeHandler;
    private readonly ChatHandler _chatHandler;
    private readonly PartyHandler _partyHandler;
    private readonly TradeHandler _tradeHandler;
    private readonly ShopHandler _shopHandler;

    public StatsViewModel StatsViewModel { get; }
    public CharacterViewModel CharacterViewModel { get; }
    public InventoryViewModel InventoryViewModel { get; }
    public HotbarViewModel HotbarViewModel { get; }
    public TradeViewModel TradeViewModel { get; }
    public PartyViewModel PartyViewModel { get; }
    public ShopViewModel ShopViewModel { get; }

    private readonly Chat _chat;
    private readonly GameInput _gameInput;

    public GameSession(
        UiContext uiContext,
        SpriteBatch spriteBatch,
        InputManager inputManager,
        AudioManager audioManager,
        DefinitionCatalog catalog,
        Connection connection,
        MenuScreen menuScreen,
        long localPlayerId)
    {
        UiContext = uiContext;
        Context = new GameContext(catalog, localPlayerId);

        IntentSender = new IntentSender(connection);

        TradeViewModel = new TradeViewModel(Context, IntentSender, catalog);
        PartyViewModel = new PartyViewModel(Context);
        InventoryViewModel = new InventoryViewModel(Context, IntentSender, catalog);
        HotbarViewModel = new HotbarViewModel(Context, IntentSender, catalog);
        ShopViewModel = new ShopViewModel(IntentSender, catalog);
        CharacterViewModel = new CharacterViewModel(Context, IntentSender, catalog);
        StatsViewModel = new StatsViewModel(Context);

        _chat = new Chat(IntentSender, uiContext);
        _gameInput = new GameInput(IntentSender, _chat, inputManager, uiContext);

        var portraitRenderer = new PortraitRenderer(spriteBatch);
        var itemIconRenderer = new ItemIconRenderer(spriteBatch);
        var equipmentSlotRenderer = new EquipmentSlotRenderer(spriteBatch, Context, catalog);
        var tooltipView = new TooltipView(uiContext, itemIconRenderer, catalog);

        Screen = new GameScreen(
            this, uiContext, spriteBatch, itemIconRenderer, equipmentSlotRenderer,
            portraitRenderer, inputManager, audioManager, tooltipView, menuScreen, _chat, _gameInput
        );

        CameraManager = new CameraManager(spriteBatch.RenderWindow);
        var groundRenderers = new List<IRenderer>
        {
            new GroundSpriteRenderer(Context.World, spriteBatch),
            new EntitySpriteRenderer(Context.World, spriteBatch)
        };
        var fringeRenderers = new List<IRenderer>
        {
            new HealthBarRenderer(Context.World, spriteBatch),
            new WeatherParticleRenderer(Context.World, spriteBatch),
            new FogRenderer(Context.World, spriteBatch)
        };
        var tilemapRenderer = new TilemapRenderer(spriteBatch, Context, CameraManager);
        RenderPipeline = new RenderPipeline(spriteBatch, CameraManager, tilemapRenderer, uiContext, groundRenderers, fringeRenderers);

        Scheduler = new SystemScheduler();
        Scheduler
            .AddSimulation(new FadeSystem(Context.World))
            .AddSimulation(new FogSystem(Context.World))
            .AddSimulation(new WeatherSimulationSystem(Context))
            .AddSimulation(new WeatherSpawnSystem(Context))
            .AddSimulation(new LightningSystem(Context, audioManager))
            .AddSimulation(new MovementInputSystem(Context, inputManager, IntentSender))
            .AddSimulation(new ItemPickupSystem(Context, inputManager, IntentSender))
            .AddSimulation(new MovementSystem(Context.World))
            .AddSimulation(new CameraSystem(Context, CameraManager))
            .AddSimulation(new CharacterAnimationSystem(Context.World))
            .AddSimulation(new AttackHitSystem(Context))
            .AddSimulation(new AttackSystem(Context, inputManager, IntentSender, uiContext))
            .AddSimulation(new DamageDecaySystem(Context.World));

        var mapRepo = new MapRepository();
        var contentSender = new ContentSender(connection);

        _mapHandler = new MapHandler(Context, contentSender, audioManager, mapRepo);
        _keyframeHandler = new KeyframeHandler(new Replication.SnapshotApplier(Context.World, Context, catalog));
        _chatHandler = new ChatHandler(_chat);
        _partyHandler = new PartyHandler(IntentSender, Screen, PartyViewModel);
        _tradeHandler = new TradeHandler(IntentSender, Screen, TradeViewModel);
        _shopHandler = new ShopHandler(catalog, Screen.ShopView);

        PacketDispatcher.Register(_mapHandler);
        PacketDispatcher.Register(_keyframeHandler);
        PacketDispatcher.Register(_chatHandler);
        PacketDispatcher.Register(_partyHandler);
        PacketDispatcher.Register(_tradeHandler);
        PacketDispatcher.Register(_shopHandler);
    }

    public void OpenScreen()
    {
        Screen.Open();
    }

    public void Update(float dt)
    {
        UiContext.Update(dt);
        Scheduler.Update(dt);

        if (Context.LocalPlayerEntity is { } playerEntityId)
        {
            var level = Context.World.Get<LevelComponent>(playerEntityId);
            var attrs = Context.World.Get<AttributesComponent>(playerEntityId);
            if (level != null && attrs != null)
            {
                short total = 0;
                foreach (var v in attrs.Values) total += v;
                if (level.TotalAttributes != total)
                    Context.World.Set(playerEntityId, level with { TotalAttributes = total });
            }
            Screen.CharacterView.Update();
        }
    }

    public void Dispose()
    {
        PacketDispatcher.Unregister(_mapHandler);
        PacketDispatcher.Unregister(_keyframeHandler);
        PacketDispatcher.Unregister(_chatHandler);
        PacketDispatcher.Unregister(_partyHandler);
        PacketDispatcher.Unregister(_tradeHandler);
        PacketDispatcher.Unregister(_shopHandler);

        Screen.Unbind();
        Context.World.Clear();
    }
}
