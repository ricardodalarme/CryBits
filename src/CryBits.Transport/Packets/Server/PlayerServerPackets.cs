using MemoryPack;

namespace CryBits.Transport.Packets.Server;

[MemoryPackable] public partial class JoinGamePacket : IServerPacket;
[MemoryPackable]
public partial class PlayerDataPacket : IServerPacket
{
    public long NetworkId; public string Name; public short TextureNum, Level; public Guid MapId; public byte X, Y, Direction;
    public short[] Vital, MaxVital, Attribute; public Guid[] Equipment;
}
[MemoryPackable] public partial class PlayerPositionPacket : IServerPacket { public long NetworkId; public byte X, Y, Direction; }
[MemoryPackable] public partial class PlayerVitalsPacket : IServerPacket { public long NetworkId; public short[] Vital, MaxVital; }
[MemoryPackable] public partial class PlayerLeavePacket : IServerPacket { public long NetworkId; }
[MemoryPackable] public partial class PlayerMovePacket : IServerPacket { public long NetworkId; public byte X, Y, Direction, Movement; public float Speed; }
[MemoryPackable] public partial class PlayerDirectionPacket : IServerPacket { public long NetworkId; public byte Direction; }
[MemoryPackable] public partial class PlayerExperiencePacket : IServerPacket { public int Experience, ExpNeeded; public byte Points; }
[MemoryPackable] public partial class PlayerInventoryPacket : IServerPacket { public Guid[] ItemIds; public short[] Amounts; }
[MemoryPackable] public partial class PlayerEquipmentsPacket : IServerPacket { public long NetworkId; public Guid[] Equipments; }
[MemoryPackable] public partial class PlayerHotbarPacket : IServerPacket { public byte[] Types; public byte[] Slots; }
