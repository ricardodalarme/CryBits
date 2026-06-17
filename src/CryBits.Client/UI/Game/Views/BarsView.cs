using CryBits.Client.Worlds;
using CryBits.Definitions.Characters;
using Iguina.Entities;
using ArchEntity = Arch.Core.Entity;
using Ent = global::Iguina.Entities.Entity;

namespace CryBits.Client.UI.Game.Views;

internal sealed class BarsView
{
    private readonly GameContext _context;
    private ProgressBar? _hpBar;
    private ProgressBar? _mpBar;
    private ProgressBar? _expBar;
    private Label? _hpLabel;
    private Label? _mpLabel;
    private Label? _expLabel;

    public BarsView(GameContext context) => _context = context;

    public void Wire(Dictionary<string, Ent> reg)
    {
        _hpBar = reg["HP_Bar"] as ProgressBar;
        _mpBar = reg["MP_Bar"] as ProgressBar;
        _expBar = reg["EXP_Bar"] as ProgressBar;
        _hpLabel = reg["HP_Value"] as Label;
        _mpLabel = reg["MP_Value"] as Label;
        _expLabel = reg["EXP_Value"] as Label;
        Update();
    }

    public void Update()
    {
        if (_context.LocalPlayer.Entity == ArchEntity.Null) return;
        ref var vitals = ref _context.LocalPlayer.GetVitals();
        ref var level = ref _context.LocalPlayer.GetLevel();
        var current = vitals.Current;
        var max = vitals.Max;
        var maxHp = max[(byte)Vital.Hp];
        var maxMp = max[(byte)Vital.Mp];

        _hpBar!.Value = maxHp > 0 ? current[(byte)Vital.Hp] * 100 / maxHp : 0;
        _mpBar!.Value = maxMp > 0 ? current[(byte)Vital.Mp] * 100 / maxMp : 0;
        _expBar!.Value = level.ExpNeeded > 0 ? level.Experience * 100 / level.ExpNeeded : 0;
        _hpLabel!.Text = $"HP: {current[(byte)Vital.Hp]}/{maxHp}";
        _mpLabel!.Text = $"MP: {current[(byte)Vital.Mp]}/{maxMp}";
        _expLabel!.Text = $"Exp: {level.Experience}/{level.ExpNeeded}";
    }
}
