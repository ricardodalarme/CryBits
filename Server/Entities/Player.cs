using System.Collections.Generic;
using System.Linq;
using CryBits.Entities;
using CryBits.Entities.Shop;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Formulas;
using CryBits.Server.World;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Entities;

/// <summary>
/// Thin handle linking a player's network session to their ECS entity.
/// All mutable player data lives in ECS components accessible via <see cref="Get{T}"/>.
/// </summary>
internal sealed class Player
{
    // ─── Identity ────────────────────────────────────────────────────────────

    /// <summary>ECS entity id for this player.</summary>
    public int EntityId { get; }

    /// <summary>The network session associated with this player.</summary>
    public GameSession Session { get; }

    public Player(int entityId, GameSession session)
    {
        EntityId = entityId;
        Session = session;
    }

    // ─── ECS access ─────────────────────────────────────────────────────────

    private static Server.ECS.World World => ServerContext.Instance.World;

    /// <summary>Retrieves a component from this player's entity. Throws if absent.</summary>
    public T Get<T>() where T : class, IComponent => World.Get<T>(EntityId);

    /// <summary>Returns true when this player's entity has the component type.</summary>
    public bool Has<T>() where T : class, IComponent => World.Has<T>(EntityId);

    // ─── Map helper ──────────────────────────────────────────────────────────

    /// <summary>Current map instance for this player.</summary>
    public MapInstance MapInstance =>
        GameWorld.Current.Maps.Get(Get<PositionComponent>().MapId)!;

    // ─── Computed stats ──────────────────────────────────────────────────────

    /// <summary>Gets the player's computed damage (Strength + weapon bonus).</summary>
    public short Damage => CombatFormulas.PlayerDamage(
        Get<AttributeComponent>().Values[(byte)Attribute.Strength],
        Get<EquipmentComponent>().Slots[(byte)Equipment.Weapon]?.WeaponDamage ?? 0);

    /// <summary>Gets the player's defense value (Resistance attribute).</summary>
    public short PlayerDefense =>
        CombatFormulas.PlayerDefense(Get<AttributeComponent>().Values[(byte)Attribute.Resistance]);

    /// <summary>Maximum value for the specified vital.</summary>
    public short MaxVital(byte vital)
    {
        var data = Get<PlayerDataComponent>();
        var attr = Get<AttributeComponent>();
        return VitalFormulas.MaxVital(
            (Vital)vital,
            data.Class!.Vital[vital],
            attr.Values[(byte)Attribute.Vitality],
            attr.Values[(byte)Attribute.Intelligence],
            data.Level);
    }

    /// <summary>Per-tick regeneration amount for the specified vital.</summary>
    public short Regeneration(byte vital)
    {
        var attr = Get<AttributeComponent>();
        return VitalFormulas.PlayerRegeneration(
            (Vital)vital,
            MaxVital(vital),
            attr.Values[(byte)Attribute.Vitality],
            attr.Values[(byte)Attribute.Intelligence]);
    }

    /// <summary>Experience required to reach the next level.</summary>
    public int ExpNeeded
    {
        get
        {
            var data = Get<PlayerDataComponent>();
            var attr = Get<AttributeComponent>();
            short total = 0;
            for (byte i = 0; i < (byte)Attribute.Count; i++) total += attr.Values[i];
            return LevelingFormulas.ExperienceNeeded(data.Level, total, data.Points);
        }
    }

    // ─── Inventory / hotbar helpers ─────────────────────────────────────────

    public ItemSlot? FindInventory(Item? item) =>
        Get<InventoryComponent>().Slots.FirstOrDefault(x => x.Item == item);

    public HotbarSlot? FindHotbar(SlotType type, short slot) =>
        Get<HotbarComponent>().Slots.FirstOrDefault(x => x.Type == type && x.Slot == slot);

    public HotbarSlot? FindHotbar(SlotType type, ItemSlot targetSlot)
    {
        var inv = Get<InventoryComponent>().Slots;
        return Get<HotbarComponent>().Slots
            .FirstOrDefault(x => x.Type == type && x.Slot < inv.Length && inv[x.Slot] == targetSlot);
    }

    /// <summary>Number of inventory slots that currently hold an item.</summary>
    public byte TotalInventoryFree =>
        (byte)Get<InventoryComponent>().Slots.Count(x => x.Item != null);

    /// <summary>Number of items offered in the active trade.</summary>
    public byte TotalTradeItems =>
        (byte)(Get<TradeComponent>().Offer?.Count(x => x.SlotNum != 0) ?? 0);

    // ─── Static lookup ───────────────────────────────────────────────────────

    /// <summary>Finds a playing player by name; returns null when not found.</summary>
    public static Player? Find(string name) =>
        GameWorld.Current.Sessions.Find(x => x.IsPlaying &&
                                             World.TryGet<PlayerDataComponent>(x.Character!.EntityId, out var d) &&
                                             d.Name.Equals(name))?.Character;
}
