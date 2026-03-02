using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Menu.Views;

namespace CryBits.Client.UI.Menu;

internal class MenuScreen
{
    private static BackgroundView BackgroundView = new();
    private static LoginView LoginView = new(NetworkClient.Instance, AuthSender.Instance);
    private static RegisterView RegisterView = new(NetworkClient.Instance, AuthSender.Instance);
    private static OptionsView OptionsPanel = new(AudioManager.Instance, NetworkClient.Instance);
    private static SelectCharacterView SelectCharacterView = new(AccountSender.Instance, CharacterRenderer.Instance);
    private static CreateCharacterView CreateCharacterView = new(NetworkClient.Instance, AccountSender.Instance, CharacterRenderer.Instance);

    private static IView[] Views =
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
