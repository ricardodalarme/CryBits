using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network;

namespace CryBits.Client.UI.Menu.Views;

internal class BackgroundView(NetworkClient networkClient) : IView
{
    private static Button OptionsButton => Tools.Buttons["Options"];

    public void Bind()
    {
        OptionsButton.OnMouseUp += OnOptionsPressed;
    }

    public void Unbind()
    {
        OptionsButton.OnMouseUp -= OnOptionsPressed;
    }

    private void OnOptionsPressed()
    {
        networkClient.Disconnect();

        OptionsView.SoundsCheckBox.Checked = Options.Sounds;
        OptionsView.MusicsCheckBox.Checked = Options.Musics;

        MenuScreen.CloseMenus();
        OptionsView.OptionsPanel.Visible = true;
    }
}
