using System.Linq;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Server.Network;

internal static class Send
{
    public static void ToPlayer(Account account, ServerPacket packetId, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.Put((byte)packetId);
        data.WriteObject(packet);
        account.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void ToPlayer(Player player, ServerPacket packetId, IServerPacket packet) =>
        ToPlayer(player.Account, packetId, packet);

    public static void ToAll(ServerPacket packetId, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.Put((byte)packetId);
        data.WriteObject(packet);

        foreach (var t in Account.List.Where(t => t.IsPlaying))
            t.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void ToAllBut(Player player, ServerPacket packetId, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.Put((byte)packetId);
        data.WriteObject(packet);

        // Send to all connected accounts except the player's account.
        foreach (var t in Account.List.Where(t => t.IsPlaying).Where(t => player != t.Character))
            ToPlayer(t, packetId, packet);
    }

    public static void ToMap(TempMap map, ServerPacket packetId, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.Put((byte)packetId);
        data.WriteObject(packet);

        // Send to all players on the specified map.
        foreach (var t in Account.List.Where(t => t.IsPlaying).Where(t => t.Character.Map == map))
            ToPlayer(t, packetId, packet);
    }

    public static void ToMapBut(TempMap map, Player player, ServerPacket packetId, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.Put((byte)packetId);
        data.WriteObject(packet);

        // Send to all players on the map except the specified player.
        foreach (var t in Account.List.Where(t => t.IsPlaying)
                     .Where(t => t.Character.Map == map)
                     .Where(t => player != t.Character))
            ToPlayer(t, packetId, packet);
    }
}
