using System;

namespace CryBits.Packets.Client;

[Serializable]
public struct TradeInvitePacket : IClientPacket
{
    public string PlayerName;
}

[Serializable] public struct TradeAcceptPacket : IClientPacket;
[Serializable] public struct TradeDeclinePacket : IClientPacket;
[Serializable] public struct TradeLeavePacket : IClientPacket;

[Serializable]
public struct TradeOfferPacket : IClientPacket
{
    public short Slot;
    public short InventorySlot;
    public short Amount;
}

[Serializable]
public struct TradeOfferStatePacket : IClientPacket
{
    public byte State;
}
