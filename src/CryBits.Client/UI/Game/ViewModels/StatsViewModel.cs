using CryBits.Client.Components;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class StatsViewModel : IDisposable
{
    private readonly IDisposable _vitalsSubscription;
    private readonly IDisposable _levelSubscription;

    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Mp { get; private set; }
    public int MaxMp { get; private set; }
    public int Experience { get; private set; }
    public int ExpNeeded { get; private set; }

    public int HpPercent { get; private set; }
    public int MpPercent { get; private set; }
    public int ExpPercent { get; private set; }

    public StatsViewModel(World world)
    {
        _vitalsSubscription = world.Events.On<Vitals>()
            .With<LocalPlayerTag>()
            .OnChanged(OnVitalsChanged);

        _levelSubscription = world.Events.On<LevelComponent>()
            .With<LocalPlayerTag>()
            .OnChanged(OnLevelChanged);
    }

    private void OnVitalsChanged(ComponentChanged<Vitals> evt)
    {
        Hp = evt.Component.Hp;
        MaxHp = evt.Component.MaxHp;
        Mp = evt.Component.Mp;
        MaxMp = evt.Component.MaxMp;

        HpPercent = MaxHp > 0 ? (int)((float)Hp / MaxHp * 100f) : 0;
        MpPercent = MaxMp > 0 ? (int)((float)Mp / MaxMp * 100f) : 0;
    }

    private void OnLevelChanged(ComponentChanged<LevelComponent> evt)
    {
        Experience = evt.Component.Experience;
        ExpNeeded = evt.Component.ExpNeeded;

        ExpPercent = ExpNeeded > 0 ? (int)((float)Experience / ExpNeeded * 100f) : 0;
    }

    public void Dispose()
    {
        _vitalsSubscription.Dispose();
        _levelSubscription.Dispose();
    }
}
