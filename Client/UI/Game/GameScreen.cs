using System.Collections.Generic;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;

namespace CryBits.Client.UI.Game;

internal class GameScreen(
    PlayerSender playerSender,
    EquipmentRenderer equipmentRenderer,
    CharacterRenderer characterRenderer,
    ItemRenderer itemRenderer,
    ShopSender shopSender,
    TradeSender tradeSender,
    PartySender partySender,
    AudioManager audioManager,
    InputManager inputManager,
    GameContext context)
{
    private readonly CharacterView CharacterView = new(playerSender, equipmentRenderer, characterRenderer, context);
    private readonly ChatView ChatView = new();
    private readonly DraggableSlotView DraggableSlotView = new(itemRenderer, inputManager, context);
    private readonly DropItemView DropItemView = new(playerSender);
    private readonly HotbarView HotbarView = new(playerSender, itemRenderer, context);
    private readonly InformationView InformationView = new(itemRenderer);
    private readonly InventoryView InventoryView = new(playerSender, shopSender, itemRenderer, context);
    private readonly MenusView MenusView = new();
    private readonly OptionsView OptionsView = new(audioManager, context);
    private readonly PartyInvitationView PartyInvitationView = new(partySender);
    private readonly ShopSellView ShopSellView = new(shopSender);
    private readonly ShopView ShopView = new(shopSender, itemRenderer);
    private readonly TradeAmountView TradeAmountView = new(tradeSender);
    private readonly TradeInvitationView TradeInvitationView = new(tradeSender);
    private readonly TradeView TradeView = new(tradeSender, itemRenderer, context);

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
