using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable]
public partial class PartyInvitePacket : IClientPacket
{
    public string PlayerName;
}

[MemoryPackable] public partial class PartyAcceptPacket : IClientPacket;
[MemoryPackable] public partial class PartyDeclinePacket : IClientPacket;
[MemoryPackable] public partial class PartyLeavePacket : IClientPacket;
