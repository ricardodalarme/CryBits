using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Entities.Shop;
using SFML.Window;
using static CryBits.Client.Utils.UIUtils;

namespace CryBits.Client.UI.Game.Views;

internal class ShopView(ShopSender shopSender) : IView
{
    internal static Panel Panel => Tools.Panels["Shop"];
    private static Button CloseButton => Tools.Buttons["Shop_Close"];

    public static short CurrentSlot => GetSlotAtMousePosition(Panel, 7, 50, 4, 7);
    public static Shop OpenedShop;

    public void Bind()
    {
        Panel.OnMouseDoubleClick += OnPanelMouseDoubleClick;
        CloseButton.OnMouseUp += OnClosePressed;
    }

    public void Unbind()
    {
        Panel.OnMouseDoubleClick -= OnPanelMouseDoubleClick;
        CloseButton.OnMouseUp -= OnClosePressed;
    }

    private void OnPanelMouseDoubleClick(MouseButtonEventArgs e)
    {
        var slot = CurrentSlot;
        if (slot < 0) return;
        if (OpenedShop == null) return;

        // Purchase shop item.
        shopSender.ShopBuy((byte)slot);
    }

    private void OnClosePressed()
    {
        Panel.Visible = false;
        shopSender.ShopClose();
    }
}
