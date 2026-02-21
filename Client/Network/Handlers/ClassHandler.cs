using CryBits.Entities;
using CryBits.Enums;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class ClassHandler
{
    [PacketHandler(ServerPacket.Classes)]
    internal static void Classes(ClassesPacket packet)
    {
        // Read classes dictionary
        Class.List = packet.List;
    }
}
