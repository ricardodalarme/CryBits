using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Worlds;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class OptionsView(IguinaContext uiContext, AudioManager audioManager, GameContext context) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("OptionsPanel");
    internal Checkbox SoundsCheckbox => uiContext.Get<Checkbox>("Sounds");
    internal Checkbox MusicsCheckbox => uiContext.Get<Checkbox>("Music");
    internal Checkbox ChatCheckbox => uiContext.Get<Checkbox>("ChatPreview");
    internal Checkbox MetricsCheckbox => uiContext.Get<Checkbox>("ShowFPS");
    internal Checkbox PartyCheckbox => uiContext.Get<Checkbox>("PartyInvites");
    internal Checkbox TradeCheckbox => uiContext.Get<Checkbox>("TradeInvites");

    public override void Bind()
    {
        SoundsCheckbox.Events.OnValueChanged += OnSoundsChanged;
        MusicsCheckbox.Events.OnValueChanged += OnMusicsChanged;
        ChatCheckbox.Events.OnValueChanged += OnChatChanged;
        MetricsCheckbox.Events.OnValueChanged += OnMetricsChanged;
        PartyCheckbox.Events.OnValueChanged += OnPartyInvitationsChanged;
        TradeCheckbox.Events.OnValueChanged += OnTradeInvitationsChanged;
    }

    public override void Unbind()
    {
        SoundsCheckbox.Events.OnValueChanged -= OnSoundsChanged;
        MusicsCheckbox.Events.OnValueChanged -= OnMusicsChanged;
        ChatCheckbox.Events.OnValueChanged -= OnChatChanged;
        MetricsCheckbox.Events.OnValueChanged -= OnMetricsChanged;
        PartyCheckbox.Events.OnValueChanged -= OnPartyInvitationsChanged;
        TradeCheckbox.Events.OnValueChanged -= OnTradeInvitationsChanged;
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
        else if (context.CurrentMap?.Music != null)
            audioManager.PlayMusic(context.CurrentMap.Music);
        else
            audioManager.PlayMusic(Musics.Menu);
    }

    private void OnChatChanged(Entity _)
    {
        Options.Instance.Chat = ChatCheckbox.Checked;
        OptionsRepository.Write();
        if (Options.Instance.Chat) Chat.VisibilityTimer = Environment.TickCount64 + Chat.SleepTimer;
    }

    private void OnMetricsChanged(Entity _)
    {
        Options.Instance.ShowMetrics = MetricsCheckbox.Checked;
        OptionsRepository.Write();
    }

    private void OnPartyInvitationsChanged(Entity _)
    {
        Options.Instance.Party = PartyCheckbox.Checked;
        OptionsRepository.Write();
    }

    private void OnTradeInvitationsChanged(Entity _)
    {
        Options.Instance.Trade = TradeCheckbox.Checked;
        OptionsRepository.Write();
    }
}
