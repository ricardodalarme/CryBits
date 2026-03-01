using Arch.Core;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Equipment;
using CryBits.Client.Components.Hotbar;
using CryBits.Client.Components.Inventory;
using CryBits.Client.Components.Player;
using CryBits.Client.Components.Trade;

namespace CryBits.Client.Worlds;

/// <summary>
/// Tracks the local player entity (the "Me" character).
/// Provides convenient access to the local player's entity and components.
/// </summary>
internal class LocalPlayer(Entity entity)
{
    /// <summary>The local player entity. Entity.Null if not logged in.</summary>
    public Entity Entity = entity;

    /// <summary>Convenient accessor for VitalsComponent.</summary>
    public ref VitalsComponent GetVitals() => ref GameContext.Instance.World.Get<VitalsComponent>(Entity);

    /// <summary>Convenient accessor for InventoryComponent.</summary>
    public ref InventoryComponent GetInventory() => ref GameContext.Instance.World.Get<InventoryComponent>(Entity);

    /// <summary>Convenient accessor for HotbarComponent.</summary>
    public ref HotbarComponent GetHotbar() => ref GameContext.Instance.World.Get<HotbarComponent>(Entity);

    /// <summary>Convenient accessor for LevelComponent.</summary>
    public ref LevelComponent GetLevel() => ref GameContext.Instance.World.Get<LevelComponent>(Entity);

    /// <summary>Convenient accessor for TradeComponent.</summary>
    public ref TradeComponent GetTrade() => ref GameContext.Instance.World.Get<TradeComponent>(Entity);

    /// <summary>Convenient accessor for AttributesComponent.</summary>
    public ref AttributesComponent GetAttributes() => ref GameContext.Instance.World.Get<AttributesComponent>(Entity);

    /// <summary>Convenient accessor for EquipmentComponent.</summary>
    public ref EquipmentComponent GetEquipment() => ref GameContext.Instance.World.Get<EquipmentComponent>(Entity);
}
