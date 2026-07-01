using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class TradeInvitationView(IguinaContext uiContext, IntentSender intentSender) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("TradeInvitation");
    private Button AcceptButton => uiContext.Get<Button>("TradeYes");
    private Button DeclineButton => uiContext.Get<Button>("TradeNo");
    private Label InviterNameLabel => uiContext.Get<Label>("TradeInvText");

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
        intentSender.Send(new TradeAcceptIntent(default));
        Panel.Visible = false;
    }

    private void OnDeclinePressed(Entity _)
    {
        intentSender.Send(new TradeDeclineIntent(default));
        Panel.Visible = false;
    }

    public void Show(string inviterName)
    {
        InviterNameLabel.Text = inviterName;
        Panel.Visible = true;
    }
}
