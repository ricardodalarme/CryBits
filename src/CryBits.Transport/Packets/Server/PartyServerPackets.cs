using System;

namespace CryBits.Transport.Packets.Server;

[Serializable]
public struct PartyPacket : IServerPacket
{
    public long[] MemberIds;
}

[Serializable]
public struct PartyInvitationPacket : IServerPacket
{
    public string PlayerInvitation;
}
