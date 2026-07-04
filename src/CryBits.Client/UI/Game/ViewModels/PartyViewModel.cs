using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.State;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class PartyMemberViewModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public short Hp { get; set; }
    public short MaxHp { get; set; } = 1;
    public short Mp { get; set; }
    public short MaxMp { get; set; } = 1;
}

internal sealed class PartyViewModel : IDisposable
{
    private readonly Func<long, EntityId?> _lookup;
    private readonly IDisposable _vitalsSubscription;
    private readonly IDisposable _appearanceSubscription;

    public List<PartyMemberViewModel> Members { get; set; } = [];

    public PartyViewModel(World world, Func<long, EntityId?> lookup)
    {
        _lookup = lookup;

        _vitalsSubscription = world.Events.On<Vitals>()
            .OnChanged(OnVitalsChanged);

        _appearanceSubscription = world.Events.On<PlayerAppearance>()
            .OnChanged(OnAppearanceChanged);
    }

    private void OnVitalsChanged(ComponentChanged<Vitals> evt)
    {
        foreach (var member in Members)
        {
            if (_lookup(member.Id) != evt.Entity) continue;
            member.Hp = evt.Component.Hp;
            member.MaxHp = evt.Component.MaxHp;
            member.Mp = evt.Component.Mp;
            member.MaxMp = evt.Component.MaxMp;
            break;
        }
    }

    private void OnAppearanceChanged(ComponentChanged<PlayerAppearance> evt)
    {
        var name = evt.Component.Name;
        foreach (var member in Members)
        {
            if (_lookup(member.Id) != evt.Entity) continue;
            member.Name = name;
            break;
        }
    }

    public void Dispose()
    {
        _vitalsSubscription.Dispose();
        _appearanceSubscription.Dispose();
    }
}
