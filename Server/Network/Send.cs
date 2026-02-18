using System.Drawing;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using LiteNetLib;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network;

internal static class Send
{
    private static void ToPlayer(Account account, NetDataWriter data)
    {
        account.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    private static void ToPlayer(Player player, NetDataWriter data)
    {
        ToPlayer(player.Account, data);
    }

    private static void ToAll(NetDataWriter data)
    {
        // Envia os dados para todos conectados
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                ToPlayer(Account.List[i].Character, data);
    }

    private static void ToAllBut(Player player, NetDataWriter data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (player != Account.List[i].Character)
                    ToPlayer(Account.List[i].Character, data);
    }

    private static void ToMap(TempMap map, NetDataWriter data)
    {
        // Envia os dados para todos conectados, com excessão do índice
        for (byte i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (Account.List[i].Character.Map == map)
                    ToPlayer(Account.List[i].Character, data);
    }

    private static void ToMapBut(TempMap map, Player player, NetDataWriter data)
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
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Alert);
        data.Put(message);
        ToPlayer(account, data);

        // Desconecta o jogador
        if (disconnect) account.Connection.Disconnect();
    }

    public static void Connect(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Connect);
        ToPlayer(account, data);
    }

    public static void CreateCharacter(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.CreateCharacter);
        ToPlayer(account, data);
    }

    public static void Join(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Join);
        data.Put(player.Name);
        ToPlayer(player, data);
    }

    public static void Characters(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Characters);
        data.Put((byte)account.Characters.Count);

        for (byte i = 0; i < account.Characters.Count; i++)
        {
            data.Put(account.Characters[i].Name);
            data.Put(account.Characters[i].TextureNum);
        }

        ToPlayer(account, data);
    }

    public static void Classes(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Classes);
        data.WriteObject(Class.List);
        ToPlayer(account, data);
    }

    public static void JoinGame(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.JoinGame);
        ToPlayer(player, data);
    }

    private static NetDataWriter PlayerDataCache(Player player)
    {
        var data = new NetDataWriter();

        // Escreve os dados
        data.Put((byte)ServerPacket.PlayerData);
        data.Put(player.Name);
        data.Put(player.TextureNum);
        data.Put(player.Level);
        data.PutGuid(player.Map.GetId());
        data.Put(player.X);
        data.Put(player.Y);
        data.Put((byte)player.Direction);
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            data.Put(player.Vital[n]);
            data.Put(player.MaxVital(n));
        }
        for (byte n = 0; n < (byte)Attribute.Count; n++) data.Put(player.Attribute[n]);
        for (byte n = 0; n < (byte)Equipment.Count; n++) data.PutGuid(player.Equipment[n].GetId());

        return data;
    }

    public static void PlayerPosition(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerPosition);
        data.Put(player.Name);
        data.Put(player.X);
        data.Put(player.Y);
        data.Put((byte)player.Direction);
        ToMap(player.Map, data);
    }

    public static void PlayerVitals(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerVitals);
        data.Put(player.Name);
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            data.Put(player.Vital[i]);
            data.Put(player.MaxVital(i));
        }

        ToMap(player.Map, data);
    }

    public static void PlayerLeave(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerLeave);
        data.Put(player.Name);
        ToAllBut(player, data);
    }

    public static void PlayerMove(Player player, byte movement)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerMove);
        data.Put(player.Name);
        data.Put(player.X);
        data.Put(player.Y);
        data.Put((byte)player.Direction);
        data.Put(movement);
        ToMapBut(player.Map, player, data);
    }

    public static void PlayerDirection(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerDirection);
        data.Put(player.Name);
        data.Put((byte)player.Direction);
        ToMapBut(player.Map, player, data);
    }

    public static void PlayerExperience(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerExperience);
        data.Put(player.Experience);
        data.Put(player.ExpNeeded);
        data.Put(player.Points);
        ToPlayer(player, data);
    }

    public static void PlayerEquipments(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerEquipments);
        data.Put(player.Name);
        for (byte i = 0; i < (byte)Equipment.Count; i++) data.PutGuid(player.Equipment[i].GetId());
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
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.JoinMap);
        ToPlayer(player, data);
    }

    public static void PlayerLeaveMap(Player player, TempMap map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerLeave);
        data.Put(player.Name);
        ToMapBut(map, player, data);
    }

    public static void MapRevision(Player player, Map map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapRevision);
        data.PutGuid(map.GetId());
        data.Put(map.Revision);
        ToPlayer(player, data);
    }

    public static void Maps(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Maps);
        data.Put((short)CryBits.Entities.Map.Map.List.Count);
        ToPlayer(account, data);
        foreach (var map in CryBits.Entities.Map.Map.List.Values) Map(account, map);
    }

    public static void Map(Account account, Map map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Map);
        data.WriteObject(map);
        ToPlayer(account, data);
    }

    public static void Latency(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Latency);
        ToPlayer(account, data);
    }

    public static void Message(Player player, string text, Color color)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Message);
        data.Put(text);
        data.Put(color.ToArgb());
        ToPlayer(player, data);
    }

    public static void MessageMap(Player player, string text)
    {
        var data = new NetDataWriter();
        var message = "[Map] " + player.Name + ": " + text;

        // Envia os dados
        data.Put((byte)ServerPacket.Message);
        data.Put(message);
        data.Put(Color.White.ToArgb());
        ToMap(player.Map, data);
    }

    public static void MessageGlobal(Player player, string text)
    {
        var data = new NetDataWriter();
        var message = "[Global] " + player.Name + ": " + text;

        // Envia os dados
        data.Put((byte)ServerPacket.Message);
        data.Put(message);
        data.Put(Color.Yellow.ToArgb());
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
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerAttack);
        data.Put(player.Name);
        data.Put(victim);
        data.Put((byte)victimType);
        ToMap(player.Map, data);
    }

    public static void Items(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Items);
        data.WriteObject(Item.List);
        ToPlayer(account, data);
    }

    public static void MapItems(Player player, TempMap map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapItems);
        data.Put((byte)map.Item.Count);

        for (byte i = 0; i < map.Item.Count; i++)
        {
            // Geral
            data.PutGuid(map.Item[i].Item.GetId());
            data.Put(map.Item[i].X);
            data.Put(map.Item[i].Y);
        }

        // Envia os dados
        ToPlayer(player, data);
    }

    public static void MapItems(TempMap map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapItems);
        data.Put((byte)map.Item.Count);
        for (byte i = 0; i < map.Item.Count; i++)
        {
            data.PutGuid(map.Item[i].Item.GetId());
            data.Put(map.Item[i].X);
            data.Put(map.Item[i].Y);
        }
        ToMap(map, data);
    }

    public static void PlayerInventory(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerInventory);
        for (byte i = 0; i < MaxInventory; i++)
        {
            data.PutGuid(player.Inventory[i].Item.GetId());
            data.Put(player.Inventory[i].Amount);
        }
        ToPlayer(player, data);
    }

    public static void PlayerHotbar(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerHotbar);
        for (byte i = 0; i < MaxHotbar; i++)
        {
            data.Put((byte)player.Hotbar[i].Type);
            data.Put(player.Hotbar[i].Slot);
        }
        ToPlayer(player, data);
    }

    public static void Npcs(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Npcs);
        data.WriteObject(Npc.List);
        ToPlayer(account, data);
    }

    public static void MapNpcs(Player player, TempMap map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcs);
        data.Put((short)map.Npc.Length);
        for (byte i = 0; i < map.Npc.Length; i++)
        {
            data.PutGuid(map.Npc[i].Data.GetId());
            data.Put(map.Npc[i].X);
            data.Put(map.Npc[i].Y);
            data.Put((byte)map.Npc[i].Direction);
            for (byte n = 0; n < (byte)Vital.Count; n++) data.Put(map.Npc[i].Vital[n]);
        }
        ToPlayer(player, data);
    }

    public static void MapNpc(TempNpc npc)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpc);
        data.Put(npc.Index);
        data.PutGuid(npc.Data.GetId());
        data.Put(npc.X);
        data.Put(npc.Y);
        data.Put((byte)npc.Direction);
        for (byte n = 0; n < (byte)Vital.Count; n++) data.Put(npc.Vital[n]);
        ToMap(npc.Map, data);
    }

    public static void MapNpcMovement(TempNpc npc, byte movement)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcMovement);
        data.Put(npc.Index);
        data.Put(npc.X);
        data.Put(npc.Y);
        data.Put((byte)npc.Direction);
        data.Put(movement);
        ToMap(npc.Map, data);
    }

    public static void MapNpcDirection(TempNpc npc)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcDirection);
        data.Put(npc.Index);
        data.Put((byte)npc.Direction);
        ToMap(npc.Map, data);
    }

    public static void MapNpcVitals(TempNpc npc)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcVitals);
        data.Put(npc.Index);
        for (byte n = 0; n < (byte)Vital.Count; n++) data.Put(npc.Vital[n]);
        ToMap(npc.Map, data);
    }

    public static void MapNpcAttack(TempNpc npc, string victim = "", Target victimType = 0)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcAttack);
        data.Put(npc.Index);
        data.Put(victim);
        data.Put((byte)victimType);
        ToMap(npc.Map, data);
    }

    public static void MapNpcDied(TempNpc npc)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcDied);
        data.Put(npc.Index);
        ToMap(npc.Map, data);
    }

    public static void ServerData(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.ServerData);
        data.Put(GameName);
        data.Put(WelcomeMessage);
        data.Put(Port);
        data.Put(MaxPlayers);
        data.Put(MaxCharacters);
        data.Put(MaxPartyMembers);
        data.Put(MaxMapItems);
        data.Put(NumPoints);
        data.Put(MinNameLength);
        data.Put(MaxNameLength);
        data.Put(MinPasswordLength);
        data.Put(MaxPasswordLength);
        ToPlayer(account, data);
    }

    public static void Party(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Party);
        data.Put((byte)player.Party.Count);
        for (byte i = 0; i < player.Party.Count; i++) data.Put(player.Party[i].Name);
        ToPlayer(player, data);
    }

    public static void PartyInvitation(Player player, string playerInvitation)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PartyInvitation);
        data.Put(playerInvitation);
        ToPlayer(player, data);
    }

    public static void Trade(Player player, bool state)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Trade);
        data.Put(state);
        ToPlayer(player, data);
    }

    public static void TradeInvitation(Player player, string playerInvitation)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.TradeInvitation);
        data.Put(playerInvitation);
        ToPlayer(player, data);
    }

    public static void TradeState(Player player, TradeStatus state)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.TradeState);
        data.Put((byte)state);
        ToPlayer(player, data);
    }

    public static void TradeOffer(Player player, bool own = true)
    {
        var data = new NetDataWriter();
        var to = own ? player : player.Trade;

        // Envia os dados
        data.Put((byte)ServerPacket.TradeOffer);
        data.Put(own);
        for (byte i = 0; i < MaxInventory; i++)
        {
            data.PutGuid(to.Inventory[to.TradeOffer[i].SlotNum].Item.GetId());
            data.Put(to.TradeOffer[i].Amount);
        }
        ToPlayer(player, data);
    }

    public static void Shops(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Shops);
        data.WriteObject(Shop.List);
        ToPlayer(account, data);
    }

    public static void ShopOpen(Player player, Shop shop)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.ShopOpen);
        data.PutGuid(shop.GetId());
        ToPlayer(player, data);
    }
}