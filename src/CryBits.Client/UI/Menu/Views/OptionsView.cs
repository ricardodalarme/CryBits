using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Menu.Views;

internal class OptionsView(
    UiContext uiContext,
    AudioManager audioManager,
    MenuScreen menuScreen) : ViewBase
{
    private Panel OptionsPanel => uiContext.Get<Panel>("Options");
    private CheckButton SoundsCheckbox => uiContext.Get<CheckButton>("Sounds");
    private CheckButton MusicsCheckbox => uiContext.Get<CheckButton>("Musics");
    private Button BackButton => uiContext.Get<Button>("OptionsBack");

    public void Open()
    {
        OptionsPanel.Visible = true;
        SoundsCheckbox.IsChecked = Options.Instance.Sounds;
        MusicsCheckbox.IsChecked = Options.Instance.Musics;
        Bind();
    }

    public void Close()
    {
        OptionsPanel.Visible = false;
        Unbind();
    }

    public override void Bind()
    {
        SoundsCheckbox.Click += OnSoundsChanged;
        MusicsCheckbox.Click += OnMusicsChanged;
        BackButton.Click += OnBackPressed;
    }

    public override void Unbind()
    {
        SoundsCheckbox.Click -= OnSoundsChanged;
        MusicsCheckbox.Click -= OnMusicsChanged;
        BackButton.Click -= OnBackPressed;
    }

    private void OnSoundsChanged(object? sender, MyraEventArgs e)
    {
        Options.Instance.Sounds = SoundsCheckbox.IsChecked;
        if (!SoundsCheckbox.IsChecked)
            audioManager.StopAllSounds();
    }

    private void OnMusicsChanged(object? sender, MyraEventArgs e)
    {
        Options.Instance.Musics = MusicsCheckbox.IsChecked;
        if (!MusicsCheckbox.IsChecked)
            audioManager.StopMusic();
        else if (audioManager.CurrentMusic != null)
            audioManager.PlayMusic(audioManager.CurrentMusic);
    }

    private void OnBackPressed(object? sender, MyraEventArgs e) => menuScreen.ShowLogin();
}
