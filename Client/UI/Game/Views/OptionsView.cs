using System;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Logic;
using CryBits.Client.Worlds;

namespace CryBits.Client.UI.Game.Views;

internal class OptionsView(AudioManager audioManager) : IView
{
    internal static Panel Panel => Tools.Panels["Menu_Options"];
    internal static CheckBox SoundsCheckBox => Tools.CheckBoxes["Options_Sounds"];
    internal static CheckBox MusicsCheckBox => Tools.CheckBoxes["Options_Musics"];
    internal static CheckBox ChatCheckBox => Tools.CheckBoxes["Options_Chat"];
    internal static CheckBox FpsCheckBox => Tools.CheckBoxes["Options_FPS"];
    internal static CheckBox LatencyCheckBox => Tools.CheckBoxes["Options_Latency"];
    internal static CheckBox PartyCheckBox => Tools.CheckBoxes["Options_Party"];
    internal static CheckBox TradeCheckBox => Tools.CheckBoxes["Options_Trade"];

    public void Bind()
    {
        SoundsCheckBox.OnMouseUp += OnSoundsChanged;
        MusicsCheckBox.OnMouseUp += OnMusicsChanged;
        ChatCheckBox.OnMouseUp += OnChatChanged;
        FpsCheckBox.OnMouseUp += OnFpsChanged;
        LatencyCheckBox.OnMouseUp += OnLatencyChanged;
        PartyCheckBox.OnMouseUp += OnPartyInvitationsChanged;
        TradeCheckBox.OnMouseUp += OnTradeInvitationsChanged;
    }

    public void Unbind()
    {
        SoundsCheckBox.OnMouseUp -= OnSoundsChanged;
        MusicsCheckBox.OnMouseUp -= OnMusicsChanged;
        ChatCheckBox.OnMouseUp -= OnChatChanged;
        FpsCheckBox.OnMouseUp -= OnFpsChanged;
        LatencyCheckBox.OnMouseUp -= OnLatencyChanged;
        PartyCheckBox.OnMouseUp -= OnPartyInvitationsChanged;
        TradeCheckBox.OnMouseUp -= OnTradeInvitationsChanged;
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
            audioManager.PlayMusic(GameContext.Instance.CurrentMap.Data.Music);
    }

    private void OnChatChanged()
    {
        Options.Chat = ChatCheckBox.Checked;
        OptionsRepository.Write();
        if (Options.Chat) GameLoop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
    }

    private void OnFpsChanged()
    {
        Options.Fps = FpsCheckBox.Checked;
        OptionsRepository.Write();
    }

    private void OnLatencyChanged()
    {
        Options.Latency = LatencyCheckBox.Checked;
        OptionsRepository.Write();
    }

    private void OnPartyInvitationsChanged()
    {
        Options.Party = PartyCheckBox.Checked;
        OptionsRepository.Write();
    }

    private void OnTradeInvitationsChanged()
    {
        Options.Trade = TradeCheckBox.Checked;
        OptionsRepository.Write();
    }
}
