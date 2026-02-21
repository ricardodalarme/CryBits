using System;

namespace CryBits.Packets.Server;

[Serializable]
public struct PartyPacket : IServerPacket
{
    public string[] Members;
}

[Serializable]
public struct PartyInvitationPacket : IServerPacket
{
    public string PlayerInvitation;
}
