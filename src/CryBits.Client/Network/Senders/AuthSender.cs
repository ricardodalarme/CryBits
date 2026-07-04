using CryBits.Client.Framework.Network;
using CryBits.Protocol.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class AuthSender(Connection connection)
{
    public void Connect(string username, string password) => connection.SendPacket(new ConnectPacket
    {
        Username = username,
        Password = password,
        IsClientAccess = false
    });

    public void Register(string username, string password) => connection.SendPacket(new RegisterPacket
    {
        Username = username,
        Password = password
    });
}
