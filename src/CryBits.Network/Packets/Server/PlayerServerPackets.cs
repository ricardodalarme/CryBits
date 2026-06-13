using System;

namespace CryBits.Network.Packets.Server;

[Serializable] public struct JoinGamePacket : IServerPacket;
[Serializable]
public struct PlayerDataPacket : IServerPacket
{
    public long NetworkId; public string Name; public short TextureNum, Level; public Guid MapId; public byte X, Y, Direction;
    public short[] Vital, MaxVital, Attribute; public Guid[] Equipment;
}
[Serializable] public struct PlayerPositionPacket : IServerPacket { public long NetworkId; public byte X, Y, Direction; }
[Serializable] public struct PlayerVitalsPacket : IServerPacket { public long NetworkId; public short[] Vital, MaxVital; }
[Serializable] public struct PlayerLeavePacket : IServerPacket { public long NetworkId; }
[Serializable] public struct PlayerMovePacket : IServerPacket { public long NetworkId; public byte X, Y, Direction, Movement; public float Speed; }
[Serializable] public struct PlayerDirectionPacket : IServerPacket { public long NetworkId; public byte Direction; }
[Serializable] public struct PlayerExperiencePacket : IServerPacket { public int Experience, ExpNeeded; public byte Points; }
[Serializable] public struct PlayerInventoryPacket : IServerPacket { public Guid[] ItemIds; public short[] Amounts; }
[Serializable] public struct PlayerEquipmentsPacket : IServerPacket { public long NetworkId; public Guid[] Equipments; }
[Serializable] public struct PlayerHotbarPacket : IServerPacket { public byte[] Types; public byte[] Slots; }
