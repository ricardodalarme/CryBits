using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Worlds;

namespace CryBits.Client.UI.Game.Views;

internal class BarsView
{
    private static Label HpValueLabel => Tools.Labels["Bars_HP_Value"];
    private static Label MpValueLabel => Tools.Labels["Bars_MP_Value"];
    private static Label ExpValueLabel => Tools.Labels["Bars_Exp_Value"];
    private static ProgressBar HpBar => Tools.ProgressBars["Bars_HP_Bar"];
    private static ProgressBar MpBar => Tools.ProgressBars["Bars_MP_Bar"];
    private static ProgressBar ExpBar => Tools.ProgressBars["Bars_Exp_Bar"];

    public static void Update()
    {
        var vitals = GameContext.Instance.LocalPlayer.GetVitals();
        var level = GameContext.Instance.LocalPlayer.GetLevel();
        if (vitals == null || level == null) return;

        var maxHp = vitals.MaxHp;
        var maxMp = vitals.MaxMp;

        HpBar.SetValue(maxHp > 0 ? (float)vitals.Hp / maxHp : 0f);
        MpBar.SetValue(maxMp > 0 ? (float)vitals.Mp / maxMp : 0f);
        ExpBar.SetValue(level.ExpNeeded > 0 ? (float)level.Experience / level.ExpNeeded : 0f);

        HpValueLabel.SetArguments(vitals.Hp, maxHp);
        MpValueLabel.SetArguments(vitals.Mp, maxMp);
        ExpValueLabel.SetArguments(level.Experience, level.ExpNeeded);
    }
}
