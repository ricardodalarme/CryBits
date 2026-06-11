using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Equipment;
using CryBits.Client.Components.Hotbar;
using CryBits.Client.Components.Inventory;
using CryBits.Client.Components.Party;
using CryBits.Client.Components.Player;
using CryBits.Client.Components.Trade;
using System;

namespace CryBits.Client.Worlds;

/// <summary>
/// Tracks the local player entity (the "Me" character).
/// Provides convenient access to the local player's entity and components.
/// </summary>
internal class LocalPlayer(World world, Entity entity)
{
    /// <summary>The local player entity. Entity.Null if not logged in.</summary>
    public Entity Entity = entity;

    /// <summary>Network ID of the local player.</summary>
    public Guid Id;

    /// <summary>Convenient accessor for the local player's name.</summary>
    public string GetName() =>
        Entity != Entity.Null ? world.Get<NameComponent>(Entity).Value : string.Empty;

    /// <summary>Convenient accessor for VitalsComponent.</summary>
    public ref VitalsComponent GetVitals() => ref world.Get<VitalsComponent>(Entity);

    /// <summary>Convenient accessor for InventoryComponent.</summary>
    public ref InventoryComponent GetInventory() => ref world.Get<InventoryComponent>(Entity);

    /// <summary>Convenient accessor for HotbarComponent.</summary>
    public ref HotbarComponent GetHotbar() => ref world.Get<HotbarComponent>(Entity);

    /// <summary>Convenient accessor for LevelComponent.</summary>
    public ref LevelComponent GetLevel() => ref world.Get<LevelComponent>(Entity);

    /// <summary>Convenient accessor for TradeComponent.</summary>
    public ref TradeComponent GetTrade() => ref world.Get<TradeComponent>(Entity);

    /// <summary>Convenient accessor for AttributesComponent.</summary>
    public ref AttributesComponent GetAttributes() => ref world.Get<AttributesComponent>(Entity);

    /// <summary>Convenient accessor for EquipmentComponent.</summary>
    public ref EquipmentComponent GetEquipment() => ref world.Get<EquipmentComponent>(Entity);

    /// <summary>Convenient accessor for FaceComponent.</summary>
    public ref FaceComponent GetFaceComponent() => ref world.Get<FaceComponent>(Entity);

    /// <summary>Convenient accessor for PartyComponent.</summary>
    public ref PartyComponent GetParty() => ref world.Get<PartyComponent>(Entity);
}
