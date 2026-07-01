using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class PartyInvitationView(IguinaContext uiContext, IntentSender intentSender) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("PartyInvitation");
    private Button AcceptButton => uiContext.Get<Button>("PartyYes");
    private Button DeclineButton => uiContext.Get<Button>("PartyNo");
    private Label InviterNameLabel => uiContext.Get<Label>("PartyText");

    public override void Bind()
    {
        AcceptButton.Events.OnClick += OnAcceptPressed;
        DeclineButton.Events.OnClick += OnDeclinePressed;
    }

    public override void Unbind()
    {
        AcceptButton.Events.OnClick -= OnAcceptPressed;
        DeclineButton.Events.OnClick -= OnDeclinePressed;
    }

    private void OnAcceptPressed(Entity _)
    {
        intentSender.Send(new PartyAcceptIntent(default));
        Panel.Visible = false;
    }

    private void OnDeclinePressed(Entity _)
    {
        intentSender.Send(new PartyDeclineIntent(default));
        Panel.Visible = false;
    }

    public void Show(string inviterName)
    {
        InviterNameLabel.Text = inviterName;
        Panel.Visible = true;
    }
}
