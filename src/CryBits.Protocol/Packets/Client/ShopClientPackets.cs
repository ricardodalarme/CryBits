using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable] public partial class ShopBuyPacket : IClientPacket { public short Slot; }
[MemoryPackable] public partial class ShopSellPacket : IClientPacket { public short Slot; public short Amount; }
[MemoryPackable] public partial class ShopClosePacket : IClientPacket;
