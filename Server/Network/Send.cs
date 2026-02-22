using System.Linq;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Server.Network;

internal static class Send
{
    public static void ToPlayer(Account account, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        account.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void ToPlayer(Player player, IServerPacket packet) =>
        ToPlayer(player.Account, packet);

    public static void ToAll(IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Accounts.Where(t => t.IsPlaying))
            t.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void ToAllBut(Player player, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Accounts.Where(t => t.IsPlaying).Where(t => player != t.Character))
            ToPlayer(t, packet);
    }

    public static void ToMap(MapInstance mapInstance, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Accounts.Where(t => t.IsPlaying).Where(t => t.Character.MapInstance == mapInstance))
            ToPlayer(t, packet);
    }

    public static void ToMapBut(MapInstance mapInstance, Player player, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Accounts.Where(t => t.IsPlaying)
                     .Where(t => t.Character.MapInstance == mapInstance)
                     .Where(t => player != t.Character))
            ToPlayer(t, packet);
    }
}
