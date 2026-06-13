using CryBits.Definitions.Characters;
using CryBits.Definitions.Items;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.Events;
using CryBits.Server.World;
using System.Linq;

namespace CryBits.Server.Systems.Inventory;

internal sealed class EquipmentSystem(PlayerSender playerSender) : ISimulationSystem
{
    public static EquipmentSystem Instance { get; } = new(PlayerSender.Instance);

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
            Player = player,
            EquipSlot = item.EquipType,
            Item = item,
            OldItem = oldItem
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
            Player = player,
            EquipSlot = equipSlot,
            Item = null,
            OldItem = oldItem
        });

        playerSender.PlayerEquipments(player);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events.ToArray())
        {
            if (ev is ItemUsedEvent use && use.Item.Type == ItemType.Equipment)
                Equip(use.Player, use.Item);
        }
    }
}
