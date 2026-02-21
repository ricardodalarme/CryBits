using System;

namespace CryBits.Packets.Server;

[Serializable] public struct JoinGamePacket : IServerPacket;
[Serializable]
public struct PlayerDataPacket : IServerPacket
{
    public string Name; public short TextureNum, Level; public Guid MapId; public byte X, Y, Direction;
    public short[] Vital, MaxVital, Attribute; public Guid[] Equipment;
}
[Serializable] public struct PlayerPositionPacket : IServerPacket { public string Name; public byte X, Y, Direction; }
[Serializable] public struct PlayerVitalsPacket : IServerPacket { public string Name; public short[] Vital, MaxVital; }
[Serializable] public struct PlayerLeavePacket : IServerPacket { public string Name; }
[Serializable] public struct PlayerAttackPacket : IServerPacket { public string Name; public string Victim; public byte VictimType; }
[Serializable] public struct PlayerMovePacket : IServerPacket { public string Name; public byte X, Y, Direction, Movement; }
[Serializable] public struct PlayerDirectionPacket : IServerPacket { public string Name; public byte Direction; }
[Serializable] public struct PlayerExperiencePacket : IServerPacket { public int Experience, ExpNeeded; public byte Points; }
[Serializable] public struct PlayerInventoryPacket : IServerPacket { public Guid[] ItemIds; public short[] Amounts; }
[Serializable] public struct PlayerEquipmentsPacket : IServerPacket { public string Name; public Guid[] Equipments; }
[Serializable] public struct PlayerHotbarPacket : IServerPacket { public byte[] Types; public byte[] Slots; }
