using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Network.Senders;
using LiteNetLib.Utils;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Network.Handlers;

internal static class PlayerHandler
{
    internal static void PlayerDirection(Player player, NetDataReader data)
    {
        var direction = (Direction)data.GetByte();

        // Previne erros
        if (direction < Direction.Up || direction > Direction.Right) return;
        if (player.GettingMap) return;

        // Defini a direção do jogador
        player.Direction = direction;
        PlayerSender.PlayerDirection(player);
    }

    internal static void PlayerMove(Player player, NetDataReader data)
    {
        // Move o jogador se necessário
        if (player.X != data.GetByte() || player.Y != data.GetByte())
            PlayerSender.PlayerPosition(player);
        else
            player.Move(data.GetByte());
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
        var mapItem = player.Map.HasItem(player.X, player.Y);

        // Somente se necessário
        if (mapItem == null) return;

        // Dá o item ao jogador
        if (player.GiveItem(mapItem.Item, mapItem.Amount))
        {
            // Retira o item do mapa
            player.Map.Item.Remove(mapItem);
            MapSender.MapItems(player.Map);
        }
    }

    internal static void DropItem(Player player, NetDataReader data)
    {
        player.DropItem(data.GetShort(), data.GetShort());
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
        player.UseItem(player.Inventory[data.GetByte()]);
    }

    internal static void EquipmentRemove(Player player, NetDataReader data)
    {
        var slot = data.GetByte();

        // Apenas se necessário
        if (player.Equipment[slot] == null) return;
        if (player.Equipment[slot].Bind == BindOn.Equip) return;

        // Adiciona o equipamento ao inventário
        if (!player.GiveItem(player.Equipment[slot], 1))
        {
            // Somente se necessário
            if (player.Map.Item.Count == MaxMapItems) return;

            // Solta o item no chão
            player.Map.Item.Add(new TempMapItems(player.Equipment[slot], 1, player.X, player.Y));

            // Envia os dados
            MapSender.MapItems(player.Map);
            PlayerSender.PlayerInventory(player);
        }

        // Remove o equipamento
        for (byte i = 0; i < (byte)Attribute.Count; i++)
            player.Attribute[i] -= player.Equipment[slot].EquipAttribute[i];
        player.Equipment[slot] = null;

        // Envia os dados
        PlayerSender.PlayerEquipments(player);
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
            case SlotType.Item: player.UseItem(player.Hotbar[hotbarSlot].Slot); break;
        }
    }
}