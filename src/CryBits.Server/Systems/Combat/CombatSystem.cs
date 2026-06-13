using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using CryBits.Simulation.Formulas;
using System;
using System.Drawing;
using static CryBits.Definitions.Globals;
using Attribute = CryBits.Definitions.Characters.Attribute;
using CryBits.Simulation.Core;
using CryBits.Simulation.Entities;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Server.Systems.Combat;

internal sealed class CombatSystem(
    CombatSender combatSender) : ISimulationSystem
{
    public static CombatSystem Instance { get; } = new(
        CombatSender.Instance);

    internal void Attack(EntityId entityId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var combat = e.Get<CombatState>()!;
        var trade = e.Get<TradeState>();
        var shop = e.Get<ShopState>();
        var map = world.Maps.Get(pos.MapId)!;

        byte nextX = pos.X, nextY = pos.Y;
        pos.Direction.NextTile(ref nextX, ref nextY);

        if (trade?.Partner != null) return;
        if (shop?.ShopId != null) return;
        if (Environment.TickCount64 < combat.AttackTimer + AttackSpeed) return;
        if (map.TileBlocked(pos.X, pos.Y, pos.Direction, world.Entities, false)) goto @continue;

        var victim = map.HasPlayer(nextX, nextY, world.Entities);
        if (victim.HasValue)
        {
            PlayerAttackPlayer(entityId, victim.Value);
            return;
        }

        victim = map.HasNpc(nextX, nextY, world.Entities);
        if (victim.HasValue)
        {
            PlayerAttackNpc(entityId, victim.Value);
            return;
        }

    @continue:
        combatSender.Attack(pos.MapId, entityId.Value);
        combat.AttackTimer = Environment.TickCount64;
        world.Dirty.Mark<Position>(entityId);
    }

    private void PlayerAttackPlayer(EntityId attackerId, EntityId victimId)
    {
        var world = GameWorld.Current;
        var attackerE = world.Entities.Get(attackerId)!;
        var victimE = world.Entities.Get(victimId)!;
        var attackerPos = attackerE.Get<Position>()!;
        var victimPos = victimE.Get<Position>()!;
        var attackerCombat = attackerE.Get<CombatState>()!;
        var victimCombat = victimE.Get<CombatState>()!;
        var attackerStats = attackerE.Get<StatBlock>()!;
        var victimStats = victimE.Get<StatBlock>()!;
        var victimVitals = victimE.Get<Vitals>()!;
        var attackerEquip = attackerE.Get<EquipmentState>()!;
        var catalog = DefinitionCatalog.Instance;
        var map = world.Maps.Get(attackerPos.MapId)!;

        if (victimCombat.GettingMap) return;
        if (map.Data.Moral == (byte)Moral.Pacific)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = attackerId.Value, Text = "This is a peaceful area.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        attackerCombat.AttackTimer = Environment.TickCount64;
        world.Dirty.Mark<Position>(attackerId);

        var weaponDamage = attackerEquip.Slots[(byte)Equipment.Weapon] != Guid.Empty
            ? catalog.Items.Get(attackerEquip.Slots[(byte)Equipment.Weapon])?.WeaponDamage ?? 0
            : (short)0;
        var attackerDamage = CombatFormulas.PlayerDamage(attackerStats.Attribute[(byte)Attribute.Strength], weaponDamage);
        var victimDefense = CombatFormulas.PlayerDefense(victimStats.Attribute[(byte)Attribute.Resistance]);
        var attackDamage = CombatFormulas.NetDamage(attackerDamage, victimDefense);
        if (attackDamage > 0)
        {
            combatSender.Attack(attackerPos.MapId, attackerId.Value, victimId.Value);

            if (attackDamage < victimVitals.Hp)
            {
                victimVitals.Hp -= attackDamage;
                world.Dirty.Mark<Vitals>(victimId);
            }
            else
            {
                world.CurrentTick?.Events.Emit(new EntityDiedEvent { EntityId = victimId.Value, EntityIsPlayer = true, SourceId = attackerId.Value, SourceIsPlayer = true });
            }
        }
        else
            combatSender.Attack(attackerPos.MapId, attackerId.Value);
    }

    private void PlayerAttackNpc(EntityId attackerId, EntityId victimId)
    {
        var world = GameWorld.Current;
        var attackerE = world.Entities.Get(attackerId)!;
        var victimE = world.Entities.Get(victimId)!;
        var attackerPos = attackerE.Get<Position>()!;
        var attackerCombat = attackerE.Get<CombatState>()!;
        var attackerStats = attackerE.Get<StatBlock>()!;
        var attackerEquip = attackerE.Get<EquipmentState>()!;
        var victimNpcState = victimE.Get<NpcState>()!;
        var victimVitals = victimE.Get<Vitals>()!;
        var catalog = DefinitionCatalog.Instance;
        var npcData = catalog.Npcs.Get(victimNpcState.NpcDefId);
        var map = world.Maps.Get(attackerPos.MapId)!;

        if (victimNpcState.TargetId != attackerId && !string.IsNullOrEmpty(npcData.SayMsg))
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = attackerId.Value, Text = npcData.Name + ": " + npcData.SayMsg, ColorArgb = Color.White.ToArgb() });

        switch (npcData.Behaviour)
        {
            case Behaviour.Friendly: return;
            case Behaviour.ShopKeeper:
                world.CurrentTick?.Events.Emit(new NpcAttackedEvent { AttackerId = attackerId.Value, NpcInstanceId = victimId.Value });
                return;
        }

        victimNpcState.TargetId = attackerId;
        world.Dirty.Mark<NpcState>(victimId);
        attackerCombat.AttackTimer = Environment.TickCount64;
        world.Dirty.Mark<Position>(attackerId);

        var weaponDamage = attackerEquip.Slots[(byte)Equipment.Weapon] != Guid.Empty
            ? catalog.Items.Get(attackerEquip.Slots[(byte)Equipment.Weapon])?.WeaponDamage ?? 0
            : (short)0;
        var attackerDamage = CombatFormulas.PlayerDamage(attackerStats.Attribute[(byte)Attribute.Strength], weaponDamage);
        var attackDamage = CombatFormulas.NetDamage(attackerDamage, npcData.Attribute[(byte)Attribute.Resistance]);
        if (attackDamage > 0)
        {
            combatSender.Attack(attackerPos.MapId, attackerId.Value, victimId.Value);

            if (attackDamage < victimVitals.Hp)
            {
                victimVitals.Hp -= attackDamage;
                world.Dirty.Mark<Vitals>(victimId);
            }
            else
            {
                Died(victimId);
                world.CurrentTick?.Events.Emit(new EntityDiedEvent { EntityId = victimId.Value, EntityIsPlayer = false, SourceId = attackerId.Value, SourceIsPlayer = true });
            }
        }
        else
            combatSender.Attack(attackerPos.MapId, attackerId.Value);
    }

    internal void AttackNpc(EntityId npcId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;

        byte nextX = pos.X, nextY = pos.Y;
        pos.Direction.NextTile(ref nextX, ref nextY);

        if (!npcState.Alive) return;
        if (Environment.TickCount64 < npcState.AttackTimer + AttackSpeed) return;
        if (map.TileBlocked(pos.X, pos.Y, pos.Direction, world.Entities, false)) return;

        if (npcState.TargetId.HasValue)
        {
            var targetE = world.Entities.Get(npcState.TargetId.Value);
            if (targetE != null)
            {
                if (targetE.Has<PlayerTag>())
                    NpcAttackPlayer(npcId, npcState.TargetId.Value);
                else if (targetE.Has<NpcTag>())
                    NpcAttackNpc(npcId, npcState.TargetId.Value);
            }
        }
    }

    private void NpcAttackPlayer(EntityId attackerId, EntityId victimId)
    {
        var world = GameWorld.Current;
        var attackerE = world.Entities.Get(attackerId)!;
        var victimE = world.Entities.Get(victimId)!;
        var attackerNpcState = attackerE.Get<NpcState>()!;
        var attackerPos = attackerE.Get<Position>()!;
        var victimCombat = victimE.Get<CombatState>()!;
        var victimVitals = victimE.Get<Vitals>()!;
        var victimStats = victimE.Get<StatBlock>()!;
        var catalog = DefinitionCatalog.Instance;
        var npcData = catalog.Npcs.Get(attackerNpcState.NpcDefId);

        if (!victimE.Has<PlayerTag>()) return;
        if (victimCombat.GettingMap) return;

        attackerNpcState.AttackTimer = Environment.TickCount64;
        world.Dirty.Mark<Position>(attackerId);

        var attackDamage = CombatFormulas.NetDamage(npcData.Attribute[(byte)Attribute.Strength], CombatFormulas.PlayerDefense(victimStats.Attribute[(byte)Attribute.Resistance]));
        if (attackDamage > 0)
        {
            combatSender.Attack(attackerPos.MapId, attackerId.Value, victimId.Value);

            if (attackDamage < victimVitals.Hp)
            {
                victimVitals.Hp -= attackDamage;
                world.Dirty.Mark<Vitals>(victimId);
            }
            else
            {
                attackerNpcState.TargetId = null;
                world.Dirty.Mark<NpcState>(attackerId);
                world.CurrentTick?.Events.Emit(new EntityDiedEvent { EntityId = victimId.Value, EntityIsPlayer = true, SourceId = attackerId.Value, SourceIsPlayer = false });
            }
        }
        else
            combatSender.Attack(attackerPos.MapId, attackerId.Value);
    }

    private void NpcAttackNpc(EntityId attackerId, EntityId victimId)
    {
        var world = GameWorld.Current;
        var attackerE = world.Entities.Get(attackerId)!;
        var victimE = world.Entities.Get(victimId)!;
        var attackerNpcState = attackerE.Get<NpcState>()!;
        var attackerPos = attackerE.Get<Position>()!;
        var victimNpcState = victimE.Get<NpcState>()!;
        var victimVitals = victimE.Get<Vitals>()!;
        var catalog = DefinitionCatalog.Instance;
        var attackerData = catalog.Npcs.Get(attackerNpcState.NpcDefId);
        var victimData = catalog.Npcs.Get(victimNpcState.NpcDefId);

        if (!victimE.Has<NpcTag>()) return;
        if (!victimNpcState.Alive) return;

        attackerNpcState.AttackTimer = Environment.TickCount64;
        world.Dirty.Mark<Position>(attackerId);
        victimNpcState.TargetId = attackerId;
        world.Dirty.Mark<NpcState>(victimId);

        var attackDamage = CombatFormulas.NetDamage(
            attackerData.Attribute[(byte)Attribute.Strength],
            victimData.Attribute[(byte)Attribute.Resistance]);
        if (attackDamage > 0)
        {
            combatSender.Attack(attackerPos.MapId, attackerId.Value, victimId.Value);

            if (attackDamage < victimVitals.Hp)
            {
                victimVitals.Hp -= attackDamage;
                world.Dirty.Mark<Vitals>(victimId);
            }
            else
            {
                attackerNpcState.TargetId = null;
                world.Dirty.Mark<NpcState>(attackerId);
                Died(victimId);
                world.CurrentTick?.Events.Emit(new EntityDiedEvent { EntityId = victimId.Value, EntityIsPlayer = false, SourceId = attackerId.Value, SourceIsPlayer = false });
            }
        }
        else
            combatSender.Attack(attackerPos.MapId, attackerId.Value);
    }

    internal void Died(EntityId npcId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(npcId)!;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = DefinitionCatalog.Instance.Npcs.Get(npcState.NpcDefId);
        var map = world.Maps.Get(pos.MapId)!;

        for (byte i = 0; i < npcData.Drop.Count; i++)
            if (npcData.Drop[i].ItemId != Guid.Empty)
                if (Random.Shared.Next(1, 99) <= npcData.Drop[i].Chance)
                    map.Item.Add(new GroundItem(npcData.Drop[i].ItemId, npcData.Drop[i].Amount, pos.X, pos.Y));

        MapSender.Instance.MapItems(map);

        npcState.Alive = false;
        npcState.TargetId = null;
        npcState.SpawnTimer = Environment.TickCount64;
        world.Dirty.Mark<NpcState>(npcId);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is AttackIntent atk)
                Attack(atk.SourceEntityId);
        }

        foreach (var mapEntry in world.Maps)
        {
            var map = mapEntry.Value;
            if (!map.HasPlayers(world.Entities)) continue;

            foreach (var npcId in map.NpcIds)
            {
                var e = world.Entities.Get(npcId);
                if (e == null) continue;
                var npcState = e.Get<NpcState>();
                if (npcState == null || !npcState.Alive) continue;
                if (!npcState.TargetId.HasValue) continue;
                AttackNpc(npcId);
            }
        }
    }
}
