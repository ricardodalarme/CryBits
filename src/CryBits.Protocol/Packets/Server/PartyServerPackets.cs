using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class PartyPacket : IServerPacket
{
    public long[] MemberIds = [];
}

[MemoryPackable]
public partial class PartyInvitationPacket : IServerPacket
{
    public string PlayerInvitation = string.Empty;
}
