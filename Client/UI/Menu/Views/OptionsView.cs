using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network;
using CryBits.Client.Worlds;

namespace CryBits.Client.UI.Menu.Views;

internal class OptionsView(AudioManager audioManager, NetworkClient networkClient, GameContext context) : IView
{
    internal static Panel OptionsPanel => Tools.Panels["Options"];
    internal static CheckBox SoundsCheckBox => Tools.CheckBoxes["Sounds"];
    internal static CheckBox MusicsCheckBox => Tools.CheckBoxes["Musics"];
    private static Button BackButton => Tools.Buttons["Options_Back"];

    public void Bind()
    {
        SoundsCheckBox.OnMouseUp += OnSoundsChanged;
        MusicsCheckBox.OnMouseUp += OnMusicsChanged;
        BackButton.OnMouseUp += OnBackPressed;
    }

    public void Unbind()
    {
        SoundsCheckBox.OnMouseUp -= OnSoundsChanged;
        MusicsCheckBox.OnMouseUp -= OnMusicsChanged;
        BackButton.OnMouseUp -= OnBackPressed;
    }

    private void OnSoundsChanged()
    {
        Options.Sounds = !Options.Sounds;
        if (!Options.Sounds) audioManager.StopAllSounds();
        OptionsRepository.Write();
    }

    private void OnMusicsChanged()
    {
        Options.Musics = !Options.Musics;
        OptionsRepository.Write();

        if (!Options.Musics)
            audioManager.StopMusic();
        else if (Screen.Current == Screens.Menu)
            audioManager.PlayMusic(Musics.Menu);
        else if (Screen.Current == Screens.Game)
            audioManager.PlayMusic(context.CurrentMap.Data.Music);
    }

    private void OnBackPressed()
    {
        networkClient.Disconnect();

        MenuScreen.CloseMenus();
        LoginView.LoginPanel.Visible = true;
    }
}
