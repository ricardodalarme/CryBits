using System;

namespace CryBits.Transport.Packets.Server;

[Serializable]
public struct AlertPacket : IServerPacket
{
    public string Message;
}

[Serializable]
public struct MessagePacket : IServerPacket
{
    public string Text;
    public int ColorArgb;
}
