using CryBits.Client.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using CryBits.Client.Framework.Network;
using CryBits.Client.Input;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering;
using CryBits.Client.Rendering.Camera;
using CryBits.Client.Rendering.Effects;
using CryBits.Client.Rendering.Entities;
using CryBits.Client.Rendering.Items;
using CryBits.Client.Rendering.Map;
using CryBits.Client.Rendering.UI;
using CryBits.Client.Replication;
using CryBits.Client.Systems;
using CryBits.Client.Systems.Character;
using CryBits.Client.Systems.Combat;
using CryBits.Client.Systems.Core;
using CryBits.Client.Systems.Map;
using CryBits.Client.Systems.Movement;
using CryBits.Client.Systems.Network;
using CryBits.Client.Systems.Player;
using CryBits.Client.UI;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Client.UI.Game.Views;
using CryBits.Definitions.Catalog;
using CryBits.Persistence.Repositories;
using CryBits.Simulation.Core;

namespace CryBits.Client.Core;

internal sealed class GameSession : IDisposable
{
    public World World { get; }
    public GameScreen Screen { get; }
    public SystemScheduler Scheduler { get; }
    public RenderPipeline RenderPipeline { get; }
    public CameraManager CameraManager { get; }

    public IntentSender IntentSender { get; }
    private readonly MapHandler _mapHandler;
    private readonly ReplicationHandler _replicationHandler;
    private readonly AckSender _ackSender;
    private readonly ChatHandler _chatHandler;
    private readonly PartyHandler _partyHandler;
    private readonly TradeHandler _tradeHandler;
    private readonly ShopHandler _shopHandler;

    private readonly Chat _chat;
    private readonly GameInput _gameInput;

    public GameSession(
        UiContext uiContext,
        SpriteBatch spriteBatch,
        InputManager inputManager,
        AudioManager audioManager,
        DefinitionCatalog catalog,
        Connection connection,
        Func<short> getFps,
        long localPlayerId)
    {
        var replication = new ReplicationState(localPlayerId);
        World = new(catalog, enableDirtyTracking: false);

        IntentSender = new IntentSender(connection);

        var tradeViewModel = new TradeViewModel(World, IntentSender, catalog);
        var partyViewModel = new PartyViewModel(World, replication.GetNetworkEntity);
        var inventoryViewModel = new InventoryViewModel(World, IntentSender, catalog);
        var hotbarViewModel = new HotbarViewModel(World, IntentSender, catalog);
        var shopViewModel = new ShopViewModel(IntentSender, catalog);
        var characterViewModel = new CharacterViewModel(World, IntentSender, catalog);
        var statsViewModel = new StatsViewModel(World);

        _chat = new Chat(IntentSender, uiContext);
        _gameInput = new GameInput(IntentSender, _chat, inputManager, uiContext);

        var portraitRenderer = new PortraitRenderer(spriteBatch);
        var itemIconRenderer = new ItemIconRenderer(spriteBatch);
        var equipmentSlotRenderer = new EquipmentSlotRenderer(spriteBatch);
        var tooltipView = new TooltipView(uiContext, itemIconRenderer);

        Screen = new GameScreen(
            this, uiContext, spriteBatch, itemIconRenderer, equipmentSlotRenderer,
            portraitRenderer, inputManager, audioManager, tooltipView, _chat, _gameInput,
            connection, getFps,
            statsViewModel, characterViewModel, inventoryViewModel, hotbarViewModel, tradeViewModel, partyViewModel,
            shopViewModel
        );

        CameraManager = new CameraManager(spriteBatch);
        RenderPipeline = new RenderPipeline(World, spriteBatch, CameraManager);

        var mapRepo = new MapRepository();
        var contentSender = new ContentSender(connection);

        var applier = new SnapshotApplier(World, replication, catalog);
        _ackSender = new AckSender(connection, replication);

        Scheduler = new SystemScheduler();
        Scheduler
            .AddSimulation(new FadeSystem())
            .AddSimulation(new FogSystem())
            .AddSimulation(new WeatherSimulationSystem(replication))
            .AddSimulation(new WeatherSpawnSystem(replication))
            .AddSimulation(new LightningSystem(replication, audioManager))
            .AddSimulation(new MovementInputSystem(replication, inputManager, IntentSender))
            .AddSimulation(new ItemPickupSystem(replication, inputManager, IntentSender))
            .AddSimulation(new MovementSystem())
            .AddSimulation(new CameraSystem(replication, CameraManager))
            .AddSimulation(new AckSystem(_ackSender))
            .AddSimulation(new CharacterAnimationSystem())
            .AddSimulation(new AttackHitSystem(replication))
            .AddSimulation(new AttackSystem(replication, inputManager, IntentSender, uiContext))
            .AddSimulation(new DamageDecaySystem());
        _mapHandler = new MapHandler(World, replication, contentSender, audioManager, mapRepo);
        _replicationHandler = new ReplicationHandler(applier);
        _chatHandler = new ChatHandler(_chat);
        _partyHandler = new PartyHandler(IntentSender, Screen, partyViewModel);
        _tradeHandler = new TradeHandler(IntentSender, Screen, tradeViewModel);
        _shopHandler = new ShopHandler(catalog, Screen.ShopView);

        PacketDispatcher.Register(_mapHandler);
        PacketDispatcher.Register(_replicationHandler);
        PacketDispatcher.Register(_chatHandler);
        PacketDispatcher.Register(_partyHandler);
        PacketDispatcher.Register(_tradeHandler);
        PacketDispatcher.Register(_shopHandler);
    }

    public void OpenScreen()
    {
        Screen.Open();
    }

    public void Dispose()
    {
        PacketDispatcher.Unregister(_mapHandler);
        PacketDispatcher.Unregister(_replicationHandler);
        PacketDispatcher.Unregister(_chatHandler);
        PacketDispatcher.Unregister(_partyHandler);
        PacketDispatcher.Unregister(_tradeHandler);
        PacketDispatcher.Unregister(_shopHandler);

        Screen.Unbind();
        World.MapDefs.Clear();
        World.Clear();
    }
}
