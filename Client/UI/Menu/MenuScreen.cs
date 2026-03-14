using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Menu.Views;
using CryBits.Client.Worlds;

namespace CryBits.Client.UI.Menu;

internal class MenuScreen
{
    private readonly BackgroundView BackgroundView = new(NetworkClient.Instance);
    private readonly LoginView LoginView = new(NetworkClient.Instance, AuthSender.Instance);
    private readonly RegisterView RegisterView = new(NetworkClient.Instance, AuthSender.Instance);
    private readonly OptionsView OptionsPanel = new(AudioManager.Instance, NetworkClient.Instance, GameContext.Instance);
    private readonly SelectCharacterView SelectCharacterView = new(AccountSender.Instance, CharacterRenderer.Instance);
    private readonly CreateCharacterView CreateCharacterView = new(NetworkClient.Instance, AccountSender.Instance, CharacterRenderer.Instance);

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
