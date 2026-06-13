using CryBits.Definitions.Catalog;
using CryBits.Definitions.Utils;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Formulas;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using System;
using static CryBits.Simulation.SimulationConstants;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Simulation.Systems.Combat;

public sealed class CombatSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is AttackIntent atk)
                Attack(world, tick, atk);
        }
    }

    private void Attack(World world, Tick tick, AttackIntent intent)
    {
        var attackerE = world.Entities.Get(intent.SourceEntityId);
        if (attackerE == null) return;

        var pos = attackerE.Get<Position>()!;
        var map = world.Maps.Get(pos.MapId)!;
        if (map == null) return;

        if (attackerE.Get<TradeState>()?.Partner != null) return;
        if (attackerE.Get<ShopState>()?.ShopId != null) return;
        var cooldown = attackerE.Get<AttackCooldown>()!;
        if (tick.TickNumber < cooldown.NextAllowedTick + AttackSpeedTicks) return;

        EntityId? victimId = intent.TargetId;
        if (victimId == null)
        {
            var (nextX, nextY) = pos.Direction.NextTile(pos.X, pos.Y);
            if (map.TileBlocked(pos.X, pos.Y, pos.Direction, world.Entities, false))
                {
                    MissAttack(tick, intent.SourceEntityId, pos, cooldown);
                    return;
                }

            victimId = map.HasPlayer(nextX, nextY, world.Entities)
                    ?? map.HasNpc(nextX, nextY, world.Entities);

            if (victimId == null)
            {
                MissAttack(tick, intent.SourceEntityId, pos, cooldown);
                return;
            }
        }

        var victimE = world.Entities.Get(victimId.Value);
        if (victimE == null) return;

        if (victimE.Has<PlayerTag>() && map.Data.Moral == (byte)Moral.Pacific)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = intent.SourceEntityId, Text = "This is a peaceful area.", ColorArgb = ChatColors.White });
            return;
        }

        if (victimE.Has<NpcTag>())
        {
            var victimNpcState = victimE.Get<NpcState>()!;
            var victimData = catalog.Npcs.Get(victimNpcState.NpcDefId);
            if (victimData != null)
            {
                if (victimData.Behaviour == Behaviour.Friendly) return;
                if (victimData.Behaviour == Behaviour.ShopKeeper)
                {
                    tick.Events.Emit(new NpcAttackedEvent { AttackerId = intent.SourceEntityId, NpcInstanceId = victimId.Value });
                    return;
                }
            }
        }

        DealDamage(world, tick, intent.SourceEntityId, victimId.Value, map, cooldown);
    }

    private void DealDamage(World world, Tick tick,
        EntityId attackerId, EntityId victimId, MapState map, AttackCooldown cooldown)
    {
        var attackerE = world.Entities.Get(attackerId)!;
        var victimE = world.Entities.Get(victimId)!;
        var attackerPos = attackerE.Get<Position>()!;
        var victimVitals = victimE.Get<Vitals>()!;
        var attackerStats = attackerE.Get<StatBlock>()!;
        var victimStats = victimE.Get<StatBlock>();

        cooldown.NextAllowedTick = tick.TickNumber;

        var weaponDamage = GetWeaponDamage(attackerE);
        var attackerDamage = CombatFormulas.BaseDamage(attackerStats.Attribute[(byte)Attribute.Strength], weaponDamage);
        var victimDefense = victimStats != null
            ? CombatFormulas.BaseDefense(victimStats.Attribute[(byte)Attribute.Resistance])
            : (short)0;
        var netDamage = CombatFormulas.NetDamage(attackerDamage, victimDefense);

        if (netDamage > 0)
        {
            tick.Events.Emit(new CombatAttackEvent
            {
                AttackerId = attackerId,
                VictimId = victimId,
                MapId = attackerPos.MapId,
                Hit = true
            });

            if (netDamage < victimVitals.Hp)
            {
                victimVitals.Hp -= netDamage;
                world.Dirty.Mark<Vitals>(victimId);
            }
            else
            {
                if (victimE.Has<PlayerTag>())
                    tick.Events.Emit(new PlayerDiedEvent { EntityId = victimId, SourceId = attackerId });
                else if (victimE.Has<NpcTag>())
                {
                    var npcState = victimE.Get<NpcState>()!;
                    tick.Events.Emit(new NpcDiedEvent
                    {
                        EntityId = victimId,
                        MapId = attackerPos.MapId,
                        NpcDefId = npcState.NpcDefId,
                        NpcIndex = npcState.Index,
                        SourceId = attackerId
                    });
                }
            }
        }
        else
            tick.Events.Emit(new CombatAttackEvent { AttackerId = attackerId, VictimId = victimId, MapId = attackerPos.MapId, Hit = false });
    }

    private static void MissAttack(Tick tick, EntityId entityId, Position pos, AttackCooldown cooldown)
    {
        tick.Events.Emit(new CombatAttackEvent { AttackerId = entityId, MapId = pos.MapId, Hit = false });
        cooldown.NextAllowedTick = tick.TickNumber;
    }

    private short GetWeaponDamage(EntityState e)
    {
        var equip = e.Get<EquipmentState>();
        if (equip == null || equip.Slots[(byte)Equipment.Weapon] == Guid.Empty)
            return 0;
        return catalog.Items.Get(equip.Slots[(byte)Equipment.Weapon])?.WeaponDamage ?? 0;
    }
}
