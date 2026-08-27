using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class TradeInvitationView(UiContext uiContext, IntentSender intentSender) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("TradeInvitation");
    private Button AcceptButton => uiContext.Get<Button>("TradeYes");
    private Button DeclineButton => uiContext.Get<Button>("TradeNo");
    private Label InviterNameLabel => uiContext.Get<Label>("TradeInvText");

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
        intentSender.Send(new TradeAcceptIntent(default));
        Panel.Visible = false;
    }

    private void OnDeclinePressed(object? sender, MyraEventArgs e)
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
