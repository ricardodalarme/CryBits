using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using System.Linq;

namespace CryBits.Server.Systems.Inventory;

internal sealed class EquipmentSystem(PlayerSender playerSender, DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static EquipmentSystem Instance { get; } = new(PlayerSender.Instance, DefinitionCatalog.Instance);

    public void Equip(Player player, Item item)
    {
        var oldItem = player.Equipment[item.EquipType];

        player.Equipment[item.EquipType] = item;
        for (byte i = 0; i < (byte)Attribute.Count; i++)
            player.Attribute[i] += item.EquipAttribute[i];
        if (oldItem != null)
            for (byte i = 0; i < (byte)Attribute.Count; i++)
                player.Attribute[i] -= oldItem.EquipAttribute[i];

        GameWorld.Current.CurrentTick?.Events.Emit(new ItemEquippedEvent
        {
            PlayerId = player.Id,
            EquipSlot = item.EquipType,
            ItemId = item.Id,
            OldItemId = oldItem?.Id
        });

        playerSender.PlayerEquipments(player);
    }

    public void Unequip(Player player, byte equipSlot)
    {
        if (player.Equipment[equipSlot] == null) return;
        if (player.Equipment[equipSlot].Bind == BindOn.Equip) return;

        var oldItem = player.Equipment[equipSlot];

        for (byte i = 0; i < (byte)Attribute.Count; i++)
            player.Attribute[i] -= oldItem.EquipAttribute[i];
        player.Equipment[equipSlot] = null;

        GameWorld.Current.CurrentTick?.Events.Emit(new ItemEquippedEvent
        {
            PlayerId = player.Id,
            EquipSlot = equipSlot,
            ItemId = null,
            OldItemId = oldItem.Id
        });

        playerSender.PlayerEquipments(player);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events.ToArray())
        {
            if (ev is not ItemUsedEvent use) continue;
            var player = world.FindPlayer(use.PlayerId);
            if (player == null) continue;
            var item = _catalog.Items.Get(use.ItemId);
            if (item == null || item.Type != ItemType.Equipment) continue;
            Equip(player, item);
        }
    }
}
