using CryBits.Network;
using CryBits.Network.Packets.Server;
using CryBits.Simulation.Components;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Linq;
using CryBits.Simulation.State;
using CryBits.Host.Core;

namespace CryBits.Host.Network;

internal sealed class PackageSender
{
    public static PackageSender Instance { get; } = new();

    public void ToPlayer(GameSession session, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        session.Connection.Send(data, delivery);
    }

    public void ToPlayer(EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var session = WorldHost.Current.SessionMap.Get(entityId)!;
        ToPlayer(session, packet, delivery);
    }

    public void ToAll(IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in WorldHost.Current.Sessions.Where(t => t.IsPlaying))
            t.Connection.Send(data, delivery);
    }

    public void ToAllBut(EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in WorldHost.Current.Sessions.Where(t => t.IsPlaying && t.Character.HasValue && !t.Character.Value.Equals(entityId)))
            t.Connection.Send(data, delivery);
    }

    public void ToMap(Guid mapId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        var world = WorldHost.Current;
        foreach (var t in world.Sessions.Where(t => t.IsPlaying && t.Character.HasValue))
        {
            var entity = world.Entities.Get(t.Character.Value);
            var pos = entity?.Get<Position>();
            if (pos?.MapId == mapId)
                t.Connection.Send(data, delivery);
        }
    }

    public void ToMapBut(Guid mapId, EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        var world = WorldHost.Current;
        foreach (var t in world.Sessions.Where(t => t.IsPlaying && t.Character.HasValue))
        {
            var cid = t.Character.Value;
            if (cid.Equals(entityId)) continue;

            var entity = world.Entities.Get(cid);
            var pos = entity?.Get<Position>();
            if (pos?.MapId == mapId)
                t.Connection.Send(data, delivery);
        }
    }
}
