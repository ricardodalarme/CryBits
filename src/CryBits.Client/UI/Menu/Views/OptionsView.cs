using CryBits.Client.Framework;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Persistence.Repositories;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu.Views;

internal class OptionsView(UiContext uiContext, AudioManager audioManager, Connection connection, MenuScreen menuScreen) : ViewBase
{
    private Panel OptionsPanel => uiContext.Get<Panel>("Options");
    private Checkbox SoundsCheckbox => uiContext.Get<Checkbox>("Sounds");
    private Checkbox MusicsCheckbox => uiContext.Get<Checkbox>("Musics");
    private Button BackButton => uiContext.Get<Button>("OptionsBack");

    public void Open()
    {
        SoundsCheckbox.Checked = Options.Instance.Sounds;
        MusicsCheckbox.Checked = Options.Instance.Musics;
        OptionsPanel.Visible = true;
        Bind();
    }

    public void Close()
    {
        OptionsPanel.Visible = false;
        Unbind();
    }

    public override void Bind()
    {
        SoundsCheckbox.Events.OnValueChanged += OnSoundsChanged;
        MusicsCheckbox.Events.OnValueChanged += OnMusicsChanged;
        BackButton.Events.OnClick += OnBackPressed;
    }

    public override void Unbind()
    {
        SoundsCheckbox.Events.OnValueChanged -= OnSoundsChanged;
        MusicsCheckbox.Events.OnValueChanged -= OnMusicsChanged;
        BackButton.Events.OnClick -= OnBackPressed;
    }

    private void OnSoundsChanged(Entity _)
    {
        Options.Instance.Sounds = SoundsCheckbox.Checked;
        if (!Options.Instance.Sounds) audioManager.StopAllSounds();
        OptionsRepository.Write();
    }

    private void OnMusicsChanged(Entity _)
    {
        Options.Instance.Musics = MusicsCheckbox.Checked;
        OptionsRepository.Write();

        if (!Options.Instance.Musics)
            audioManager.StopMusic();
        else
            audioManager.PlayMusic(Musics.Menu);
    }

    private void OnBackPressed(Entity _)
    {
        connection.Disconnect();
        menuScreen.ShowLogin();
    }
}
