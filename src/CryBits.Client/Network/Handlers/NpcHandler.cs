using CryBits.Client.Components;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using static CryBits.Definitions.Globals;
using Direction = CryBits.Definitions.Common.Direction;
using MovementState = CryBits.Definitions.Common.Movement;

namespace CryBits.Client.Network.Handlers;

internal class NpcHandler(GameContext context, DefinitionCatalog catalog)
{
    [PacketHandler]
    internal void MapNpcs(MapNpcsPacket packet)
    {
        // Destroy any existing NPC entities from the previous map.
        foreach (var state in context.World.All)
        {
            if (state.Has<NpcTag>())
            {
                var nid = state.Get<NetworkId>();
                if (nid is not null)
                    context.UnregisterNetworkEntity(nid.Value);
            }
        }
        context.World.DestroyWhere(s => s.Has<NpcTag>());

        // Spawn new NPC entities for the current map.
        for (byte i = 0; i < packet.Npcs.Length; i++)
        {
            var npc = packet.Npcs[i];
            var data = catalog.Npcs.Get(npc.NpcId);
            var direction = (Direction)npc.Direction;

            if (data is null) continue;
            var vitals = new Vitals(
                Hp: npc.Vital[(byte)Vital.Hp],
                Mp: npc.Vital[(byte)Vital.Mp],
                MaxHp: data.Vital[(byte)Vital.Hp],
                MaxMp: data.Vital[(byte)Vital.Mp]
            );
            var entity = NpcSpawner.Spawn(context.World, npc.InstanceId, data, npc.X, npc.Y, direction, vitals);
            context.RegisterNetworkEntity(npc.InstanceId, entity);
        }
    }

    [PacketHandler]
    internal void MapNpc(MapNpcPacket packet)
    {
        var old = context.GetNetworkEntity(packet.InstanceId);
        if (old is not null)
        {
            context.UnregisterNetworkEntity(packet.InstanceId);
            context.World.Destroy(old.Value);
        }

        var data = catalog.Npcs.Get(packet.NpcId);
        var direction = (Direction)packet.Direction;

        if (data is null) return;
        var vitals = new Vitals(
            Hp: packet.Vital[(byte)Vital.Hp],
            Mp: packet.Vital[(byte)Vital.Mp],
            MaxHp: data.Vital[(byte)Vital.Hp],
            MaxMp: data.Vital[(byte)Vital.Mp]
        );
        var entity = NpcSpawner.Spawn(context.World, packet.InstanceId, data, packet.X, packet.Y, direction, vitals);
        context.RegisterNetworkEntity(packet.InstanceId, entity);
    }

    [PacketHandler]
    internal void MapNpcMovement(MapNpcMovementPacket packet)
    {
        var npc = context.GetNetworkEntity(packet.InstanceId);
        if (npc is null) return;

        var movement = context.World.Get<MovementComponent>(npc.Value);
        if (movement is null) return;

        var dir = (Direction)packet.Direction;
        var offsetX = 0f;
        var offsetY = 0f;

        if (movement.TileX != packet.X || movement.TileY != packet.Y)
            switch (dir)
            {
                case Direction.Up: offsetY = Grid; break;
                case Direction.Down: offsetY = -Grid; break;
                case Direction.Right: offsetX = -Grid; break;
                case Direction.Left: offsetX = Grid; break;
            }

        context.World.Set(npc.Value, new MovementComponent(
            packet.X, packet.Y, offsetX, offsetY, packet.Speed, (MovementState)packet.Movement, dir
        ));
    }

    [PacketHandler]
    internal void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        var npc = context.GetNetworkEntity(packet.InstanceId);

        if (npc is null) return;

        var movement = context.World.Get<MovementComponent>(npc.Value);
        if (movement is null) return;
        context.World.Set(npc.Value, movement with { Direction = (Direction)packet.Direction, OffsetX = 0f, OffsetY = 0f });
    }

    [PacketHandler]
    internal void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var npc = context.GetNetworkEntity(packet.InstanceId);
        if (npc is null) return;

        var vitals = context.World.Get<Vitals>(npc.Value);
        if (vitals is null) return;
        context.World.Set(npc.Value, new Vitals(
            Hp: packet.Vital[(byte)Vital.Hp],
            Mp: packet.Vital[(byte)Vital.Mp],
            MaxHp: vitals.MaxHp,
            MaxMp: vitals.MaxMp
        ));
    }

    [PacketHandler]
    internal void MapNpcDied(MapNpcDiedPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.InstanceId);
        if (entity is not null)
        {
            context.UnregisterNetworkEntity(packet.InstanceId);
            context.World.Destroy(entity.Value);
        }
    }
}
