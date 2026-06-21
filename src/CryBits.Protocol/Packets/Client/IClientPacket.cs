using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable]
[MemoryPackUnion(0, typeof(IntentPacket))]
[MemoryPackUnion(1, typeof(ConnectPacket))]
[MemoryPackUnion(2, typeof(RegisterPacket))]
[MemoryPackUnion(3, typeof(CreateCharacterPacket))]
[MemoryPackUnion(4, typeof(CharacterUsePacket))]
[MemoryPackUnion(5, typeof(CharacterCreatePacket))]
[MemoryPackUnion(6, typeof(CharacterDeletePacket))]
[MemoryPackUnion(7, typeof(WriteClassesPacket))]
[MemoryPackUnion(8, typeof(WriteMapsPacket))]
[MemoryPackUnion(9, typeof(WriteNpcsPacket))]
[MemoryPackUnion(10, typeof(WriteItemsPacket))]
[MemoryPackUnion(11, typeof(WriteShopsPacket))]
[MemoryPackUnion(12, typeof(RequestClassesPacket))]
[MemoryPackUnion(13, typeof(RequestMapPacket))]
[MemoryPackUnion(14, typeof(RequestMapsPacket))]
[MemoryPackUnion(15, typeof(RequestNpcsPacket))]
[MemoryPackUnion(16, typeof(RequestItemsPacket))]
[MemoryPackUnion(17, typeof(RequestShopsPacket))]
public partial interface IClientPacket : IPacket;
