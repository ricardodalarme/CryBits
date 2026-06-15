using CryBits.Transport;
using CryBits.Transport.Packets.Server;
using CryBits.Simulation.Components;
using LiteNetLib;
using System;
using System.Linq;
using CryBits.Simulation.State;
using CryBits.Host.Core;
using CryBits.Transport.Abstractions;

namespace CryBits.Host.Network;

internal sealed class PackageSender(ITransport transport, SessionManager sessions, EntityRegistry entities)
{
    public void ToPlayer(Session session, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);
        transport.Send(session.Id, bytes, delivery);
    }

    public void ToPlayer(EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var session = sessions.Get(entityId)!;
        ToPlayer(session, packet, delivery);
    }

    public void ToAll(IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        foreach (var t in sessions.Where(t => t.IsPlaying))
            transport.Send(t.Id, bytes, delivery);
    }

    public void ToAllBut(EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        foreach (var t in sessions.Where(t => t.IsPlaying && t.Character.HasValue && !t.Character.Value.Equals(entityId)))
            transport.Send(t.Id, bytes, delivery);
    }

    public void ToMap(Guid mapId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        foreach (var t in sessions.Where(t => t.IsPlaying && t.Character.HasValue))
        {
            var entity = entities.Get(t.Character.Value);
            var pos = entity?.Get<Position>();
            if (pos?.MapId == mapId)
                transport.Send(t.Id, bytes, delivery);
        }
    }

    public void ToMapBut(Guid mapId, EntityId entityId, IServerPacket packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        foreach (var t in sessions.Where(t => t.IsPlaying && t.Character.HasValue))
        {
            var cid = t.Character.Value;
            if (cid.Equals(entityId)) continue;

            var entity = entities.Get(cid);
            var pos = entity?.Get<Position>();
            if (pos?.MapId == mapId)
                transport.Send(t.Id, bytes, delivery);
        }
    }
}
