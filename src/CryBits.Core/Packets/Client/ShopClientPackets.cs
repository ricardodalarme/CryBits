using System;

namespace CryBits.Packets.Client;

[Serializable] public struct ShopBuyPacket : IClientPacket { public short Slot; }
[Serializable] public struct ShopSellPacket : IClientPacket { public short Slot; public short Amount; }
[Serializable] public struct ShopClosePacket : IClientPacket;
