using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Worlds;
using Iguina.Entities;
using Ent = global::Iguina.Entities.Entity;

namespace CryBits.Client.UI.Game.Views;

internal sealed class OptionsView
{
    private readonly AudioManager _audioManager;
    private readonly GameContext _context;
    private Panel? _panel;

    public bool IsVisible => _panel?.Visible ?? false;
    public void SetVisible(bool visible) { if (_panel != null) _panel.Visible = visible; }

    public OptionsView(AudioManager audioManager, GameContext context)
    {
        _audioManager = audioManager;
        _context = context;
    }

    public void Wire(Dictionary<string, Ent> reg)
    {
        _panel = reg["OptionsPanel"] as Panel;
        WireCheckbox(reg, "Sounds", Options.Instance.Sounds, ToggleSounds);
        WireCheckbox(reg, "Music", Options.Instance.Musics, ToggleMusic);
        WireCheckbox(reg, "ChatPreview", Options.Instance.Chat, ToggleChat);
        WireCheckbox(reg, "ShowFPS", Options.Instance.ShowMetrics, ToggleFps);
        WireCheckbox(reg, "PartyInvites", Options.Instance.Party, ToggleParty);
        WireCheckbox(reg, "TradeInvites", Options.Instance.Trade, ToggleTrade);
    }

    private void WireCheckbox(Dictionary<string, Ent> reg, string name, bool initial, Action<Checkbox> onChange)
    {
        if (reg.TryGetValue(name, out var ent) && ent is Checkbox cb)
        {
            cb.Checked = initial;
            cb.Events.OnChecked += _ => onChange(cb);
            cb.Events.OnUnchecked += _ => onChange(cb);
        }
    }

    private void ToggleSounds(Checkbox cb)
    {
        Options.Instance.Sounds = cb.Checked;
        if (!Options.Instance.Sounds) _audioManager.StopAllSounds();
        OptionsRepository.Write();
    }

    private void ToggleMusic(Checkbox cb)
    {
        Options.Instance.Musics = cb.Checked;
        OptionsRepository.Write();
        if (!Options.Instance.Musics) _audioManager.StopMusic();
        else if (GameState.CurrentScreen == ScreenType.Menu)
            _audioManager.PlayMusic(Musics.Menu);
        else if (GameState.CurrentScreen == ScreenType.Game)
            _audioManager.PlayMusic(_context.CurrentMap.Data.Music);
    }

    private static void ToggleChat(Checkbox cb)
    {
        Options.Instance.Chat = cb.Checked;
        OptionsRepository.Write();
        if (Options.Instance.Chat) UI.Game.Chat.VisibilityTimer = Environment.TickCount64 + UI.Game.Chat.SleepTimer;
    }

    private static void ToggleFps(Checkbox cb)
    {
        Options.Instance.ShowMetrics = cb.Checked;
        OptionsRepository.Write();
    }

    private static void ToggleParty(Checkbox cb)
    {
        Options.Instance.Party = cb.Checked;
        OptionsRepository.Write();
    }

    private static void ToggleTrade(Checkbox cb)
    {
        Options.Instance.Trade = cb.Checked;
        OptionsRepository.Write();
    }
}
