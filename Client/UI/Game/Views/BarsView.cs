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

    public static void Update()
    {
        HpValueLabel.SetArguments(Player.Me.Vital[(byte)Vital.Hp], Player.Me.MaxVital[(byte)Vital.Hp]);
        MpValueLabel.SetArguments(Player.Me.Vital[(byte)Vital.Mp], Player.Me.MaxVital[(byte)Vital.Mp]);
        ExpValueLabel.SetArguments(Player.Me.Experience, Player.Me.ExpNeeded);
    }
}
