using CryBits.Client.Core;

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

internal sealed class PartyViewModel(GameContext context)
{
    public List<PartyMemberViewModel> Members { get; set; } = [];

    public void Refresh()
    {
        var world = context.World;
        foreach (var member in Members)
        {
            var entity = context.GetNetworkEntity(member.Id);
            if (entity != null)
            {
                var vitals = world.Get<Simulation.Components.Vitals>(entity.Value);
                if (vitals != null)
                {
                    member.Hp = vitals.Hp;
                    member.MaxHp = vitals.MaxHp;
                    member.Mp = vitals.Mp;
                    member.MaxMp = vitals.MaxMp;
                }
                member.Name = world.Get<Simulation.Components.PlayerAppearance>(entity.Value)?.Name ?? string.Empty;
            }
        }
    }
}
