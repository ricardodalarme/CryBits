using MemoryPack;

namespace CryBits.Transport.Packets.Server;

[MemoryPackable]
public partial class TradePacket : IServerPacket
{
    public bool State;
}

[MemoryPackable]
public partial class TradeInvitationPacket : IServerPacket
{
    public string PlayerInvitation;
}

[MemoryPackable]
public partial class TradeStatePacket : IServerPacket
{
    public byte State;
}

[MemoryPackable]
public partial class TradeOfferPacket : IServerPacket
{
    public bool Own;
    public PacketsTradeOfferItem[] Items;
}

[MemoryPackable]
public partial struct PacketsTradeOfferItem
{
    public Guid ItemId;
    public short Amount;
}
