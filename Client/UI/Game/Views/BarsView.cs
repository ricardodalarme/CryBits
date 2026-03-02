using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Enums;

namespace CryBits.Client.UI.Game.Views;

internal static class BarsView
{
    private static Label HpValueLabel => Tools.Labels["Bars_HP_Value"];
    private static Label MpValueLabel => Tools.Labels["Bars_MP_Value"];
    private static Label ExpValueLabel => Tools.Labels["Bars_Exp_Value"];
    private static ProgressBar HpBar => Tools.ProgressBars["Bars_HP_Bar"];
    private static ProgressBar MpBar => Tools.ProgressBars["Bars_MP_Bar"];
    private static ProgressBar ExpBar => Tools.ProgressBars["Bars_Exp_Bar"];

    public static void Update()
    {
        var maxHp = Player.Me.MaxVital[(byte)Vital.Hp];
        var maxMp = Player.Me.MaxVital[(byte)Vital.Mp];

        HpBar.SetValue(maxHp > 0 ? (float)Player.Me.Vital[(byte)Vital.Hp] / maxHp : 0f);
        MpBar.SetValue(maxMp > 0 ? (float)Player.Me.Vital[(byte)Vital.Mp] / maxMp : 0f);
        ExpBar.SetValue(Player.Me.ExpNeeded > 0 ? (float)Player.Me.Experience / Player.Me.ExpNeeded : 0f);

        HpValueLabel.SetArguments(Player.Me.Vital[(byte)Vital.Hp], maxHp);
        MpValueLabel.SetArguments(Player.Me.Vital[(byte)Vital.Mp], maxMp);
        ExpValueLabel.SetArguments(Player.Me.Experience, Player.Me.ExpNeeded);
    }
}
