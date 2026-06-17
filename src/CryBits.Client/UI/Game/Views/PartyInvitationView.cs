using CryBits.Client.Network.Senders;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using IguinaRect = Iguina.Defs.Rectangle;

namespace CryBits.Client.UI.Game.Views;

internal sealed class PartyInvitationView
{
    private static PartyInvitationView? _instance;
    private readonly PartySender _partySender;
    private readonly UISystem _ui;

    private Panel? _panel;
    private Button? _acceptButton;
    private Button? _declineButton;
    private Label? _inviterNameLabel;

    public PartyInvitationView(UISystem ui)
    {
        _instance = this;
        _ui = ui;
        _partySender = PartySender.Instance;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Panel root)
    {
        _panel = new Panel(_ui);
        _panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/17.png",
            SourceRect = new IguinaRect { Width = 200, Height = 100 }
        };
        _panel.Size.SetPixels(200, 100);
        _panel.Anchor = Anchor.TopLeft;
        _panel.Offset.SetPixels(305, 249);
        _panel.Visible = false;
        root.AddChild(_panel);

        _inviterNameLabel = new Label(_ui);
        _inviterNameLabel.Anchor = Anchor.TopLeft;
        _inviterNameLabel.Offset.SetPixels(14, 10);
        _panel.AddChild(_inviterNameLabel);

        _acceptButton = new Button(_ui);
        _acceptButton.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/25.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _acceptButton.Size.SetPixels(32, 32);
        _acceptButton.Anchor = Anchor.TopLeft;
        _acceptButton.Offset.SetPixels(14, 45);
        _acceptButton.Paragraph.Text = string.Empty;
        _acceptButton.Events.OnClick += _ => OnAcceptPressed();
        _panel.AddChild(_acceptButton);

        _declineButton = new Button(_ui);
        _declineButton.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/26.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _declineButton.Size.SetPixels(32, 32);
        _declineButton.Anchor = Anchor.TopLeft;
        _declineButton.Offset.SetPixels(97, 45);
        _declineButton.Paragraph.Text = string.Empty;
        _declineButton.Events.OnClick += _ => OnDeclinePressed();
        _panel.AddChild(_declineButton);
    }

    private void OnAcceptPressed()
    {
        _partySender.PartyAccept();
        _panel!.Visible = false;
    }

    private void OnDeclinePressed()
    {
        _partySender.PartyDecline();
        _panel!.Visible = false;
    }

    public static void Show(string inviterName)
    {
        if (_instance == null) return;
        _instance._inviterNameLabel!.Text = $"{inviterName} has invited you to a party. Would you like to join?";
        _instance._panel!.Visible = true;
    }
}
