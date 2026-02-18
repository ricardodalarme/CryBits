using System;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Library;
using CryBits.Client.Framework.Library.Repositories;
using CryBits.Client.Logic;

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
        // Salva os dados
        Options.Sounds = !Options.Sounds;
        if (!Options.Sounds) Sound.StopAll();
        OptionsRepository.Write();
    }

    private static void ToggleMusic()
    {
        // Salva os dados
        Options.Musics = !Options.Musics;
        OptionsRepository.Write();

        // Para ou reproduz a música dependendo do estado do marcador
        if (!Options.Musics)
            Music.Stop();
        else if (Screen.Current == Screens.Menu)
            Music.Play(Musics.Menu);
        else if (Screen.Current == Screens.Game)
            Music.Play(TempMap.Current.Data.Music);
    }

    private static void SaveUsername()
    {
        // Salva os dados
        Options.SaveUsername = CheckBoxes.ConnectSaveUsername.Checked;
        OptionsRepository.Write();
    }

    private static void GenreName()
    {
        // Altera o estado do marcador de outro gênero
        CheckBoxes.GenderFemale.Checked = !CheckBoxes.GenderMale.Checked;
        PanelsEvents.CreateCharacterTex = 0;
    }

    private static void GenreFemale()
    {
        // Altera o estado do marcador de outro gênero
        CheckBoxes.GenderMale.Checked = !CheckBoxes.GenderFemale.Checked;
        PanelsEvents.CreateCharacterTex = 0;
    }

    private static void Chat()
    {
        // Salva os dado
        Options.Chat = CheckBoxes.OptionsChat.Checked;
        OptionsRepository.Write();
        if (Options.Chat) Loop.ChatTimer = Environment.TickCount + UI.Chat.SleepTimer;
    }

    private static void Fps()
    {
        // Salva os dado
        Options.Fps = CheckBoxes.OptionsFps.Checked;
        OptionsRepository.Write();
    }

    private static void Latency()
    {
        // Desabilita a prévia do chat
        Options.Latency = CheckBoxes.OptionsLatency.Checked;
        OptionsRepository.Write();
    }

    private static void Party()
    {
        // Salva os dado
        Options.Party = CheckBoxes.OptionsParty.Checked;
        OptionsRepository.Write();
    }

    private static void Trade()
    {
        // Salva os dado
        Options.Trade = CheckBoxes.OptionsTrade.Checked;
        OptionsRepository.Write();
    }
}