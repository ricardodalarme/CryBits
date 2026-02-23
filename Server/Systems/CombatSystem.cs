using System;
using System.Drawing;
using CryBits.Enums;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Formulas;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using static CryBits.Globals;
using static CryBits.Utils;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Systems;

/// <summary>Owns all combat resolution logic for players and NPCs.</summary>
internal static class CombatSystem
{
    /// <summary>Initiates an attack for <paramref name="player"/> against whatever is in front of them.</summary>
    internal static void Attack(Player player)
    {
        var world = ServerContext.Instance.World;
        var pos   = world.Get<PositionComponent>(player.EntityId);
        var dir   = world.Get<DirectionComponent>(player.EntityId);
        var timer = world.Get<TimerComponent>(player.EntityId);
        var trade = world.Get<TradeComponent>(player.EntityId);
        var map   = player.MapInstance;

        byte nextX = pos.X, nextY = pos.Y;
        NextTile(dir.Value, ref nextX, ref nextY);

        if (trade.PartnerId != null) return;
        if (world.Has<ShopComponent>(player.EntityId)) return;
        if (Environment.TickCount64 < timer.AttackTimer + AttackSpeed) return;
        if (map.TileBlocked(pos.X, pos.Y, dir.Value, false)) goto @continue;

        var victimPlayer = map.HasPlayer(nextX, nextY);
        if (victimPlayer != null)
        {
            PlayerAttackPlayer(player, victimPlayer);
            return;
        }

        var victimNpcId = map.HasNpc(nextX, nextY);
        if (victimNpcId >= 0)
        {
            PlayerAttackNpc(player, victimNpcId);
            return;
        }

    @continue:
        PlayerSender.PlayerAttack(player, null);
        timer.AttackTimer = Environment.TickCount64;
    }

    private static void PlayerAttackPlayer(Player attacker, Player victim)
    {
        var world = ServerContext.Instance.World;
        if (world.Has<LoadingMapTag>(victim.EntityId)) return;
        if (attacker.MapInstance.Data.Moral == (byte)Moral.Pacific)
        {
            ChatSender.Message(attacker, "This is a peaceful area.", Color.White);
            return;
        }

        world.Get<TimerComponent>(attacker.EntityId).AttackTimer = Environment.TickCount64;

        var attackDamage = CombatFormulas.NetDamage(attacker.Damage, victim.PlayerDefense);
        if (attackDamage > 0)
        {
            var victimData = world.Get<PlayerDataComponent>(victim.EntityId);
            PlayerSender.PlayerAttack(attacker, victimData.Name, Target.Player);

            var victimVitals = world.Get<VitalsComponent>(victim.EntityId);
            if (attackDamage < victimVitals.Values[(byte)Vital.Hp])
            {
                victimVitals.Values[(byte)Vital.Hp] -= attackDamage;
                PlayerSender.PlayerVitals(victim);
            }
            else
            {
                var attackerData = world.Get<PlayerDataComponent>(attacker.EntityId);
                LevelingSystem.GiveExperience(attacker, world.Get<PlayerDataComponent>(victim.EntityId).Experience / 10);
                Died(victim);
            }
        }
        else
            PlayerSender.PlayerAttack(attacker);
    }

    private static void PlayerAttackNpc(Player attacker, int npcEntityId)
    {
        var world   = ServerContext.Instance.World;
        var npcData = world.Get<NpcDataComponent>(npcEntityId);
        var npcTgt  = world.Get<NpcTargetComponent>(npcEntityId);

        if (npcTgt.TargetEntityId != attacker.EntityId && !string.IsNullOrEmpty(npcData.Data.SayMsg))
            ChatSender.Message(attacker, npcData.Data.Name + ": " + npcData.Data.SayMsg, Color.White);

        switch (npcData.Data.Behaviour)
        {
            case Behaviour.Friendly: return;
            case Behaviour.ShopKeeper:
                ShopSystem.Open(attacker, npcData.Data.Shop);
                return;
        }

        npcTgt.TargetEntityId = attacker.EntityId;
        world.Get<TimerComponent>(attacker.EntityId).AttackTimer = Environment.TickCount64;

        var attackDamage = CombatFormulas.NetDamage(attacker.Damage, npcData.Data.Attribute[(byte)Attribute.Resistance]);
        if (attackDamage > 0)
        {
            PlayerSender.PlayerAttack(attacker, npcData.Index.ToString(), Target.Npc);

            var npcVitals = world.Get<VitalsComponent>(npcEntityId);
            if (attackDamage < npcVitals.Values[(byte)Vital.Hp])
            {
                npcVitals.Values[(byte)Vital.Hp] -= attackDamage;
                NpcSender.MapNpcVitals(npcEntityId);
            }
            else
            {
                LevelingSystem.GiveExperience(attacker, npcData.Data.Experience);
                Died(npcEntityId);
            }
        }
        else
            PlayerSender.PlayerAttack(attacker);
    }

    /// <summary>Kills <paramref name="player"/>: restores vitals, penalises XP, warps to spawn.</summary>
    internal static void Died(Player player)
    {
        var world  = ServerContext.Instance.World;
        var vitals = world.Get<VitalsComponent>(player.EntityId);
        var pd     = world.Get<PlayerDataComponent>(player.EntityId);
        var dir    = world.Get<DirectionComponent>(player.EntityId);

        for (byte n = 0; n < (byte)Vital.Count; n++)
            vitals.Values[n] = player.MaxVital(n);
        PlayerSender.PlayerVitals(player);

        pd.Experience /= 10;
        PlayerSender.PlayerExperience(player);

        dir.Value = (Direction)pd.Class!.SpawnDirection;
        MovementSystem.Warp(player,
            GameWorld.Current.Maps.Get(pd.Class.SpawnMap.Id)!,
            pd.Class.SpawnX,
            pd.Class.SpawnY);
    }

    /// <summary>Initiates an attack for the NPC entity against its current target.</summary>
    internal static void Attack(int npcEntityId)
    {
        var world    = ServerContext.Instance.World;
        var npcData  = world.Get<NpcDataComponent>(npcEntityId);
        var npcState = world.Get<NpcStateComponent>(npcEntityId);
        var npcTgt   = world.Get<NpcTargetComponent>(npcEntityId);
        var npcTimer = world.Get<NpcTimerComponent>(npcEntityId);
        var pos      = world.Get<PositionComponent>(npcEntityId);
        var dir      = world.Get<DirectionComponent>(npcEntityId);
        var map      = GameWorld.Current.Maps[npcData.MapId];

        if (!npcState.Alive) return;
        if (Environment.TickCount64 < npcTimer.AttackTimer + AttackSpeed) return;
        if (map.TileBlocked(pos.X, pos.Y, dir.Value, false)) return;

        byte nextX = pos.X, nextY = pos.Y;
        NextTile(dir.Value, ref nextX, ref nextY);

        if (npcTgt.TargetEntityId != null)
        {
            var tid = npcTgt.TargetEntityId.Value;
            if (world.Has<PlayerDataComponent>(tid))
                NpcAttackPlayer(npcEntityId, map.HasPlayer(nextX, nextY));
            else if (world.Has<NpcDataComponent>(tid))
            {
                var otherNpcId = map.HasNpc(nextX, nextY);
                if (otherNpcId >= 0) NpcAttackNpc(npcEntityId, otherNpcId);
            }
        }
    }

    private static void NpcAttackPlayer(int npcEntityId, Player? victim)
    {
        if (victim == null) return;

        var world    = ServerContext.Instance.World;
        var npcData  = world.Get<NpcDataComponent>(npcEntityId);
        var npcTimer = world.Get<NpcTimerComponent>(npcEntityId);
        var npcTgt   = world.Get<NpcTargetComponent>(npcEntityId);

        if (world.Has<LoadingMapTag>(victim.EntityId)) return;

        npcTimer.AttackTimer = Environment.TickCount64;

        var attackDamage = CombatFormulas.NetDamage(
            npcData.Data.Attribute[(byte)Attribute.Strength],
            victim.PlayerDefense);

        if (attackDamage > 0)
        {
            var victimData = world.Get<PlayerDataComponent>(victim.EntityId);
            NpcSender.MapNpcAttack(npcEntityId, victimData.Name, Target.Player);

            var victimVitals = world.Get<VitalsComponent>(victim.EntityId);
            if (attackDamage < victimVitals.Values[(byte)Vital.Hp])
            {
                victimVitals.Values[(byte)Vital.Hp] -= attackDamage;
                PlayerSender.PlayerVitals(victim);
            }
            else
            {
                npcTgt.TargetEntityId = null;
                Died(victim);
            }
        }
        else
            NpcSender.MapNpcAttack(npcEntityId);
    }

    private static void NpcAttackNpc(int attackerEntityId, int victimEntityId)
    {
        var world        = ServerContext.Instance.World;
        var attackerData = world.Get<NpcDataComponent>(attackerEntityId);
        var victimData   = world.Get<NpcDataComponent>(victimEntityId);
        var victimState  = world.Get<NpcStateComponent>(victimEntityId);
        var victimVitals = world.Get<VitalsComponent>(victimEntityId);
        var victimTgt    = world.Get<NpcTargetComponent>(victimEntityId);
        var atkTimer     = world.Get<NpcTimerComponent>(attackerEntityId);
        var atkTgt       = world.Get<NpcTargetComponent>(attackerEntityId);

        if (!victimState.Alive) return;

        atkTimer.AttackTimer = Environment.TickCount64;
        victimTgt.TargetEntityId = attackerEntityId;

        var attackDamage = CombatFormulas.NetDamage(
            attackerData.Data.Attribute[(byte)Attribute.Strength],
            victimData.Data.Attribute[(byte)Attribute.Resistance]);

        if (attackDamage > 0)
        {
            NpcSender.MapNpcAttack(attackerEntityId, victimData.Index.ToString(), Target.Npc);

            if (attackDamage < victimVitals.Values[(byte)Vital.Hp])
            {
                victimVitals.Values[(byte)Vital.Hp] -= attackDamage;
                NpcSender.MapNpcVitals(victimEntityId);
            }
            else
            {
                atkTgt.TargetEntityId = null;
                Died(victimEntityId);
            }
        }
        else
            NpcSender.MapNpcAttack(attackerEntityId);
    }

    /// <summary>Kills an NPC entity: drops items, resets spawn state, notifies the map.</summary>
    internal static void Died(int npcEntityId)
    {
        var world    = ServerContext.Instance.World;
        var npcData  = world.Get<NpcDataComponent>(npcEntityId);
        var npcState = world.Get<NpcStateComponent>(npcEntityId);
        var npcTgt   = world.Get<NpcTargetComponent>(npcEntityId);
        var pos      = world.Get<PositionComponent>(npcEntityId);
        var map      = GameWorld.Current.Maps[npcData.MapId];

        for (byte i = 0; i < npcData.Data.Drop.Count; i++)
        {
            var drop = npcData.Data.Drop[i];
            if (drop.Item == null) continue;
            if (MyRandom.Next(1, 99) > drop.Chance) continue;

            var itemEntity = world.Create();
            world.Add(itemEntity, new MapItemComponent
            {
                Item   = drop.Item,
                Amount = (short)drop.Amount,
                X      = pos.X,
                Y      = pos.Y,
                MapId  = npcData.MapId
            });
        }

        MapSender.MapItems(map);

        npcState.SpawnTimer          = Environment.TickCount64;
        npcTgt.TargetEntityId        = null;
        npcState.Alive               = false;
        NpcSender.MapNpcDied(npcEntityId);
    }
}
