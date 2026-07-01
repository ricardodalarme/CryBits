using CryBits.Client.Worlds;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class BarsView(IguinaContext uiContext) : ViewBase
{
    private Label HpValueLabel => uiContext.Get<Label>("HP_Value");
    private Label MpValueLabel => uiContext.Get<Label>("MP_Value");
    private Label ExpValueLabel => uiContext.Get<Label>("EXP_Value");
    private ProgressBar HpBar => uiContext.Get<ProgressBar>("HP_Bar");
    private ProgressBar MpBar => uiContext.Get<ProgressBar>("MP_Bar");
    private ProgressBar ExpBar => uiContext.Get<ProgressBar>("EXP_Bar");

    public override void Bind()
    {
        uiContext.PostDraw += Update;
    }

    public override void Unbind()
    {
        uiContext.PostDraw -= Update;
    }

    private void Update()
    {
        var vitals = GameContext.Instance.LocalPlayer.GetVitals();
        var level = GameContext.Instance.LocalPlayer.GetLevel();
        if (vitals == null || level == null) return;

        var maxHp = vitals.MaxHp;
        var maxMp = vitals.MaxMp;

        HpBar.ValueSafe = maxHp > 0 ? (int)((float)vitals.Hp / maxHp * 100f) : 0;
        MpBar.ValueSafe = maxMp > 0 ? (int)((float)vitals.Mp / maxMp * 100f) : 0;
        ExpBar.ValueSafe = level.ExpNeeded > 0 ? (int)((float)level.Experience / level.ExpNeeded * 100f) : 0;

        HpValueLabel.Text = $"HP: {vitals.Hp}/{maxHp}";
        MpValueLabel.Text = $"MP: {vitals.Mp}/{maxMp}";
        ExpValueLabel.Text = $"EXP: {level.Experience}/{level.ExpNeeded}";
    }
}
