using CryBits.Transport;
using CryBits.Transport.Packets.Server;
using CryBits.Simulation.Components;
using LiteNetLib;
using System;
using System.Linq;
using CryBits.Simulation.State;
using CryBits.Host.Core;

namespace CryBits.Host.Network;

internal sealed class PackageSender
{
    public static PackageSender Instance { get; } = new();

    public void ToPlayer(Session session, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);
        WorldHost.Current.Transport.Send(session.Id, bytes, delivery);
    }

    public void ToPlayer(EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var session = WorldHost.Current.Sessions.Get(entityId)!;
        ToPlayer(session, packet, delivery);
    }

    public void ToAll(IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        foreach (var t in WorldHost.Current.Sessions.Where(t => t.IsPlaying))
            WorldHost.Current.Transport.Send(t.Id, bytes, delivery);
    }

    public void ToAllBut(EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        foreach (var t in WorldHost.Current.Sessions.Where(t => t.IsPlaying && t.Character.HasValue && !t.Character.Value.Equals(entityId)))
            WorldHost.Current.Transport.Send(t.Id, bytes, delivery);
    }

    public void ToMap(Guid mapId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        var world = WorldHost.Current;
        foreach (var t in world.Sessions.Where(t => t.IsPlaying && t.Character.HasValue))
        {
            var entity = world.Entities.Get(t.Character.Value);
            var pos = entity?.Get<Position>();
            if (pos?.MapId == mapId)
                WorldHost.Current.Transport.Send(t.Id, bytes, delivery);
        }
    }

    public void ToMapBut(Guid mapId, EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        var world = WorldHost.Current;
        foreach (var t in world.Sessions.Where(t => t.IsPlaying && t.Character.HasValue))
        {
            var cid = t.Character.Value;
            if (cid.Equals(entityId)) continue;

            var entity = world.Entities.Get(cid);
            var pos = entity?.Get<Position>();
            if (pos?.MapId == mapId)
                WorldHost.Current.Transport.Send(t.Id, bytes, delivery);
        }
    }
}
