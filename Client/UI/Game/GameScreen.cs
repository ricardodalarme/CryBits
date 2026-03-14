using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using System.Collections.Generic;

namespace CryBits.Client.UI.Game;

internal class GameScreen
{
    private readonly CharacterView CharacterView = new(GameContext.Instance, PlayerSender.Instance, EquipmentRenderer.Instance, CharacterRenderer.Instance);
    private readonly ChatView ChatView = new();
    private readonly DraggableSlotView DraggableSlotView = new(ItemRenderer.Instance, InputManager.Instance, GameContext.Instance);
    private readonly DropItemView DropItemView = new(PlayerSender.Instance);
    private readonly HotbarView HotbarView = new(PlayerSender.Instance, ItemRenderer.Instance, GameContext.Instance);
    private readonly InformationView InformationView = new(ItemRenderer.Instance);
    private readonly InventoryView InventoryView = new(PlayerSender.Instance, ShopSender.Instance, ItemRenderer.Instance, GameContext.Instance);
    private readonly MenusView MenusView = new();
    private readonly OptionsView OptionsView = new(AudioManager.Instance, GameContext.Instance);
    private readonly PartyInvitationView PartyInvitationView = new(PartySender.Instance);
    private readonly ShopSellView ShopSellView = new(ShopSender.Instance);
    private readonly ShopView ShopView = new(ShopSender.Instance, ItemRenderer.Instance);
    private readonly TradeAmountView TradeAmountView = new(TradeSender.Instance);
    private readonly TradeInvitationView TradeInvitationView = new(TradeSender.Instance);
    private readonly TradeView TradeView = new(TradeSender.Instance, ItemRenderer.Instance, GameContext.Instance);

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

    public static short? HotbarChange;
    public static short? InventoryChange;

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
