using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Formulas;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;
using CryBits.Simulation.State;
using static CryBits.Simulation.SimulationConstants;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Simulation.Systems.Combat;

public sealed class CombatSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var state in world.All)
            state.Remove<AttackHit>();

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
        if (!world.MapDefs.TryGetValue(pos.MapId, out var mapDef))
            return;

        if (attackerE.Get<TradeState>()?.Partner != null) return;
        if (attackerE.Get<ShopState>()?.ShopId != null) return;
        var cooldown = attackerE.Get<AttackCooldown>()!;
        if (tick.TickNumber < cooldown.NextAllowedTick + (int)AttackSpeedTicks) return;

        EntityId? victimId = intent.TargetId;
        if (victimId == null)
        {
            var dir = pos.Direction;
            var nextX = dir == Direction.Right ? pos.X + 1 : dir == Direction.Left ? pos.X - 1 : pos.X;
            var nextY = dir == Direction.Down ? pos.Y + 1 : dir == Direction.Up ? pos.Y - 1 : pos.Y;

            if (ChunkGrid.IsTileBlocked(world, pos.MapId, nextX, nextY))
            {
                MissAttack(world, intent.SourceEntityId);
                return;
            }

            victimId = ChunkGrid.FindEntityAtTile(world, pos.MapId, nextX, nextY);

            if (victimId == null)
            {
                MissAttack(world, intent.SourceEntityId);
                return;
            }
        }

        var victimE = world.Entities.Get(victimId.Value);
        if (victimE == null) return;

        if (victimE.Has<PlayerTag>() && mapDef.Moral == Moral.Pacific)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, intent.SourceEntityId, "This is a peaceful area.", ChatColors.White));
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
                    tick.Events.Emit(new NpcAttackedEvent(tick.TickNumber, intent.SourceEntityId, victimId.Value));
                    return;
                }
            }
        }

        DealDamage(world, tick, intent.SourceEntityId, victimId.Value, mapDef, cooldown);
    }

    private void DealDamage(World world, Tick tick,
        EntityId attackerId, EntityId victimId, Map mapDef, AttackCooldown cooldown)
    {
        var attackerE = world.Entities.Get(attackerId);
        var victimE = world.Entities.Get(victimId);
        if (attackerE == null || victimE == null) return;
        var attackerPos = attackerE.Get<Position>()!;
        var victimVitals = victimE.Get<Vitals>()!;
        var attackerAttrs = attackerE.Get<AttributesComponent>()!;
        var victimAttrs = victimE.Get<AttributesComponent>();

        world.Update<AttackCooldown>(attackerId, c => c with { NextAllowedTick = tick.TickNumber });

        var weaponDamage = GetWeaponDamage(attackerE);
        var attackerDamage = CombatFormulas.BaseDamage(attackerAttrs.Values[(byte)Attribute.Strength], weaponDamage);
        var victimDefense = victimAttrs != null
            ? CombatFormulas.BaseDefense(victimAttrs.Values[(byte)Attribute.Resistance])
            : (short)0;
        var netDamage = CombatFormulas.NetDamage(attackerDamage, victimDefense);

        var victimPos = victimE.Get<Position>();
        if (netDamage > 0)
        {
            if (netDamage < victimVitals.Hp)
                world.Update<Vitals>(victimId, v => v with { Hp = (short)(v.Hp - netDamage) });
            else
            {
                if (victimE.Has<PlayerTag>())
                    tick.Events.Emit(new PlayerDiedEvent(tick.TickNumber, victimId, attackerId));
                else if (victimE.Has<NpcTag>())
                {
                    var npcState = victimE.Get<NpcState>()!;
                    tick.Events.Emit(new NpcDiedEvent(tick.TickNumber, victimId, attackerPos.MapId, npcState.NpcDefId, npcState.Index, attackerId));
                }
            }
        }

        world.Set(attackerId, new AttackHit(victimId.Value, victimPos?.X ?? 0, victimPos?.Y ?? 0));
    }

    private void MissAttack(World world, EntityId entityId)
    {
        world.Set(entityId, new AttackHit(null));
    }

    private short GetWeaponDamage(EntityState e)
    {
        var equip = e.Get<EquipmentState>();
        if (equip == null || equip.Slots[(byte)Equipment.Weapon] == Guid.Empty)
            return 0;
        return catalog.Items.Get(equip.Slots[(byte)Equipment.Weapon])?.WeaponDamage ?? 0;
    }
}
