using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;
using LiteNetLib.Utils;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Network.Handlers;

internal static class PlayerHandler
{
    internal static void PlayerDirection(Player player, NetDataReader data)
    {
        MovementSystem.ChangeDirection(player, (Direction)data.GetByte());
    }

    internal static void PlayerMove(Player player, NetDataReader data)
    {
        if (player.X != data.GetByte() || player.Y != data.GetByte())
            PlayerSender.PlayerPosition(player);
        else
            MovementSystem.Move(player, data.GetByte());
    }

    internal static void PlayerAttack(Player player)
    {
        // Ataca
        player.Attack();
    }

    internal static void AddPoint(Player player, NetDataReader data)
    {
        var attributeNum = data.GetByte();

        // Adiciona um ponto a determinado atributo
        if (player.Points > 0)
        {
            player.Attribute[attributeNum]++;
            player.Points--;
            PlayerSender.PlayerExperience(player);
            MapSender.MapPlayers(player);
        }
    }

    internal static void CollectItem(Player player)
    {
        InventorySystem.CollectItem(player);
    }

    internal static void DropItem(Player player, NetDataReader data)
    {
        var slot = data.GetShort();
        var amount = data.GetShort();
        if (slot != -1) InventorySystem.DropItem(player, player.Inventory[slot], amount);
    }

    internal static void InventoryChange(Player player, NetDataReader data)
    {
        short slotOld = data.GetShort(), slotNew = data.GetShort();

        // Somente se necessário
        if (player.Inventory[slotOld].Item == null) return;
        if (slotOld == slotNew) return;
        if (player.Trade != null) return;

        // Muda o item de slot
        (player.Inventory[slotOld], player.Inventory[slotNew]) = (player.Inventory[slotNew], player.Inventory[slotOld]);
        PlayerSender.PlayerInventory(player);

        // Altera na hotbar
        var hotbarSlot = player.FindHotbar(SlotType.Item, player.Inventory[slotOld]);
        if (hotbarSlot != null)
        {
            hotbarSlot.Slot = slotNew;
            PlayerSender.PlayerHotbar(player);
        }
    }

    internal static void InventoryUse(Player player, NetDataReader data)
    {
        InventorySystem.UseItem(player, player.Inventory[data.GetByte()]);
    }

    internal static void EquipmentRemove(Player player, NetDataReader data)
    {
        EquipmentSystem.Unequip(player, data.GetByte());
    }

    internal static void HotbarAdd(Player player, NetDataReader data)
    {
        var hotbarSlot = data.GetShort();
        var type = (SlotType)data.GetByte();
        var slot = data.GetShort();

        // Somente se necessário
        if (slot != 0 && player.FindHotbar(type, slot) != null) return;

        // Define os dados
        player.Hotbar[hotbarSlot].Slot = slot;
        player.Hotbar[hotbarSlot].Type = type;

        // Envia os dados
        PlayerSender.PlayerHotbar(player);
    }

    internal static void HotbarChange(Player player, NetDataReader data)
    {
        short slotOld = data.GetShort(), slotNew = data.GetShort();

        // Somente se necessário
        if (slotOld < 0 || slotNew < 0) return;
        if (slotOld == slotNew) return;
        if (player.Hotbar[slotOld].Slot == 0) return;

        // Muda o item de slot
        (player.Hotbar[slotOld], player.Hotbar[slotNew]) = (player.Hotbar[slotNew], player.Hotbar[slotOld]);
        PlayerSender.PlayerHotbar(player);
    }

    internal static void HotbarUse(Player player, NetDataReader data)
    {
        var hotbarSlot = data.GetShort();

        // Usa o item
        switch (player.Hotbar[hotbarSlot].Type)
        {
            case SlotType.Item:
                InventorySystem.UseItem(player, player.Inventory[player.Hotbar[hotbarSlot].Slot]); break;
        }
    }
}