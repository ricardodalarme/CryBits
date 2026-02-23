using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class NpcSender
{
    private static CryBits.Server.ECS.World World => ServerContext.Instance.World;

    public static void Npcs(GameSession session)
    {
        Send.ToPlayer(session, new NpcsPacket { List = Npc.List });
    }

    public static void MapNpcs(Player player, MapInstance mapInstance)
    {
        // Gather all NPC entities on this map
        var npcsOnMap = new System.Collections.Generic.List<PacketsMapNpc>();
        foreach (var (entityId, npcData) in World.Query<NpcDataComponent>())
        {
            if (npcData.MapId != mapInstance.Data.Id) continue;
            var pos   = World.Get<PositionComponent>(entityId);
            var dir   = World.Get<DirectionComponent>(entityId);
            var vital = World.Get<VitalsComponent>(entityId);
            var entry = new PacketsMapNpc
            {
                NpcId     = npcData.Data.GetId(),
                X         = pos.X,
                Y         = pos.Y,
                Direction = (byte)dir.Value,
                Vital     = new short[(byte)Vital.Count]
            };
            for (byte n = 0; n < (byte)Vital.Count; n++) entry.Vital[n] = vital.Values[n];
            npcsOnMap.Add(entry);
        }

        Send.ToPlayer(player, new MapNpcsPacket { Npcs = npcsOnMap.ToArray() });
    }

    public static void MapNpc(int npcEntityId)
    {
        var npcData = World.Get<NpcDataComponent>(npcEntityId);
        var pos     = World.Get<PositionComponent>(npcEntityId);
        var dir     = World.Get<DirectionComponent>(npcEntityId);
        var vital   = World.Get<VitalsComponent>(npcEntityId);
        var mapInst = GameWorld.Current.Maps.Get(npcData.MapId);
        if (mapInst == null) return;

        var packet = new MapNpcPacket
        {
            Index     = npcData.Index,
            NpcId     = npcData.Data.GetId(),
            X         = pos.X,
            Y         = pos.Y,
            Direction = (byte)dir.Value,
            Vital     = new short[(byte)Vital.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = vital.Values[n];
        Send.ToMap(mapInst, packet);
    }

    public static void MapNpcMovement(int npcEntityId, byte movement)
    {
        var npcData = World.Get<NpcDataComponent>(npcEntityId);
        var pos     = World.Get<PositionComponent>(npcEntityId);
        var dir     = World.Get<DirectionComponent>(npcEntityId);
        var mapInst = GameWorld.Current.Maps.Get(npcData.MapId);
        if (mapInst == null) return;

        Send.ToMap(mapInst,
            new MapNpcMovementPacket
            { Index = npcData.Index, X = pos.X, Y = pos.Y, Direction = (byte)dir.Value, Movement = movement });
    }

    public static void MapNpcDirection(int npcEntityId)
    {
        var npcData = World.Get<NpcDataComponent>(npcEntityId);
        var dir     = World.Get<DirectionComponent>(npcEntityId);
        var mapInst = GameWorld.Current.Maps.Get(npcData.MapId);
        if (mapInst == null) return;

        Send.ToMap(mapInst,
            new MapNpcDirectionPacket { Index = npcData.Index, Direction = (byte)dir.Value });
    }

    public static void MapNpcVitals(int npcEntityId)
    {
        var npcData = World.Get<NpcDataComponent>(npcEntityId);
        var vital   = World.Get<VitalsComponent>(npcEntityId);
        var mapInst = GameWorld.Current.Maps.Get(npcData.MapId);
        if (mapInst == null) return;

        var packet = new MapNpcVitalsPacket { Index = npcData.Index, Vital = new short[(byte)Vital.Count] };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = vital.Values[n];
        Send.ToMap(mapInst, packet);
    }

    public static void MapNpcAttack(int npcEntityId, string victim = "", Target victimType = 0)
    {
        var npcData = World.Get<NpcDataComponent>(npcEntityId);
        var mapInst = GameWorld.Current.Maps.Get(npcData.MapId);
        if (mapInst == null) return;

        Send.ToMap(mapInst,
            new MapNpcAttackPacket { Index = npcData.Index, Victim = victim, VictimType = (byte)victimType });
    }

    public static void MapNpcDied(int npcEntityId)
    {
        var npcData = World.Get<NpcDataComponent>(npcEntityId);
        var mapInst = GameWorld.Current.Maps.Get(npcData.MapId);
        if (mapInst == null) return;

        Send.ToMap(mapInst, new MapNpcDiedPacket { Index = npcData.Index });
    }
}
