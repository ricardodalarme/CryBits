using MemoryPack;

namespace CryBits.Transport.Packets.Client;

[MemoryPackable]
public partial class TradeInvitePacket : IClientPacket
{
    public string PlayerName;
}

[MemoryPackable] public partial class TradeAcceptPacket : IClientPacket;
[MemoryPackable] public partial class TradeDeclinePacket : IClientPacket;
[MemoryPackable] public partial class TradeLeavePacket : IClientPacket;

[MemoryPackable]
public partial class TradeOfferPacket : IClientPacket
{
    public short Slot;
    public short InventorySlot;
    public short Amount;
}

[MemoryPackable]
public partial class TradeOfferStatePacket : IClientPacket
{
    public byte State;
}
