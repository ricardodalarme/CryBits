using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;

namespace CryBits.Client.UI.Game.Views;

internal class TradeInvitationView(TradeSender tradeSender) : IView
{
    internal static Panel Panel => Tools.Panels["Trade_Invitation"];
    private static Button AcceptButton => Tools.Buttons["Trade_Yes"];
    private static Button DeclineButton => Tools.Buttons["Trade_No"];

    public static string InviterName;

    public void Bind()
    {
        AcceptButton.OnMouseUp += OnAcceptPressed;
        DeclineButton.OnMouseUp += OnDeclinePressed;
    }

    public void Unbind()
    {
        AcceptButton.OnMouseUp -= OnAcceptPressed;
        DeclineButton.OnMouseUp -= OnDeclinePressed;
    }

    private void OnAcceptPressed()
    {
        tradeSender.TradeAccept();
        Panel.Visible = false;
    }

    private void OnDeclinePressed()
    {
        tradeSender.TradeDecline();
        Panel.Visible = false;
    }
}
