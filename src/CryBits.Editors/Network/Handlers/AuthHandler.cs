using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Forms.Login;
using CryBits.Editors.Maps;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Editors.Network.Handlers;

internal class AuthHandler
{
    [PacketHandler]
    internal void Alert(AlertPacket packet)
    {
        MessageBox.Show(packet.Message);
    }

    [PacketHandler]
    internal void Connect(ConnectPacket _)
    {
        LoginWindow.HideWindow();
        EditorMapsWindow.Open();
    }
}
