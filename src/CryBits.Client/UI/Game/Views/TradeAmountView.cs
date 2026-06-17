using CryBits.Client.Network.Senders;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using IguinaRect = Iguina.Defs.Rectangle;

namespace CryBits.Client.UI.Game.Views;

internal sealed class TradeAmountView
{
    private static TradeAmountView? _instance;
    private readonly TradeSender _tradeSender;
    private readonly UISystem _ui;

    private Panel? _panel;
    private TextInput? _amountInput;
    private Button? _confirmButton;
    private Button? _cancelButton;

    public TradeAmountView(UISystem ui)
    {
        _instance = this;
        _ui = ui;
        _tradeSender = TradeSender.Instance;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Panel root)
    {
        _panel = new Panel(_ui);
        _panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/20.png",
            SourceRect = new IguinaRect { Width = 190, Height = 100 }
        };
        _panel.Size.SetPixels(190, 100);
        _panel.Anchor = Anchor.TopLeft;
        _panel.Offset.SetPixels(305, 249);
        _panel.Visible = false;
        root.AddChild(_panel);

        _amountInput = new TextInput(_ui);
        _amountInput.Anchor = Anchor.TopLeft;
        _amountInput.Offset.SetPixels(14, 10);
        _amountInput.Size.SetPixels(162, 20);
        _amountInput.PlaceholderText = "Amount...";
        _panel.AddChild(_amountInput);

        _confirmButton = new Button(_ui);
        _confirmButton.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/30.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _confirmButton.Size.SetPixels(32, 32);
        _confirmButton.Anchor = Anchor.TopLeft;
        _confirmButton.Offset.SetPixels(14, 40);
        _confirmButton.Paragraph.Text = string.Empty;
        _confirmButton.Events.OnClick += _ => OnConfirmPressed();
        _panel.AddChild(_confirmButton);

        _cancelButton = new Button(_ui);
        _cancelButton.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/24.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _cancelButton.Size.SetPixels(32, 32);
        _cancelButton.Anchor = Anchor.TopLeft;
        _cancelButton.Offset.SetPixels(97, 40);
        _cancelButton.Paragraph.Text = string.Empty;
        _cancelButton.Events.OnClick += _ => OnCancelPressed();
        _panel.AddChild(_cancelButton);
    }

    public static bool PanelVisible
    {
        get => _instance?._panel?.Visible ?? false;
        set { if (_instance?._panel != null) _instance._panel.Visible = value; }
    }

    public static string AmountText
    {
        get => _instance?._amountInput?.Value ?? string.Empty;
        set { if (_instance?._amountInput != null) _instance._amountInput.Value = value; }
    }

    private void OnConfirmPressed()
    {
        if (!short.TryParse(_amountInput!.Value, out var amount) || amount <= 0)
        {
            Alert.Show("Enter a valid value!");
            return;
        }

        _tradeSender.TradeOffer(TradeView.OwnSlot, TradeView.InventorySlot, amount);
        _panel!.Visible = false;
    }

    private void OnCancelPressed()
    {
        _panel!.Visible = false;
    }
}
