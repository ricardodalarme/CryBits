using CryBits.Client.UI.Menu.Views;
using CryBits.Entities;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ClassHandler
{
    [PacketHandler]
    internal void Classes(ClassesPacket packet)
    {
        // Read classes dictionary
        Class.List = packet.List;
        CreateCharacterView.UpdateClassLabels();
    }
}
