using System.Drawing;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using Lidgren.Network;
using static CryBits.Globals;
using static CryBits.Utils;

namespace CryBits.Server.Network;

internal static class Send
{
    private static void ToPlayer(Account account, NetOutgoingMessage data)
    {
        // Recria o pacote e o envia
        var dataSend = Socket.Device.CreateMessage(data.LengthBytes);
        dataSend.Write(data);
        Socket.Device.SendMessage(dataSend, account.Connection, NetDeliveryMethod.ReliableOrdered);
    }

    private static void ToPlayer(Player player, NetOutgoingMessage data)
    {
        ToPlayer(player.Account, data);
    }

    private static void ToAll(NetOutgoingMessage data)
    {
        // Envia os dados para todos conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                ToPlayer(Account.List[i].Character, data);
    }

    private static void ToAllBut(Player player, NetOutgoingMessage data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (player != Account.List[i].Character)
                    ToPlayer(Account.List[i].Character, data);
    }

    private static void ToMap(TempMap map, NetOutgoingMessage data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (Account.List[i].Character.Map == map)
                    ToPlayer(Account.List[i].Character, data);
    }

    private static void ToMapBut(TempMap map, Player player, NetOutgoingMessage data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (Account.List[i].Character.Map == map)
                    if (player != Account.List[i].Character)
                        ToPlayer(Account.List[i].Character, data);
    }

    public static void Alert(Account account, string message, bool disconnect = true)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Alert);
        data.Write(message);
        ToPlayer(account, data);

        // Desconecta o jogador
        if (disconnect) account.Connection.Disconnect(string.Empty);
    }

    public static void Connect(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Connect);
        ToPlayer(account, data);
    }

    public static void CreateCharacter(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.CreateCharacter);
        ToPlayer(account, data);
    }

    public static void Join(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Join);
        data.Write(player.Name);
        ToPlayer(player, data);
    }

    public static void Characters(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Characters);
        data.Write((byte)account.Characters.Count);

        for (byte i = 0; i < account.Characters.Count; i++)
        {
            data.Write(account.Characters[i].Name);
            data.Write(account.Characters[i].TextureNum);
        }

        ToPlayer(account, data);
    }

    public static void Classes(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Classes);
        ObjectToByteArray(data, Class.List);
        ToPlayer(account, data);
    }

    public static void JoinGame(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.JoinGame);
        ToPlayer(player, data);
    }

    private static NetOutgoingMessage PlayerDataCache(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Escreve os dados
        data.Write((byte)ServerPacket.PlayerData);
        data.Write(player.Name);
        data.Write(player.TextureNum);
        data.Write(player.Level);
        data.Write(player.Map.GetId());
        data.Write(player.X);
        data.Write(player.Y);
        data.Write((byte)player.Direction);
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            data.Write(player.Vital[n]);
            data.Write(player.MaxVital(n));
        }
        for (byte n = 0; n < (byte)Attribute.Count; n++) data.Write(player.Attribute[n]);
        for (byte n = 0; n < (byte)Equipment.Count; n++) data.Write(player.Equipment[n].GetId());

        return data;
    }

    public static void PlayerPosition(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerPosition);
        data.Write(player.Name);
        data.Write(player.X);
        data.Write(player.Y);
        data.Write((byte)player.Direction);
        ToMap(player.Map, data);
    }

    public static void PlayerVitals(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerVitals);
        data.Write(player.Name);
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            data.Write(player.Vital[i]);
            data.Write(player.MaxVital(i));
        }

        ToMap(player.Map, data);
    }

    public static void PlayerLeave(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerLeave);
        data.Write(player.Name);
        ToAllBut(player, data);
    }

    public static void PlayerMove(Player player, byte movement)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerMove);
        data.Write(player.Name);
        data.Write(player.X);
        data.Write(player.Y);
        data.Write((byte)player.Direction);
        data.Write(movement);
        ToMapBut(player.Map, player, data);
    }

    public static void PlayerDirection(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerDirection);
        data.Write(player.Name);
        data.Write((byte)player.Direction);
        ToMapBut(player.Map, player, data);
    }

    public static void PlayerExperience(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerExperience);
        data.Write(player.Experience);
        data.Write(player.ExpNeeded);
        data.Write(player.Points);
        ToPlayer(player, data);
    }

    public static void PlayerEquipments(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerEquipments);
        data.Write(player.Name);
        for (byte i = 0; i < (byte)Equipment.Count; i++) data.Write(player.Equipment[i].GetId());
        ToMap(player.Map, data);
    }

    public static void MapPlayers(Player player)
    {
        // Envia os dados dos outros jogadores 
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (player != Account.List[i].Character)
                    if (Account.List[i].Character.Map == player.Map)
                        ToPlayer(player, PlayerDataCache(Account.List[i].Character));

        // Envia os dados do jogador
        ToMap(player.Map, PlayerDataCache(player));
    }

    public static void JoinMap(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.JoinMap);
        ToPlayer(player, data);
    }

    public static void PlayerLeaveMap(Player player, TempMap map)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerLeave);
        data.Write(player.Name);
        ToMapBut(map, player, data);
    }

    public static void MapRevision(Player player, Map map)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapRevision);
        data.Write(map.GetId());
        data.Write(map.Revision);
        ToPlayer(player, data);
    }

    public static void Maps(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Maps);
        data.Write((short)CryBits.Entities.Map.Map.List.Count);
        ToPlayer(account, data);
        foreach (var map in CryBits.Entities.Map.Map.List.Values) Map(account, map);
    }

    public static void Map(Account account, Map map)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Map);
        ObjectToByteArray(data, map);
        ToPlayer(account, data);
    }

    public static void Latency(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Latency);
        ToPlayer(account, data);
    }

    public static void Message(Player player, string text, Color color)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Message);
        data.Write(text);
        data.Write(color.ToArgb());
        ToPlayer(player, data);
    }

    public static void MessageMap(Player player, string text)
    {
        var data = Socket.Device.CreateMessage();
        var message = "[Map] " + player.Name + ": " + text;

        // Envia os dados
        data.Write((byte)ServerPacket.Message);
        data.Write(message);
        data.Write(Color.White.ToArgb());
        ToMap(player.Map, data);
    }

    public static void MessageGlobal(Player player, string text)
    {
        var data = Socket.Device.CreateMessage();
        var message = "[Global] " + player.Name + ": " + text;

        // Envia os dados
        data.Write((byte)ServerPacket.Message);
        data.Write(message);
        data.Write(Color.Yellow.ToArgb());
        ToAll(data);
    }

    public static void MessagePrivate(Player player, string addresseeName, string text)
    {
        var addressee = Player.Find(addresseeName);

        // Verifica se o jogador está conectado
        if (addressee == null)
        {
            Message(player, addresseeName + " is currently offline.", Color.Blue);
            return;
        }

        // Envia as mensagens
        Message(player, "[To] " + addresseeName + ": " + text, Color.Pink);
        Message(addressee, "[From] " + player.Name + ": " + text, Color.Pink);
    }

    public static void PlayerAttack(Player player, string victim = "", Target victimType = 0)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerAttack);
        data.Write(player.Name);
        data.Write(victim);
        data.Write((byte)victimType);
        ToMap(player.Map, data);
    }

    public static void Items(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Items);
        ObjectToByteArray(data, Item.List);
        ToPlayer(account, data);
    }

    public static void MapItems(Player player, TempMap map)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapItems);
        data.Write((byte)map.Item.Count);

        for (byte i = 0; i < map.Item.Count; i++)
        {
            // Geral
            data.Write(map.Item[i].Item.GetId());
            data.Write(map.Item[i].X);
            data.Write(map.Item[i].Y);
        }

        // Envia os dados
        ToPlayer(player, data);
    }

    public static void MapItems(TempMap map)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapItems);
        data.Write((byte)map.Item.Count);
        for (byte i = 0; i < map.Item.Count; i++)
        {
            data.Write(map.Item[i].Item.GetId());
            data.Write(map.Item[i].X);
            data.Write(map.Item[i].Y);
        }
        ToMap(map, data);
    }

    public static void PlayerInventory(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerInventory);
        for (byte i = 0; i < MaxInventory; i++)
        {
            data.Write(player.Inventory[i].Item.GetId());
            data.Write(player.Inventory[i].Amount);
        }
        ToPlayer(player, data);
    }

    public static void PlayerHotbar(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PlayerHotbar);
        for (byte i = 0; i < MaxHotbar; i++)
        {
            data.Write((byte)player.Hotbar[i].Type);
            data.Write(player.Hotbar[i].Slot);
        }
        ToPlayer(player, data);
    }

    public static void Npcs(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Npcs);
        ObjectToByteArray(data, Npc.List);
        ToPlayer(account, data);
    }

    public static void MapNpcs(Player player, TempMap map)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapNpcs);
        data.Write((short)map.Npc.Length);
        for (byte i = 0; i < map.Npc.Length; i++)
        {
            data.Write(map.Npc[i].Data.GetId());
            data.Write(map.Npc[i].X);
            data.Write(map.Npc[i].Y);
            data.Write((byte)map.Npc[i].Direction);
            for (byte n = 0; n < (byte)Vital.Count; n++) data.Write(map.Npc[i].Vital[n]);
        }
        ToPlayer(player, data);
    }

    public static void MapNpc(TempNpc npc)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapNpc);
        data.Write(npc.Index);
        data.Write(npc.Data.GetId());
        data.Write(npc.X);
        data.Write(npc.Y);
        data.Write((byte)npc.Direction);
        for (byte n = 0; n < (byte)Vital.Count; n++) data.Write(npc.Vital[n]);
        ToMap(npc.Map, data);
    }

    public static void MapNpcMovement(TempNpc npc, byte movement)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapNpcMovement);
        data.Write(npc.Index);
        data.Write(npc.X);
        data.Write(npc.Y);
        data.Write((byte)npc.Direction);
        data.Write(movement);
        ToMap(npc.Map, data);
    }

    public static void MapNpcDirection(TempNpc npc)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapNpcDirection);
        data.Write(npc.Index);
        data.Write((byte)npc.Direction);
        ToMap(npc.Map, data);
    }

    public static void MapNpcVitals(TempNpc npc)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapNpcVitals);
        data.Write(npc.Index);
        for (byte n = 0; n < (byte)Vital.Count; n++) data.Write(npc.Vital[n]);
        ToMap(npc.Map, data);
    }

    public static void MapNpcAttack(TempNpc npc, string victim = "", Target victimType = 0)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapNpcAttack);
        data.Write(npc.Index);
        data.Write(victim);
        data.Write((byte)victimType);
        ToMap(npc.Map, data);
    }

    public static void MapNpcDied(TempNpc npc)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.MapNpcDied);
        data.Write(npc.Index);
        ToMap(npc.Map, data);
    }

    public static void ServerData(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.ServerData);
        data.Write(GameName);
        data.Write(WelcomeMessage);
        data.Write(Port);
        data.Write(MaxPlayers);
        data.Write(MaxCharacters);
        data.Write(MaxPartyMembers);
        data.Write(MaxMapItems);
        data.Write(NumPoints);
        data.Write(MinNameLength);
        data.Write(MaxNameLength);
        data.Write(MinPasswordLength);
        data.Write(MaxPasswordLength);
        ToPlayer(account, data);
    }

    public static void Party(Player player)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Party);
        data.Write((byte)player.Party.Count);
        for (byte i = 0; i < player.Party.Count; i++) data.Write(player.Party[i].Name);
        ToPlayer(player, data);
    }

    public static void PartyInvitation(Player player, string playerInvitation)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.PartyInvitation);
        data.Write(playerInvitation);
        ToPlayer(player, data);
    }

    public static void Trade(Player player, bool state)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Trade);
        data.Write(state);
        ToPlayer(player, data);
    }

    public static void TradeInvitation(Player player, string playerInvitation)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.TradeInvitation);
        data.Write(playerInvitation);
        ToPlayer(player, data);
    }

    public static void TradeState(Player player, TradeStatus state)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.TradeState);
        data.Write((byte)state);
        ToPlayer(player, data);
    }

    public static void TradeOffer(Player player, bool own = true)
    {
        var data = Socket.Device.CreateMessage();
        var to = own ? player : player.Trade;

        // Envia os dados
        data.Write((byte)ServerPacket.TradeOffer);
        data.Write(own);
        for (byte i = 0; i < MaxInventory; i++)
        {
            data.Write(to.Inventory[to.TradeOffer[i].SlotNum].Item.GetId());
            data.Write(to.TradeOffer[i].Amount);
        }
        ToPlayer(player, data);
    }

    public static void Shops(Account account)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.Shops);
        ObjectToByteArray(data, Shop.List);
        ToPlayer(account, data);
    }

    public static void ShopOpen(Player player, Shop shop)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ServerPacket.ShopOpen);
        data.Write(shop.GetId());
        ToPlayer(player, data);
    }
}