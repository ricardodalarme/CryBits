using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Menu.Views;
using CryBits.Client.Worlds;

namespace CryBits.Client.UI.Menu;

internal class MenuScreen(
    NetworkClient networkClient,
    AuthSender authSender,
    AudioManager audioManager,
    AccountSender accountSender,
    CharacterRenderer characterRenderer,
    GameContext context)
{
    private readonly BackgroundView BackgroundView = new(networkClient);
    private readonly LoginView LoginView = new(networkClient, authSender);
    private readonly RegisterView RegisterView = new(networkClient, authSender);
    private readonly OptionsView OptionsPanel = new(audioManager, networkClient, context);
    private readonly SelectCharacterView SelectCharacterView = new(accountSender, characterRenderer);
    private readonly CreateCharacterView CreateCharacterView = new(networkClient, accountSender, characterRenderer);

    private IView[] Views =>
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

    public static void CloseMenus()
    {
        LoginView.LoginPanel.Visible = false;
        RegisterView.RegisterPanel.Visible = false;
        OptionsView.OptionsPanel.Visible = false;
        SelectCharacterView.SelectCharacterPanel.Visible = false;
        CreateCharacterView.CreateCharacterPanel.Visible = false;
    }
}
