using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;

namespace CryBits.Client.UI.Game.Views;

internal class TradeInvitationView(IntentSender intentSender) : IView
{
    internal static Panel Panel => Tools.Panels["Trade_Invitation"];
    private static Button AcceptButton => Tools.Buttons["Trade_Yes"];
    private static Button DeclineButton => Tools.Buttons["Trade_No"];
    private static Label InviterNameLabel => Tools.Labels["Trade_Invitation_Text"];

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
        intentSender.Send(new TradeAcceptIntent(default));
        Panel.Visible = false;
    }

    private void OnDeclinePressed()
    {
        intentSender.Send(new TradeDeclineIntent(default));
        Panel.Visible = false;
    }

    public static void Show(string inviterName)
    {
        InviterNameLabel.SetArguments(inviterName);
        Panel.Visible = true;
    }
}
