using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;

namespace CryBits.Client.UI.Game.Views;

internal class PartyInvitationView(IntentSender intentSender) : IView
{
    internal static Panel Panel => Tools.Panels["Party_Invitation"];
    private static Button AcceptButton => Tools.Buttons["Party_Yes"];
    private static Button DeclineButton => Tools.Buttons["Party_No"];
    private static Label InviterNameLabel => Tools.Labels["Party_Invitation_Text"];

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
        intentSender.Send(new PartyAcceptIntent(default));
        Panel.Visible = false;
    }

    private void OnDeclinePressed()
    {
        intentSender.Send(new PartyDeclineIntent(default));
        Panel.Visible = false;
    }

    public static void Show(string inviterName)
    {
        InviterNameLabel.SetArguments(inviterName);
        Panel.Visible = true;
    }
}
