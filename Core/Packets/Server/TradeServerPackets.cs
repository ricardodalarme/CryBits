using System;

namespace CryBits.Packets.Server;

[Serializable]
public struct TradePacket : IServerPacket
{
    public bool State;
}

[Serializable]
public struct TradeInvitationPacket : IServerPacket
{
    public string PlayerInvitation;
}

[Serializable]
public struct TradeStatePacket : IServerPacket
{
    public byte State;
}

[Serializable]
public struct TradeOfferPacket : IServerPacket
{
    public bool Own;
    public PacketsTradeOfferItem[] Items;
}

[Serializable]
public struct PacketsTradeOfferItem
{
    public Guid ItemId;
    public short Amount;
}
