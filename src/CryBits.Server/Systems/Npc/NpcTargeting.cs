using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Npcs;
using CryBits.Server.Simulation.State;
using CryBits.Server.Simulation.State.Components;
using CryBits.Server.World;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using System;
using System.Drawing;

namespace CryBits.Server.Systems.Npc;

internal static class NpcTargeting
{
    internal static void UpdateTarget(EntityId npcId, Tick tick)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);

        if (npcData.Behaviour == Behaviour.AttackOnSight && !npcState.TargetId.HasValue)
            ScanForTarget(npcId, tick);

        if (npcState.TargetId.HasValue)
        {
            var targetE = world.Entities.Get(npcState.TargetId.Value);
            if (targetE != null)
            {
                if (targetE.Has<PlayerTag>())
                {
                    var targetPos = targetE.Get<Position>()!;
                    var session = world.SessionMap.Get(npcState.TargetId.Value);
                    if (session == null || !session.IsPlaying || targetPos.MapId != pos.MapId)
                        npcState.TargetId = null;
                }
                else if (targetE.Has<NpcTag>())
                {
                    var targetNpcState = targetE.Get<NpcState>();
                    var targetPos = targetE.Get<Position>()!;
                    if (targetNpcState == null || !targetNpcState.Alive || targetPos.MapId != pos.MapId)
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

    private static void ScanForTarget(EntityId npcId, Tick tick)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);
        var map = world.Maps.Get(pos.MapId)!;

        short distance;

        foreach (var session in world.Sessions)
        {
            if (!session.IsPlaying) continue;
            if (session.Character is not { } targetPlayerId) continue;
            var targetE = world.Entities.Get(targetPlayerId);
            if (targetE == null) continue;
            var targetPos = targetE.Get<Position>();
            if (targetPos == null || targetPos.MapId != pos.MapId) continue;

            distance = (short)Math.Sqrt(Math.Pow(pos.X - targetPos.X, 2) +
                                        Math.Pow(pos.Y - targetPos.Y, 2));
            if (distance <= npcData.Sight)
            {
                npcState.TargetId = targetPlayerId;
                world.Dirty.Mark<NpcState>(npcId);
                if (!string.IsNullOrEmpty(npcData.SayMsg))
                    tick.Events.Emit(new ChatMessageEvent
                    {
                        RecipientId = targetPlayerId.Value,
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
            if (otherNpcState == null || !otherNpcState.Alive) continue;
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
