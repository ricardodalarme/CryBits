using CryBits.Entities.Map;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class MapSender
{
    public static void Map(Account account, Map map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Map);
        data.WriteObject(map);
        Send.ToPlayer(account, data);
    }

    public static void Maps(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Maps);
        data.Put((short)CryBits.Entities.Map.Map.List.Count);
        Send.ToPlayer(account, data);
        foreach (var map in CryBits.Entities.Map.Map.List.Values) Map(account, map);
    }

    public static void MapRevision(Player player, Map map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapRevision);
        data.Put(map.GetId());
        data.Put(map.Revision);
        Send.ToPlayer(player, data);
    }

    public static void MapPlayers(Player player)
    {
        // Envia os dados dos outros jogadores 
        for (var i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (player != Account.List[i].Character)
                    if (Account.List[i].Character.Map == player.Map)
                        Send.ToPlayer(player, PlayerDataCache(Account.List[i].Character));

        // Envia os dados do jogador
        Send.ToMap(player.Map, PlayerDataCache(player));
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
            data.Put(map.Item[i].Item.GetId());
            data.Put(map.Item[i].X);
            data.Put(map.Item[i].Y);
        }

        // Envia os dados
        Send.ToPlayer(player, data);
    }

    public static void MapItems(TempMap map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapItems);
        data.Put((byte)map.Item.Count);
        for (byte i = 0; i < map.Item.Count; i++)
        {
            data.Put(map.Item[i].Item.GetId());
            data.Put(map.Item[i].X);
            data.Put(map.Item[i].Y);
        }

        Send.ToMap(map, data);
    }

    private static NetDataWriter PlayerDataCache(Player player)
    {
        var data = new NetDataWriter();

        // Escreve os dados
        data.Put((byte)ServerPacket.PlayerData);
        data.Put(player.Name);
        data.Put(player.TextureNum);
        data.Put(player.Level);
        data.Put(player.Map.GetId());
        data.Put(player.X);
        data.Put(player.Y);
        data.Put((byte)player.Direction);
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            data.Put(player.Vital[n]);
            data.Put(player.MaxVital(n));
        }

        for (byte n = 0; n < (byte)Attribute.Count; n++) data.Put(player.Attribute[n]);
        for (byte n = 0; n < (byte)Equipment.Count; n++) data.Put(player.Equipment[n].GetId());

        return data;
    }
}
