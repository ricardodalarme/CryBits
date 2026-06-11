using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Logic;
using CryBits.Client.Worlds;
using System;

namespace CryBits.Client.UI.Game.Views;

internal class OptionsView(AudioManager audioManager, GameContext context) : IView
{
    internal static Panel Panel => Tools.Panels["Menu_Options"];
    internal static CheckBox SoundsCheckBox => Tools.CheckBoxes["Options_Sounds"];
    internal static CheckBox MusicsCheckBox => Tools.CheckBoxes["Options_Musics"];
    internal static CheckBox ChatCheckBox => Tools.CheckBoxes["Options_Chat"];
    internal static CheckBox MetricsCheckBox => Tools.CheckBoxes["Options_Metrics"];
    internal static CheckBox PartyCheckBox => Tools.CheckBoxes["Options_Party"];
    internal static CheckBox TradeCheckBox => Tools.CheckBoxes["Options_Trade"];

    public void Bind()
    {
        SoundsCheckBox.OnMouseUp += OnSoundsChanged;
        MusicsCheckBox.OnMouseUp += OnMusicsChanged;
        ChatCheckBox.OnMouseUp += OnChatChanged;
        MetricsCheckBox.OnMouseUp += OnMetricsChanged;
        PartyCheckBox.OnMouseUp += OnPartyInvitationsChanged;
        TradeCheckBox.OnMouseUp += OnTradeInvitationsChanged;
    }

    public void Unbind()
    {
        SoundsCheckBox.OnMouseUp -= OnSoundsChanged;
        MusicsCheckBox.OnMouseUp -= OnMusicsChanged;
        ChatCheckBox.OnMouseUp -= OnChatChanged;
        MetricsCheckBox.OnMouseUp -= OnMetricsChanged;
        PartyCheckBox.OnMouseUp -= OnPartyInvitationsChanged;
        TradeCheckBox.OnMouseUp -= OnTradeInvitationsChanged;
    }

    private void OnSoundsChanged()
    {
        Options.Instance.Sounds = !Options.Instance.Sounds;
        if (!Options.Instance.Sounds) audioManager.StopAllSounds();
        OptionsRepository.Write();
    }

    private void OnMusicsChanged()
    {
        Options.Instance.Musics = !Options.Instance.Musics;
        OptionsRepository.Write();

        if (!Options.Instance.Musics)
            audioManager.StopMusic();
        else if (Screen.Current == Screens.Menu)
            audioManager.PlayMusic(Musics.Menu);
        else if (Screen.Current == Screens.Game)
            audioManager.PlayMusic(context.CurrentMap.Data.Music);
    }

    private void OnChatChanged()
    {
        Options.Instance.Chat = ChatCheckBox.Checked;
        OptionsRepository.Write();
        if (Options.Instance.Chat) Chat.VisibilityTimer = Environment.TickCount64 + Chat.SleepTimer;
    }

    private void OnMetricsChanged()
    {
        Options.Instance.ShowMetrics = MetricsCheckBox.Checked;
        OptionsRepository.Write();
    }

    private void OnPartyInvitationsChanged()
    {
        Options.Instance.Party = PartyCheckBox.Checked;
        OptionsRepository.Write();
    }

    private void OnTradeInvitationsChanged()
    {
        Options.Instance.Trade = TradeCheckBox.Checked;
        OptionsRepository.Write();
    }
}
