using System;

namespace CryBits.Packets.Server;

[Serializable]
public struct MapsPacket : IServerPacket
{
    public System.Collections.Generic.Dictionary<Guid, Entities.Map.Map> List;
}

[Serializable]
public struct MapPacket : IServerPacket
{
    public Entities.Map.Map Map;
}

[Serializable] public struct JoinMapPacket : IServerPacket;

[Serializable]
public struct MapRevisionPacket : IServerPacket
{
    public Guid MapId;
    public short Revision;
}

[Serializable]
public struct MapNpcsPacket : IServerPacket
{
    public PacketsMapNpc[] Npcs;
}

[Serializable]
public struct MapNpcPacket : IServerPacket
{
    public byte Index;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital;
}

[Serializable]
public struct MapNpcMovementPacket : IServerPacket
{
    public byte Index;
    public byte X, Y;
    public byte Direction;
    public byte Movement;
}

[Serializable]
public struct MapNpcDirectionPacket : IServerPacket
{
    public byte Index;
    public byte Direction;
}

[Serializable]
public struct MapNpcVitalsPacket : IServerPacket
{
    public byte Index;
    public short[] Vital;
}

[Serializable]
public struct MapNpcAttackPacket : IServerPacket
{
    public byte Index;
    public string Victim;
    public byte VictimType;
}

[Serializable]
public struct MapNpcDiedPacket : IServerPacket
{
    public byte Index;
}

[Serializable]
public struct MapItemsPacket : IServerPacket
{
    public PacketsMapItem[] Items;
}

[Serializable]
public struct PacketsMapItem
{
    public Guid ItemId;
    public byte X, Y;
}

[Serializable]
public struct PacketsMapNpc
{
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital;
}
