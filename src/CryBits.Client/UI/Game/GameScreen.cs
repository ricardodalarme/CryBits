using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;

namespace CryBits.Client.UI.Game;

internal class GameScreen
{
    private readonly CharacterView CharacterView = new(GameContext.Instance, IntentSender.Instance, EquipmentRenderer.Instance, CharacterRenderer.Instance);
    private readonly ChatView ChatView = new();
    private readonly DraggableSlotView DraggableSlotView = new(ItemRenderer.Instance, InputManager.Instance, GameContext.Instance, DefinitionCatalog.Instance);
    private readonly DropItemView DropItemView = new(IntentSender.Instance);
    private readonly HotbarView HotbarView = new(IntentSender.Instance, ItemRenderer.Instance, GameContext.Instance, DefinitionCatalog.Instance);
    private readonly InformationView InformationView = new(ItemRenderer.Instance, DefinitionCatalog.Instance);
    private readonly InventoryView InventoryView = new(IntentSender.Instance, ItemRenderer.Instance, GameContext.Instance, DefinitionCatalog.Instance);
    private readonly MenusView MenusView = new();
    private readonly OptionsView OptionsView = new(AudioManager.Instance, GameContext.Instance);
    private readonly PartyInvitationView PartyInvitationView = new(IntentSender.Instance);
    private readonly ShopSellView ShopSellView = new(IntentSender.Instance);
    private readonly ShopView ShopView = new(IntentSender.Instance, ItemRenderer.Instance, DefinitionCatalog.Instance);
    private readonly TradeAmountView TradeAmountView = new(IntentSender.Instance);
    private readonly TradeInvitationView TradeInvitationView = new(IntentSender.Instance);
    private readonly TradeView TradeView = new(IntentSender.Instance, ItemRenderer.Instance, GameContext.Instance, DefinitionCatalog.Instance);

    private List<IView> Views =>
    [
        CharacterView,
        ChatView,
        DraggableSlotView,
        DropItemView,
        HotbarView,
        InformationView,
        InventoryView,
        MenusView,
        OptionsView,
        PartyInvitationView,
        ShopSellView,
        ShopView,
        TradeAmountView,
        TradeInvitationView,
        TradeView
    ];

    public static short HotbarChange;
    public static short InventoryChange;

    public void Bind()
    {
        foreach (var view in Views)
            view.Bind();
    }

    public void Unbind()
    {
        foreach (var view in Views)
            view.Unbind();
    }
}
