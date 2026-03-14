using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Linq;

namespace CryBits.Server.Network;

internal sealed class PackageSender
{
    public static PackageSender Instance { get; } = new();

    public void ToPlayer(GameSession session, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        session.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public void ToPlayer(Player player, IServerPacket packet) =>
        ToPlayer(player.Session, packet);

    public void ToAll(IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying))
            t.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public void ToAllBut(Player player, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying).Where(t => player != t.Character))
            ToPlayer(t, packet);
    }

    public void ToMap(MapInstance mapInstance, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying)
                     .Where(t => t.Character!.MapInstance == mapInstance))
            ToPlayer(t, packet);
    }

    public void ToMapBut(MapInstance mapInstance, Player player, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying)
                     .Where(t => t.Character!.MapInstance == mapInstance)
                     .Where(t => player != t.Character))
            ToPlayer(t, packet);
    }
}
