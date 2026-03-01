using System;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Logic;
using CryBits.Client.Worlds;

namespace CryBits.Client.UI.Events;

internal static class CheckBoxEvents
{
    public static void Bind()
    {
        CheckBoxes.Sounds.OnMouseUp += ToggleSound;
        CheckBoxes.Musics.OnMouseUp += ToggleMusic;
        CheckBoxes.ConnectSaveUsername.OnMouseUp += SaveUsername;
        CheckBoxes.GenderMale.OnMouseUp += GenreName;
        CheckBoxes.GenderFemale.OnMouseUp += GenreFemale;
        CheckBoxes.OptionsSounds.OnMouseUp += ToggleSound;
        CheckBoxes.OptionsMusics.OnMouseUp += ToggleMusic;
        CheckBoxes.OptionsChat.OnMouseUp += Chat;
        CheckBoxes.OptionsFps.OnMouseUp += Fps;
        CheckBoxes.OptionsLatency.OnMouseUp += Latency;
        CheckBoxes.OptionsParty.OnMouseUp += Party;
        CheckBoxes.OptionsTrade.OnMouseUp += Trade;
    }

    private static void ToggleSound()
    {
        Options.Sounds = !Options.Sounds;
        if (!Options.Sounds) AudioManager.Instance.StopAllSounds();
        OptionsRepository.Write();
    }

    private static void ToggleMusic()
    {
        Options.Musics = !Options.Musics;
        OptionsRepository.Write();

        if (!Options.Musics)
            AudioManager.Instance.StopMusic();
        else if (Screen.Current == Screens.Menu)
            AudioManager.Instance.PlayMusic(Musics.Menu);
        else if (Screen.Current == Screens.Game)
            AudioManager.Instance.PlayMusic(GameContext.Instance.CurrentMap.Data.Music);
    }

    private static void SaveUsername()
    {
        Options.SaveUsername = CheckBoxes.ConnectSaveUsername.Checked;
        OptionsRepository.Write();
    }

    private static void GenreName()
    {
        CheckBoxes.GenderFemale.Checked = !CheckBoxes.GenderMale.Checked;
        PanelsEvents.CreateCharacterTex = 0;
    }

    private static void GenreFemale()
    {
        CheckBoxes.GenderMale.Checked = !CheckBoxes.GenderFemale.Checked;
        PanelsEvents.CreateCharacterTex = 0;
    }

    private static void Chat()
    {
        Options.Chat = CheckBoxes.OptionsChat.Checked;
        OptionsRepository.Write();
        if (Options.Chat) GameLoop.ChatTimer = Environment.TickCount + UI.Chat.SleepTimer;
    }

    private static void Fps()
    {
        Options.Fps = CheckBoxes.OptionsFps.Checked;
        OptionsRepository.Write();
    }

    private static void Latency()
    {
        Options.Latency = CheckBoxes.OptionsLatency.Checked;
        OptionsRepository.Write();
    }

    private static void Party()
    {
        Options.Party = CheckBoxes.OptionsParty.Checked;
        OptionsRepository.Write();
    }

    private static void Trade()
    {
        Options.Trade = CheckBoxes.OptionsTrade.Checked;
        OptionsRepository.Write();
    }
}
