using CryBits.Client.Framework.Network;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Client.Network.Senders;

internal class ContentSender(Connection connection)
{
    public static ContentSender Instance { get; } = new(Connection.Instance);

    public void RequestMap(bool order) =>
        connection.SendPacket(new RequestMapPacket { SendMap = order }, DeliveryChannel.ReliableUnordered);
}
