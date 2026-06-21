using CryBits.Client.Framework.Network;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Client.Network.Senders;

internal class MapSender(Connection connection)
{
    public static MapSender Instance { get; } = new(Connection.Instance);

    public void RequestMap(bool order) =>
        connection.SendPacket(new RequestMapPacket { SendMap = order }, DeliveryChannel.ReliableUnordered);
}
