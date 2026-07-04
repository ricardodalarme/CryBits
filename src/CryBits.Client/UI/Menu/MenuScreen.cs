using CryBits.Client.Core;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Menu.Views;
using CryBits.Definitions.Catalog;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu;

internal class MenuScreen
{
    internal readonly BackgroundView BackgroundView;
    internal readonly LoginView LoginView;
    internal readonly RegisterView RegisterView;
    internal readonly OptionsView OptionsPanel;
    internal readonly SelectCharacterView SelectCharacterView;
    internal readonly CreateCharacterView CreateCharacterView;

    private readonly AudioManager _audio;
    private readonly UiContext _uiContext;

    internal MenuScreen(AudioManager audio, UiContext uiContext, AuthSender authSender, AccountSender accountSender,
        PortraitRenderer characterRenderer, GameContext context, DefinitionCatalog catalog, Connection connection)
    {
        _audio = audio;
        _uiContext = uiContext;
        BackgroundView = new(uiContext, connection, this);
        LoginView = new(uiContext, authSender, connection, this);
        RegisterView = new(uiContext, authSender, connection, this);
        OptionsPanel = new(uiContext, audio, context, connection, this);
        SelectCharacterView = new(uiContext, accountSender, characterRenderer);
        CreateCharacterView = new(uiContext, accountSender, characterRenderer, catalog, this);
    }

    private ViewBase[] Views =>
    [
        BackgroundView,
        LoginView,
        RegisterView,
        OptionsPanel,
        SelectCharacterView,
        CreateCharacterView
    ];

    public void Bind()
    {
        foreach (var view in Views)
            view.Bind();
    }

    public void Unbind()
    {
        foreach (var view in Views)
            view.Unbind();
    }

    public void Open()
    {
        _audio.StopAllSounds();
        if (Options.Instance.Musics) _audio.PlayMusic(Musics.Menu);

        _uiContext.LoadScreen("Menu");
        Bind();

        LoginView.SaveUsernameCheckbox.Checked = Options.Instance.SaveUsername;
        if (Options.Instance.SaveUsername) LoginView.UsernameTextBox.Value = Options.Instance.Username;

        CloseMenus();
        LoginView.LoginPanel.Visible = true;
        _uiContext.CurrentScreen = ScreenType.Menu;
    }

    public void CloseMenus()
    {
        foreach (var panelName in new[] { "Login", "Register", "Options", "SelectCharacter", "CreateCharacter" })
        {
            if (_uiContext.TryGet<Panel>(panelName, out var panel))
                panel.Visible = false;
        }
    }
}
