using CryBits.Client.Framework.Network;
using CryBits.Client.Iguina;
using CryBits.Client.Network.Senders;
using Iguina;
using Iguina.Entities;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Client.UI.Menu.Views;

internal sealed class RegisterView(UISystem ui)
{
    private Panel? _panel;
    private TextInput? _registerUsernameInput;
    private TextInput? _registerPasswordInput;
    private TextInput? _registerConfirmInput;

    public event Action? LoginRequested;

    public void Build(Panel root, ScreenData config)
    {
        var (panel, reg) = MenuLoader.BuildScreen(ui, config, root);
        _panel = panel;
        _registerUsernameInput = reg["RegisterUsername"] as TextInput;
        _registerPasswordInput = reg["RegisterPassword"] as TextInput;
        _registerConfirmInput = reg["RegisterConfirm"] as TextInput;

        ((Button)reg["RegisterConfirmBtn"]).Events.OnClick += OnConfirmClicked;
        ((Button)reg["RegisterBackBtn"]).Events.OnClick += _ => LoginRequested?.Invoke();
    }

    public void Destroy()
    {
        _panel?.RemoveSelf();
        _panel = null;
        _registerUsernameInput = null;
        _registerPasswordInput = null;
        _registerConfirmInput = null;
    }

    private void OnConfirmClicked(Ent _)
    {
        var password = _registerPasswordInput?.Value ?? string.Empty;
        var confirm = _registerConfirmInput?.Value ?? string.Empty;

        if (password != confirm)
        {
            ui.MessageBoxes.ShowInfoMessageBox("Error", "The password don't match.", null, "OK");
            return;
        }
        if (!Connection.Instance.TryConnect())
        {
            ui.MessageBoxes.ShowInfoMessageBox("Error", "The server is currently unavailable.", null, "OK");
            return;
        }
        AuthSender.Instance.Register(_registerUsernameInput?.Value ?? string.Empty, password);
    }
}
