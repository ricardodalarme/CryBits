using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Worlds;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu.Views;

internal class OptionsView(IguinaContext uiContext, AudioManager audioManager, GameContext context) : ViewBase
{
    internal Panel OptionsPanel => uiContext.Get<Panel>("Options");
    internal Checkbox SoundsCheckbox => uiContext.Get<Checkbox>("Sounds");
    internal Checkbox MusicsCheckbox => uiContext.Get<Checkbox>("Musics");
    private Button BackButton => uiContext.Get<Button>("OptionsBack");

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
        else if (context.CurrentMap?.Data?.Music != null)
            audioManager.PlayMusic(context.CurrentMap.Data.Music);
        else
            audioManager.PlayMusic(Musics.Menu);
    }

    private void OnBackPressed(Entity _)
    {
        Connection.Instance.Disconnect();

        MenuScreen.Instance.CloseMenus();
        MenuScreen.Instance.LoginView.LoginPanel.Visible = true;
    }
}
