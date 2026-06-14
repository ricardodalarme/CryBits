using System;

namespace CryBits.Transport.Packets.Client;

[Serializable]
public struct MessagePacket : IClientPacket
{
    public string Text;
    public byte Type;
    public string Addressee;
}
