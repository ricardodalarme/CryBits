using System.Collections.Generic;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;

namespace CryBits.Client.UI.Game;

internal class GameScreen
{
    private static CharacterView CharacterView = new(PlayerSender.Instance, EquipmentRenderer.Instance, CharacterRenderer.Instance);
    private static ChatView ChatView = new();
    private static DropItemView DropItemView = new(PlayerSender.Instance);
    private static HotbarView HotbarView = new(PlayerSender.Instance, ItemRenderer.Instance);
    private static InventoryView InventoryView = new(PlayerSender.Instance, ShopSender.Instance, ItemRenderer.Instance);
    private static MenusView MenusView = new();
    private static OptionsView OptionsView = new(AudioManager.Instance);
    private static PartyInvitationView PartyInvitationView = new(PartySender.Instance);
    private static ShopSellView ShopSellView = new(ShopSender.Instance);
    private static ShopView ShopView = new(ShopSender.Instance, ItemRenderer.Instance);
    private static TradeAmountView TradeAmountView = new(TradeSender.Instance);
    private static TradeInvitationView TradeInvitationView = new(TradeSender.Instance);
    private static TradeView TradeView = new(TradeSender.Instance, ItemRenderer.Instance);

    private static List<IView> Views =
    [
        CharacterView,
        ChatView,
        DropItemView,
        HotbarView,
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
