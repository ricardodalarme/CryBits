using CryBits.Client.Core;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Simulation.Core;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class OptionsView(UiContext uiContext, AudioManager audioManager, World world, Chat chat) : ViewBase
{
    internal CheckButton SoundsCheckbox => uiContext.Get<CheckButton>("Sounds");
    internal CheckButton MusicsCheckbox => uiContext.Get<CheckButton>("Music");
    internal CheckButton ChatCheckbox => uiContext.Get<CheckButton>("ChatPreview");
    internal CheckButton MetricsCheckbox => uiContext.Get<CheckButton>("ShowFPS");
    internal CheckButton PartyCheckbox => uiContext.Get<CheckButton>("PartyInvites");
    internal CheckButton TradeCheckbox => uiContext.Get<CheckButton>("TradeInvites");

    public override void Bind()
    {
        SoundsCheckbox.Click += OnSoundsChanged;
        MusicsCheckbox.Click += OnMusicsChanged;
        ChatCheckbox.Click += OnChatChanged;
        MetricsCheckbox.Click += OnMetricsChanged;
        PartyCheckbox.Click += OnPartyInvitationsChanged;
        TradeCheckbox.Click += OnTradeInvitationsChanged;
    }

    public override void Unbind()
    {
        SoundsCheckbox.Click -= OnSoundsChanged;
        MusicsCheckbox.Click -= OnMusicsChanged;
        ChatCheckbox.Click -= OnChatChanged;
        MetricsCheckbox.Click -= OnMetricsChanged;
        PartyCheckbox.Click -= OnPartyInvitationsChanged;
        TradeCheckbox.Click -= OnTradeInvitationsChanged;
    }

    private void OnSoundsChanged(object? sender, MyraEventArgs e)
    {
        Options.Instance.Sounds = SoundsCheckbox.IsChecked;
        if (!Options.Instance.Sounds) audioManager.StopAllSounds();
        OptionsRepository.Write();
    }

    private void OnMusicsChanged(object? sender, MyraEventArgs e)
    {
        Options.Instance.Musics = MusicsCheckbox.IsChecked;
        OptionsRepository.Write();

        if (!Options.Instance.Musics)
            audioManager.StopMusic();
        else if (world.CurrentMap?.Music != null)
            audioManager.PlayMusic(world.CurrentMap!.Music);
        else
            audioManager.PlayMusic(Musics.Menu);
    }

    private void OnChatChanged(object? sender, MyraEventArgs e)
    {
        Options.Instance.Chat = ChatCheckbox.IsChecked;
        OptionsRepository.Write();
        if (Options.Instance.Chat) chat.VisibilityTimer = Environment.TickCount64 + Chat.SleepTimer;
    }

    private void OnMetricsChanged(object? sender, MyraEventArgs e)
    {
        Options.Instance.ShowMetrics = MetricsCheckbox.IsChecked;
        OptionsRepository.Write();
    }

    private void OnPartyInvitationsChanged(object? sender, MyraEventArgs e)
    {
        Options.Instance.Party = PartyCheckbox.IsChecked;
        OptionsRepository.Write();
    }

    private void OnTradeInvitationsChanged(object? sender, MyraEventArgs e)
    {
        Options.Instance.Trade = TradeCheckbox.IsChecked;
        OptionsRepository.Write();
    }
}
