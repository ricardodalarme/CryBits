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
using static CryBits.Simulation.SimulationConstants;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Simulation.Systems.Combat;

public sealed class CombatSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var entityId in world.All)
            world.Remove<AttackHit>(entityId);

        foreach (var intent in tick.Intents.All)
            if (intent is AttackIntent atk)
                Attack(world, tick, atk);
    }

    private void Attack(World world, Tick tick, AttackIntent intent)
    {
        if (!world.IsAlive(intent.SourceEntityId)) return;

        var pos = world.Get<Position>(intent.SourceEntityId)!;
        if (!world.MapDefs.TryGetValue(pos.MapId, out var mapDef))
            return;

        if (world.Get<ShopState>(intent.SourceEntityId)?.ShopId != null) return;
        var cooldown = world.Get<AttackCooldown>(intent.SourceEntityId)!;
        if (tick.TickNumber < cooldown.NextAllowedTick + AttackSpeedTicks) return;

        var victimId = intent.TargetId;
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

            victimId = ChunkGrid.FindAt<Vitals>(world, pos.MapId, nextX, nextY);

            if (victimId == null)
            {
                MissAttack(world, intent.SourceEntityId);
                return;
            }
        }

        if (!world.IsAlive(victimId.Value)) return;

        if (world.Has<PlayerTag>(victimId.Value) && mapDef.Moral == Moral.Pacific)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, intent.SourceEntityId, "This is a peaceful area.",
                ChatColors.White));
            return;
        }

        if (world.Has<NpcTag>(victimId.Value))
        {
            var victimNpcState = world.Get<NpcState>(victimId.Value)!;
            var victimData = world.Catalog.Npcs.Get(victimNpcState.NpcDefId);
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

        DealDamage(world, tick, intent.SourceEntityId, victimId.Value);
    }

    private void DealDamage(World world, Tick tick, EntityId attackerId, EntityId victimId)
    {
        if (!world.IsAlive(attackerId) || !world.IsAlive(victimId)) return;
        var attackerPos = world.Get<Position>(attackerId)!;
        var victimVitals = world.Get<Vitals>(victimId)!;
        var attackerAttrs = world.Get<AttributesComponent>(attackerId)!;
        var victimAttrs = world.Get<AttributesComponent>(victimId);

        world.Update<AttackCooldown>(attackerId, c => new AttackCooldown(NextAllowedTick: tick.TickNumber));

        var weaponDamage = GetWeaponDamage(world, attackerId);
        var attackerDamage = CombatFormulas.BaseDamage(attackerAttrs.Values[(byte)Attribute.Strength], weaponDamage);
        var victimDefense = victimAttrs != null
            ? CombatFormulas.BaseDefense(victimAttrs.Values[(byte)Attribute.Resistance])
            : (short)0;
        var netDamage = CombatFormulas.NetDamage(attackerDamage, victimDefense);

        tick.Events.Emit(new CombatAttackEvent(tick.TickNumber, attackerId, victimId, attackerPos.MapId,
            netDamage > 0));

        var victimPos = world.Get<Position>(victimId);
        if (netDamage > 0)
        {
            if (netDamage < victimVitals.Hp)
            {
                world.Update<Vitals>(victimId, v => v with { Hp = (short)(v.Hp - netDamage) });
            }
            else
            {
                if (world.Has<PlayerTag>(victimId))
                {
                    tick.Events.Emit(new PlayerDiedEvent(tick.TickNumber, victimId, attackerId));
                }
                else if (world.Has<NpcTag>(victimId))
                {
                    var npcState = world.Get<NpcState>(victimId)!;
                    tick.Events.Emit(new NpcDiedEvent(tick.TickNumber, victimId, attackerPos.MapId, npcState.NpcDefId,
                        npcState.Index, attackerId));
                }
            }
        }

        world.Set(attackerId, new AttackHit(victimId.Value, victimPos?.X ?? 0, victimPos?.Y ?? 0));
    }

    private void MissAttack(World world, EntityId entityId)
    {
        world.Set(entityId, new AttackHit(null));
    }

    private static short GetWeaponDamage(World world, EntityId e)
    {
        var equip = world.Get<EquipmentState>(e);
        if (equip == null || equip.Slots[(byte)Equipment.Weapon] == Guid.Empty)
            return 0;
        return world.Catalog.Items.Get(equip.Slots[(byte)Equipment.Weapon])?.WeaponDamage ?? 0;
    }
}
