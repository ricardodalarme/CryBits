using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class PartyInvitationView(UiContext uiContext, IntentSender intentSender) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("PartyInvitation");
    private Button AcceptButton => uiContext.Get<Button>("PartyYes");
    private Button DeclineButton => uiContext.Get<Button>("PartyNo");
    private Label InviterNameLabel => uiContext.Get<Label>("PartyText");

    public override void Bind()
    {
        AcceptButton.Click += OnAcceptPressed;
        DeclineButton.Click += OnDeclinePressed;
    }

    public override void Unbind()
    {
        AcceptButton.Click -= OnAcceptPressed;
        DeclineButton.Click -= OnDeclinePressed;
    }

    private void OnAcceptPressed(object? sender, MyraEventArgs e)
    {
        intentSender.Send(new PartyAcceptIntent(default));
        Panel.Visible = false;
    }

    private void OnDeclinePressed(object? sender, MyraEventArgs e)
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
