using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
[MemoryPackUnion(0, typeof(CreateCharacterPacket))]
[MemoryPackUnion(1, typeof(CharactersPacket))]
[MemoryPackUnion(2, typeof(ConnectPacket))]
[MemoryPackUnion(3, typeof(AlertPacket))]
[MemoryPackUnion(4, typeof(MessagePacket))]
[MemoryPackUnion(5, typeof(MapsPacket))]
[MemoryPackUnion(6, typeof(MapPacket))]
[MemoryPackUnion(7, typeof(MapRevisionPacket))]
[MemoryPackUnion(8, typeof(PartyPacket))]
[MemoryPackUnion(9, typeof(PartyInvitationPacket))]
[MemoryPackUnion(10, typeof(JoinGamePacket))]
[MemoryPackUnion(11, typeof(ClassesPacket))]
[MemoryPackUnion(12, typeof(NpcsPacket))]
[MemoryPackUnion(13, typeof(ItemsPacket))]
[MemoryPackUnion(14, typeof(JoinPacket))]
[MemoryPackUnion(15, typeof(ShopsPacket))]
[MemoryPackUnion(16, typeof(ShopOpenPacket))]
[MemoryPackUnion(17, typeof(TradePacket))]
[MemoryPackUnion(18, typeof(TradeInvitationPacket))]
[MemoryPackUnion(19, typeof(TradeStatePacket))]
[MemoryPackUnion(20, typeof(TradeOfferPacket))]
[MemoryPackUnion(21, typeof(KeyframePacket))]
public partial interface IServerPacket : IPacket;
