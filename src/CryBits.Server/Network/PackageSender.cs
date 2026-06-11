using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Linq;

namespace CryBits.Server.Network;

internal sealed class PackageSender
{
    public static PackageSender Instance { get; } = new();

    public void ToPlayer(GameSession session, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        session.Connection.Send(data, delivery);
    }

    public void ToPlayer(Player player, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) =>
        ToPlayer(player.Session, packet, delivery);

    public void ToAll(IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying))
            t.Connection.Send(data, delivery);
    }

    public void ToAllBut(Player player, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying).Where(t => player != t.Character))
            t.Connection.Send(data, delivery);
    }

    public void ToMap(Guid mapId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying)
                     .Where(t => t.Character!.MapInstance.Id == mapId))
            t.Connection.Send(data, delivery);
    }

    public void ToMapBut(Guid mapId, Player player, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying)
                     .Where(t => t.Character!.MapInstance.Id == mapId)
                     .Where(t => player != t.Character))
            t.Connection.Send(data, delivery);
    }
}
