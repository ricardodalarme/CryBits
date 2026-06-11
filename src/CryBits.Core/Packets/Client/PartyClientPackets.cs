using System;

namespace CryBits.Packets.Client;

[Serializable]
public struct PartyInvitePacket : IClientPacket
{
    public string PlayerName;
}

[Serializable] public struct PartyAcceptPacket : IClientPacket;
[Serializable] public struct PartyDeclinePacket : IClientPacket;
[Serializable] public struct PartyLeavePacket : IClientPacket;
