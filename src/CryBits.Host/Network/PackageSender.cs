using CryBits.Host.Core;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Transport;
using CryBits.Transport.Abstractions;

namespace CryBits.Host.Network;

public sealed class PackageSender(ITransport transport, SessionManager sessions, EntityRegistry entities)
{
    public void ToPlayer(Session session, IServerPacket packet, DeliveryChannel delivery = DeliveryChannel.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);
        transport.Send(session.Id, bytes, delivery);
    }

    public void ToPlayer(EntityId entityId, IServerPacket packet, DeliveryChannel delivery = DeliveryChannel.ReliableOrdered)
    {
        var session = sessions.Get(entityId)!;
        ToPlayer(session, packet, delivery);
    }

    public void ToAll(IServerPacket packet, DeliveryChannel delivery = DeliveryChannel.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        foreach (var t in sessions.Where(t => t.IsPlaying))
            transport.Send(t.Id, bytes, delivery);
    }

    public void ToMap(Guid mapId, IServerPacket packet, DeliveryChannel delivery = DeliveryChannel.ReliableOrdered)
    {
        var bytes = PacketSerializer.Serialize(packet);

        foreach (var t in sessions.Where(t => t.IsPlaying && t.Character.HasValue))
        {
            var characterId = t.Character!.Value;
            var pos = entities.Get<Position>(characterId);
            if (pos?.MapId == mapId)
                transport.Send(t.Id, bytes, delivery);
        }
    }
}
