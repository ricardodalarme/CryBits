using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.State;
using CryBits.Host.Core;
using CryBits.Transport.Abstractions;
using CryBits.Transport;

namespace CryBits.Host.Network;

internal sealed class PackageSender(ITransport transport, SessionManager sessions, EntityRegistry entities)
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
            var entity = entities.Get(characterId);
            var pos = entity?.Get<Position>();
            if (pos?.MapId == mapId)
                transport.Send(t.Id, bytes, delivery);
        }
    }
}
