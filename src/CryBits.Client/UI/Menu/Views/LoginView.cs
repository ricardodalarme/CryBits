using CryBits.Client.Framework;
using CryBits.Client.Iguina;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network.Senders;
using Iguina;
using Iguina.Entities;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Client.UI.Menu.Views;

internal sealed class LoginView(UISystem ui)
{
    private Panel? _panel;
    private TextInput? _usernameInput;
    private TextInput? _passwordInput;
    private Checkbox? _saveUsernameCheckbox;

    public event Action? RegisterRequested;

    public void Build(Panel root, ScreenData config)
    {
        var (panel, reg) = MenuLoader.BuildScreen(ui, config, root);
        _panel = panel;
        _usernameInput = reg["Username"] as TextInput;
        _passwordInput = reg["Password"] as TextInput;
        _saveUsernameCheckbox = reg["SaveUsername"] as Checkbox;

        if (Options.Instance.SaveUsername)
            _usernameInput!.Value = Options.Instance.Username;
        _saveUsernameCheckbox!.Checked = Options.Instance.SaveUsername;
        _saveUsernameCheckbox.Events.OnChecked += OnSaveUsernameChanged;
        _saveUsernameCheckbox.Events.OnUnchecked += OnSaveUsernameChanged;

        ((Button)reg["LoginConfirm"]).Events.OnClick += OnConfirmClicked;
        ((Button)reg["LoginRegister"]).Events.OnClick += _ => RegisterRequested?.Invoke();
    }

    public void Destroy()
    {
        _panel?.RemoveSelf();
        _panel = null;
        _usernameInput = null;
        _passwordInput = null;
        _saveUsernameCheckbox = null;
    }

    private void OnSaveUsernameChanged(Ent _)
    {
        Options.Instance.SaveUsername = _saveUsernameCheckbox!.Checked;
        Options.Instance.Username = Options.Instance.SaveUsername ? _usernameInput?.Value ?? string.Empty : string.Empty;
        OptionsRepository.Write();
    }

    private void OnConfirmClicked(Ent _)
    {
        Options.Instance.Username = _usernameInput?.Value ?? string.Empty;
        OptionsRepository.Write();

        if (!Connection.Instance.TryConnect())
        {
            ui.MessageBoxes.ShowInfoMessageBox("Error", "The server is currently unavailable.", null, "OK");
            return;
        }
        AuthSender.Instance.Connect(_usernameInput?.Value ?? string.Empty, _passwordInput?.Value ?? string.Empty);
    }
}
