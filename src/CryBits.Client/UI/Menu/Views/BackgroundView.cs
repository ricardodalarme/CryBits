using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Menu.Views;

internal class BackgroundView(UiContext uiContext, MenuScreen menuScreen) : ViewBase
{
    private Button OptionsButton => uiContext.Get<Button>("OptionsButton");

    public override void Bind()
    {
        OptionsButton.Click += OnOptionsPressed;
    }

    public override void Unbind()
    {
        OptionsButton.Click -= OnOptionsPressed;
    }

    private void OnOptionsPressed(object? sender, Myra.Events.MyraEventArgs e) => menuScreen.ShowOptions();
}
