using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.State;
using System;
using System.Drawing;

namespace CryBits.Simulation.Systems.Npc;

public static class NpcTargeting
{
    public static void UpdateTarget(World world, EntityId npcId, Tick tick)
    {
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);

        if (npcData.Behaviour == Behaviour.AttackOnSight && !npcState.TargetId.HasValue)
            ScanForTarget(world, npcId, tick);

        if (npcState.TargetId.HasValue)
        {
            var targetE = world.Entities.Get(npcState.TargetId.Value);
            if (targetE != null)
            {
                if (targetE.Has<PlayerTag>())
                {
                    var targetPos = targetE.Get<Position>()!;
                    if (targetPos.MapId != pos.MapId)
                        npcState.TargetId = null;
                }
                else if (targetE.Has<NpcTag>())
                {
                    var targetNpcState = targetE.Get<NpcState>();
                    var targetPos = targetE.Get<Position>()!;
                    if (targetNpcState == null || targetPos.MapId != pos.MapId)
                        npcState.TargetId = null;
                }
            }
            else
                npcState.TargetId = null;

            world.Dirty.Mark<NpcState>(npcId);
        }

        if (npcState.TargetId.HasValue)
        {
            var targetE = world.Entities.Get(npcState.TargetId.Value)!;
            var targetPos = targetE.Get<Position>()!;
            var distance = Math.Sqrt(Math.Pow(pos.X - targetPos.X, 2) + Math.Pow(pos.Y - targetPos.Y, 2));
            if (npcData.Sight < distance)
            {
                npcState.TargetId = null;
                world.Dirty.Mark<NpcState>(npcId);
            }
        }
    }

    private static void ScanForTarget(World world, EntityId npcId, Tick tick)
    {
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);
        var map = world.Maps.Get(pos.MapId)!;

        short distance;

        foreach (var state in world.Entities.All)
        {
            if (!state.Has<PlayerTag>()) continue;
            var targetPos = state.Get<Position>();
            if (targetPos == null || targetPos.MapId != pos.MapId) continue;

            distance = (short)Math.Sqrt(Math.Pow(pos.X - targetPos.X, 2) +
                                        Math.Pow(pos.Y - targetPos.Y, 2));
            if (distance <= npcData.Sight)
            {
                npcState.TargetId = state.Id;
                world.Dirty.Mark<NpcState>(npcId);
                if (!string.IsNullOrEmpty(npcData.SayMsg))
                    tick.Events.Emit(new ChatMessageEvent
                    {
                        RecipientId = state.Id.Value,
                        Text = npcData.Name + ": " + npcData.SayMsg,
                        ColorArgb = Color.White.ToArgb()
                    });
                return;
            }
        }

        if (!npcData.AttackNpc) return;

        foreach (var otherNpcId in map.NpcIds)
        {
            if (otherNpcId == npcId) continue;
            var otherE = world.Entities.Get(otherNpcId);
            if (otherE == null) continue;
            var otherNpcState = otherE.Get<NpcState>();
            if (otherNpcState == null) continue;
            var otherPos = otherE.Get<Position>();
            if (otherPos == null) continue;
            var otherData = DefinitionCatalog.Instance.Npcs.Get(otherNpcState.NpcDefId);
            if (npcData.IsAllied(otherData.Id)) continue;

            distance = (short)Math.Sqrt(Math.Pow(pos.X - otherPos.X, 2) +
                                        Math.Pow(pos.Y - otherPos.Y, 2));
            if (distance <= npcData.Sight)
            {
                npcState.TargetId = otherNpcId;
                world.Dirty.Mark<NpcState>(npcId);
                return;
            }
        }
    }
}
