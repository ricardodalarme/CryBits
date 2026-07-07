using CryBits.Client.Core;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Network;
using CryBits.Client.Input;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI;
using CryBits.Client.UI.Menu;
using CryBits.Definitions.Catalog;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Intents;
using SFML.Graphics;

namespace CryBits.Client;

public sealed class Game : IDisposable
{
    private readonly UiContext _uiContext;
    private readonly InputManager _inputManager;
    private readonly SpriteBatch _spriteBatch;
    private readonly Connection _connection;
    private readonly AudioManager _audioManager = new();
    private readonly DefinitionCatalog _catalog = new();
    private MenuScreen _menuScreen = null!;
    private GameSession? _activeSession;

    public Game(SpriteBatch spriteBatch, Connection connection)
    {
        _spriteBatch = spriteBatch;
        _connection = connection;

        var window = spriteBatch.RenderWindow;
        _uiContext = new UiContext(window.Size.X, window.Size.Y, window);
        _inputManager = new InputManager(_uiContext.UISystem, window);

        window.LostFocus += (_, _) => _inputManager.IsFocused = false;
        window.GainedFocus += (_, _) => _inputManager.IsFocused = true;

        RegisterIntentTypes();

        _menuScreen = new MenuScreen(_audioManager, _uiContext, new AuthSender(connection), new AccountSender(connection, _catalog), new PortraitRenderer(spriteBatch), _catalog, connection);

        PacketDispatcher.Register(new AuthHandler(_catalog, _uiContext, _menuScreen));
        PacketDispatcher.Register(new AccountHandler(this, _menuScreen));
        PacketDispatcher.Register(new ContentHandler(_catalog, _menuScreen));

        _audioManager.LoadSounds();
        _menuScreen.Open();
    }

    public void Update(float deltaTime)
    {
        _inputManager.BeginFrame();
        _uiContext.Update(deltaTime);
        _activeSession?.Scheduler.Update(deltaTime);
    }

    public void Render(RenderWindow window)
    {
        if (_activeSession != null)
        {
            _activeSession.RenderPipeline.Present();
        }
        else
        {
            window.Clear(Color.Black);
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
    }

    public void StartSession(long localPlayerId)
    {
        if (_activeSession != null) return;
        _activeSession = new GameSession(
            _uiContext, _spriteBatch, _inputManager, _audioManager, _catalog, _connection, localPlayerId
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

    public static short Fps { get; set; }

    public void Dispose()
    {
        _activeSession?.Dispose();
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
