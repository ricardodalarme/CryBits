using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Menu.Views;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu;

internal class MenuScreen(AudioManager audioManager)
{
    public static MenuScreen Instance { get; } = new(AudioManager.Instance);

    internal readonly BackgroundView BackgroundView = new(IguinaContext.Instance);
    internal readonly LoginView LoginView = new(IguinaContext.Instance, AuthSender.Instance);
    internal readonly RegisterView RegisterView = new(IguinaContext.Instance, AuthSender.Instance);
    internal readonly OptionsView OptionsPanel = new(IguinaContext.Instance, AudioManager.Instance, GameContext.Instance);
    internal readonly SelectCharacterView SelectCharacterView = new(IguinaContext.Instance, AccountSender.Instance, CharacterRenderer.Instance);
    internal readonly CreateCharacterView CreateCharacterView = new(IguinaContext.Instance, AccountSender.Instance, CharacterRenderer.Instance, DefinitionCatalog.Instance);

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
        audioManager.StopAllSounds();
        if (Options.Instance.Musics) audioManager.PlayMusic(Musics.Menu);

        IguinaContext.Instance.LoadScreen("Menu");
        Instance.Bind();

        LoginView.SaveUsernameCheckbox.Checked = Options.Instance.SaveUsername;
        if (Options.Instance.SaveUsername) LoginView.UsernameTextBox.Value = Options.Instance.Username;

        CloseMenus();
        LoginView.LoginPanel.Visible = true;
        IguinaContext.Instance.CurrentScreen = ScreenType.Menu;
    }

    public void CloseMenus()
    {
        foreach (var panelName in new[] { "Login", "Register", "Options", "SelectCharacter", "CreateCharacter" })
        {
            if (IguinaContext.Instance.TryGet<Panel>(panelName, out var panel))
                panel.Visible = false;
        }
    }
}
