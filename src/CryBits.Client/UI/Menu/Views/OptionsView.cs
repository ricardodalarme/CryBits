using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Iguina;
using CryBits.Client.Worlds;
using Iguina;
using Iguina.Entities;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Client.UI.Menu.Views;

internal sealed class OptionsView(UISystem ui)
{
    private Panel? _panel;
    private Checkbox? _soundsCheckbox;
    private Checkbox? _musicsCheckbox;

    public event Action? LoginRequested;

    public void Build(Panel root, ScreenData config)
    {
        var (panel, reg) = MenuLoader.BuildScreen(ui, config, root);
        _panel = panel;
        _soundsCheckbox = reg["Sounds"] as Checkbox;
        _musicsCheckbox = reg["Musics"] as Checkbox;

        _soundsCheckbox!.Checked = Options.Instance.Sounds;
        _soundsCheckbox.Events.OnChecked += OnSoundsChanged;
        _soundsCheckbox.Events.OnUnchecked += OnSoundsChanged;
        _musicsCheckbox!.Checked = Options.Instance.Musics;
        _musicsCheckbox.Events.OnChecked += OnMusicsChanged;
        _musicsCheckbox.Events.OnUnchecked += OnMusicsChanged;

        ((Button)reg["OptionsBack"]).Events.OnClick += _ => LoginRequested?.Invoke();
    }

    public void Destroy()
    {
        _panel?.RemoveSelf();
        _panel = null;
        _soundsCheckbox = null;
        _musicsCheckbox = null;
    }

    private void OnSoundsChanged(Ent _)
    {
        Options.Instance.Sounds = _soundsCheckbox!.Checked;
        if (!Options.Instance.Sounds) AudioManager.Instance.StopAllSounds();
        OptionsRepository.Write();
    }

    private void OnMusicsChanged(Ent _)
    {
        Options.Instance.Musics = _musicsCheckbox!.Checked;
        OptionsRepository.Write();
        if (!Options.Instance.Musics)
            AudioManager.Instance.StopMusic();
        else if (GameState.CurrentScreen == ScreenType.Menu)
            AudioManager.Instance.PlayMusic(Musics.Menu);
        else if (GameState.CurrentScreen == ScreenType.Game)
            AudioManager.Instance.PlayMusic(GameContext.Instance.CurrentMap.Data.Music);
    }
}
