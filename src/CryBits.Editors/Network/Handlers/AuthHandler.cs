using CryBits.Editors.Core;
using CryBits.Editors.Forms.Login;
using CryBits.Editors.Forms.Maps;
using CryBits.Editors.Utils;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Editors.Network.Handlers;

internal class AuthHandler(EditorShell shell)
{
    [PacketHandler]
    internal void Alert(AlertPacket packet)
    {
        MessageBox.Show(packet.Message);
    }

    [PacketHandler]
    internal void Connect(ConnectPacket _)
    {
        LoginWindow.HideWindow(shell);
        EditorMapsWindow.Open(shell);
    }
}
