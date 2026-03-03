using System;
using Arch.Core;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Equipment;
using CryBits.Client.Components.Hotbar;
using CryBits.Client.Components.Inventory;
using CryBits.Client.Components.Map;
using CryBits.Client.Components.Movement;
using CryBits.Client.Components.Player;
using CryBits.Client.Components.Trade;
using CryBits.Client.Components.Party;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using static CryBits.Globals;

namespace CryBits.Client.Worlds;

/// <summary>
/// Tracks the local player entity (the "Me" character).
/// Provides convenient access to the local player's entity and components.
/// </summary>
internal class LocalPlayer(Entity entity)
{
    /// <summary>The local player entity. Entity.Null if not logged in.</summary>
    public Entity Entity = entity;

    private long _collectTimer;

    /// <summary>Convenient accessor for the local player's name.</summary>
    public string GetName() =>
        Entity != Entity.Null ? GameContext.Instance.World.Get<NameComponent>(Entity).Value : string.Empty;

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

    /// <summary>Convenient accessor for AppearanceComponent.</summary>
    public ref AppearanceComponent GetAppearance() => ref GameContext.Instance.World.Get<AppearanceComponent>(Entity);

    /// <summary>Convenient accessor for PartyComponent.</summary>
    public ref PartyComponent GetParty() => ref GameContext.Instance.World.Get<PartyComponent>(Entity);

    /// <summary>
    /// Collects the item at the local player's current tile, if any free inventory slot exists.
    /// Debounced to 250 ms to prevent server spam.
    /// </summary>
    public void CollectItem()
    {
        if (TextBox.Focused != null) return;
        if (Entity == Entity.Null) return;

        bool hasItem = false, hasSlot = false;

        var world = GameContext.Instance.World;
        var myTile = world.Get<MovementComponent>(Entity);
        var itemQuery = new QueryDescription().WithAll<GroundItemComponent, TransformComponent>();
        world.Query(in itemQuery, (ref GroundItemComponent _, ref TransformComponent transform) =>
        {
            if (transform.X / Grid == myTile.TileX && transform.Y / Grid == myTile.TileY)
                hasItem = true;
        });

        ref var inv = ref GetInventory();
        for (byte i = 0; i < MaxInventory; i++)
            if (inv.Slots[i]?.Item == null)
                hasSlot = true;

        if (!hasItem || !hasSlot) return;
        if (Environment.TickCount64 <= _collectTimer + 250) return;

        PlayerSender.Instance.CollectItem();
        _collectTimer = Environment.TickCount64;
    }
}