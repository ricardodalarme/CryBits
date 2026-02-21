using CryBits.Entities;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class ClassHandler
{
    [PacketHandler]
    internal static void Classes(ClassesPacket packet)
    {
        // Read classes dictionary
        Class.List = packet.List;
    }
}
