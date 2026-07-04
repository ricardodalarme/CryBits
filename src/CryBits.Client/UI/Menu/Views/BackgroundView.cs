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
        menuScreen.ShowOptions();
    }
}
