using CryBits.Client.Framework;
using CryBits.Client.Framework.Network;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu.Views;

internal class BackgroundView(IguinaContext uiContext) : ViewBase
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
        Connection.Instance.Disconnect();

        MenuScreen.Instance.OptionsPanel.SoundsCheckbox.Checked = Options.Instance.Sounds;
        MenuScreen.Instance.OptionsPanel.MusicsCheckbox.Checked = Options.Instance.Musics;

        MenuScreen.Instance.CloseMenus();
        MenuScreen.Instance.OptionsPanel.OptionsPanel.Visible = true;
    }
}
