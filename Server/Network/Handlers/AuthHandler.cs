using CryBits.Packets.Client;
using CryBits.Server.Systems;
using CryBits.Server.World;

namespace CryBits.Server.Network.Handlers;

internal sealed class AuthHandler(AuthSystem authSystem)
{
    public static AuthHandler Instance { get; } = new(AuthSystem.Instance);

    [PacketHandler]
    internal void Connect(GameSession session, ConnectPacket packet)
    {
        authSystem.Connect(session, packet);
    }

    [PacketHandler]
    internal void Register(GameSession session, RegisterPacket packet)
    {
        authSystem.Register(session, packet);
    }
}
