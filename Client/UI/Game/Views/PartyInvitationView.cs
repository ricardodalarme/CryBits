using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;

namespace CryBits.Client.UI.Game.Views;

internal class PartyInvitationView(PartySender partySender) : IView
{
    internal static Panel Panel => Tools.Panels["Party_Invitation"];
    private static Button AcceptButton => Tools.Buttons["Party_Yes"];
    private static Button DeclineButton => Tools.Buttons["Party_No"];

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
        partySender.PartyAccept();
        Panel.Visible = false;
    }

    private void OnDeclinePressed()
    {
        partySender.PartyDecline();
        Panel.Visible = false;
    }
}
