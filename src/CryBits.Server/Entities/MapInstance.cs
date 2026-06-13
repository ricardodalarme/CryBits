using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Server.Systems.Npc;
using CryBits.Server.World;
using CryBits.Simulation.Entities;
using System;
using System.Collections.Generic;
using CryBits.Simulation.State;

namespace CryBits.Server.Entities;

internal class MapInstance(Guid id, Map map) : Entity(id)
{
    public readonly Map Data = map;
    public List<EntityId> NpcIds = [];
    public List<GroundItem> Item = [];

    public EntityId? HasNpc(byte x, byte y, EntityRegistry entities)
    {
        foreach (var npcId in NpcIds)
        {
            var entityState = entities.Get(npcId);
            if (entityState == null) continue;
            var npcState = entityState.Get<NpcState>();
            if (npcState == null || !npcState.Alive) continue;
            var pos = entityState.Get<Position>();
            if (pos == null) continue;
            if (pos.X == x && pos.Y == y)
                return npcId;
        }
        return null;
    }

    public EntityId? HasPlayer(byte x, byte y, EntityRegistry entities)
    {
        foreach (var session in GameWorld.Current.Sessions)
        {
            if (!session.IsPlaying) continue;
            if (session.Character is not { } characterId) continue;
            var entityState = entities.Get(characterId);
            if (entityState == null) continue;
            var pos = entityState.Get<Position>();
            if (pos == null) continue;
            if (pos.X == x && pos.Y == y && pos.MapId == Id)
                return characterId;
        }
        return null;
    }

    public bool HasPlayers(EntityRegistry entities)
    {
        foreach (var session in GameWorld.Current.Sessions)
        {
            if (!session.IsPlaying) continue;
            if (session.Character is not { } characterId) continue;
            var entityState = entities.Get(characterId);
            if (entityState == null) continue;
            var pos = entityState.Get<Position>();
            if (pos == null) continue;
            if (pos.MapId == Id)
                return true;
        }
        return false;
    }

    public GroundItem HasItem(byte x, byte y)
    {
        for (var i = Item.Count - 1; i >= 0; i--)
            if (Item[i].X == x && Item[i].Y == y)
                return Item[i];
        return null;
    }

    public void SpawnItems()
    {
        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                if (Data.Attribute[x, y].Type == (byte)TileAttribute.Item)
                    Item.Add(new GroundItem(new Guid(Data.Attribute[x, y].Data1),
                        Data.Attribute[x, y].Data2, x, y));
    }

    public bool TileBlocked(byte x, byte y, Direction direction, EntityRegistry entities, bool countEntities = true)
    {
        byte nextX = x, nextY = y;
        direction.NextTile(ref nextX, ref nextY);

        if (Data.TileBlocked(nextX, nextY)) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)direction.Reverse()]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (countEntities && (HasPlayer(nextX, nextY, entities) != null || HasNpc(nextX, nextY, entities) != null)) return true;
        return false;
    }

    public static void Create(Map map, bool isOriginal, EntityRegistry entities)
    {
        var id = isOriginal ? map.Id : Guid.NewGuid();
        var tempMap = new MapInstance(id, map);
        GameWorld.Current.Maps.Add(id, tempMap);

        for (byte i = 0; i < map.Npc.Count; i++)
        {
            var npcData = DefinitionCatalog.Instance.Npcs.Get(map.Npc[i].NpcId);
            if (npcData == null) continue;

            var entityId = entities.Create();
            var entityState = entities.Get(entityId)!;

            entityState.Set(new NpcState
            {
                Index = i,
                NpcDefId = map.Npc[i].NpcId,
                Alive = false,
                TargetId = null,
                SpawnTimer = 0,
                AttackTimer = 0
            });

            entityState.Set(new Position
            {
                X = map.Npc[i].X,
                Y = map.Npc[i].Y,
                Direction = Direction.Down,
                MapId = tempMap.Id
            });

            entityState.Set(new Vitals
            {
                Hp = npcData.Vital[(byte)Vital.Hp],
                Mp = npcData.Vital[(byte)Vital.Mp],
                MaxHp = npcData.Vital[(byte)Vital.Hp],
                MaxMp = npcData.Vital[(byte)Vital.Mp]
            });

            entityState.Set(new CombatState());
            entityState.Set(new NpcTag());

            tempMap.NpcIds.Add(entityId);
            NpcBrainSystem.Instance.Spawn(entityId);
        }

        tempMap.SpawnItems();
    }
}
