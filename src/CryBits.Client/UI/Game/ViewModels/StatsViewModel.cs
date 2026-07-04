using CryBits.Client.Core;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class StatsViewModel(GameContext context)
{
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Mp { get; private set; }
    public int MaxMp { get; private set; }
    public int Experience { get; private set; }
    public int ExpNeeded { get; private set; }

    public int HpPercent { get; private set; }
    public int MpPercent { get; private set; }
    public int ExpPercent { get; private set; }

    public void Refresh()
    {
        var vitals = context.LocalPlayer.GetVitals();
        var level = context.LocalPlayer.GetLevel();
        if (vitals == null || level == null) return;

        Hp = vitals.Hp;
        MaxHp = vitals.MaxHp;
        Mp = vitals.Mp;
        MaxMp = vitals.MaxMp;
        Experience = level.Experience;
        ExpNeeded = level.ExpNeeded;

        HpPercent = MaxHp > 0 ? (int)((float)Hp / MaxHp * 100f) : 0;
        MpPercent = MaxMp > 0 ? (int)((float)Mp / MaxMp * 100f) : 0;
        ExpPercent = ExpNeeded > 0 ? (int)((float)Experience / ExpNeeded * 100f) : 0;
    }
}
