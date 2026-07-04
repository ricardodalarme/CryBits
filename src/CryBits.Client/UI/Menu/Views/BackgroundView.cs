using CryBits.Client.Framework;
using CryBits.Client.Framework.Network;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu.Views;

internal class BackgroundView(UiContext uiContext, Connection connection, MenuScreen menuScreen) : ViewBase
{
    private Button OptionsButton => uiContext.Get<Button>("OptionsButton");

    public override void Bind()
    {
        OptionsButton.Events.OnClick += OnOptionsPressed;
    }

    public override void Unbind()
    {
        OptionsButton.Events.OnClick -= OnOptionsPressed;
    }

    private void OnOptionsPressed(Entity _)
    {
        connection.Disconnect();

        menuScreen.OptionsPanel.SoundsCheckbox.Checked = Options.Instance.Sounds;
        menuScreen.OptionsPanel.MusicsCheckbox.Checked = Options.Instance.Musics;

        menuScreen.CloseMenus();
        menuScreen.OptionsPanel.OptionsPanel.Visible = true;
    }
}
