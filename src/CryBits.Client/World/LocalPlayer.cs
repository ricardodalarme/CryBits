using CryBits.Client.Components;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Client.Worlds;

internal class LocalPlayer(World world, EntityId? entity)
{
    public EntityId? Entity = entity;

    public long Id;

    public string GetName() =>
        Entity.HasValue ? world.Get<PlayerAppearance>(Entity.Value)?.Name ?? string.Empty : string.Empty;

    public Vitals? GetVitals() => Entity.HasValue ? world.Get<Vitals>(Entity.Value) : null;

    public InventoryState? GetInventory() => Entity.HasValue ? world.Get<InventoryState>(Entity.Value) : null;

    public HotbarState? GetHotbar() => Entity.HasValue ? world.Get<HotbarState>(Entity.Value) : null;

    public LevelComponent? GetLevel() => Entity.HasValue ? world.Get<LevelComponent>(Entity.Value) : null;

    public TradeState? GetTrade() => Entity.HasValue ? world.Get<TradeState>(Entity.Value) : null;

    public AttributesComponent? GetAttributes() => Entity.HasValue ? world.Get<AttributesComponent>(Entity.Value) : null;

    public EquipmentState? GetEquipment() => Entity.HasValue ? world.Get<EquipmentState>(Entity.Value) : null;

    public PartyState? GetParty() => Entity.HasValue ? world.Get<PartyState>(Entity.Value) : null;

    public PlayerAppearance? GetAppearance() => Entity.HasValue ? world.Get<PlayerAppearance>(Entity.Value) : null;
}
