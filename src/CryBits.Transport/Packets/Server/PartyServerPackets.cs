using MemoryPack;

namespace CryBits.Transport.Packets.Server;

[MemoryPackable]
public partial class PartyPacket : IServerPacket
{
    public long[] MemberIds;
}

[MemoryPackable]
public partial class PartyInvitationPacket : IServerPacket
{
    public string PlayerInvitation;
}
