using System;

namespace CryBits.Network.Packets.Server;

[Serializable]
public struct PartyPacket : IServerPacket
{
    public Guid[] MemberIds;
}

[Serializable]
public struct PartyInvitationPacket : IServerPacket
{
    public string PlayerInvitation;
}
