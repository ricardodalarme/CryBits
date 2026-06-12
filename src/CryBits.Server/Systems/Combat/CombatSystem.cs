using CryBits.Definitions.Common;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.Events;
using CryBits.Server.Systems.Shops;
using CryBits.Simulation.Formulas;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using System;
using System.Drawing;
using static CryBits.Globals;
using static CryBits.Utils.RandomUtils;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Server.Systems.Combat;

internal sealed class CombatSystem(
    CombatSender combatSender,
    PlayerSender playerSender,
    NpcSender npcSender,
    ChatSender chatSender) : ISimulationSystem
{
    public static CombatSystem Instance { get; } = new(
        CombatSender.Instance,
        PlayerSender.Instance,
        NpcSender.Instance,
        ChatSender.Instance);

    internal void Attack(Player player)
    {
        byte nextX = player.X, nextY = player.Y;
        player.Direction.NextTile(ref nextX, ref nextY);

        if (player.Trade != null) return;
        if (player.Shop != null) return;
        if (Environment.TickCount64 < player.AttackTimer + AttackSpeed) return;
        if (player.MapInstance.TileBlocked(player.X, player.Y, player.Direction, false)) goto @continue;

        Character victim = player.MapInstance.HasPlayer(nextX, nextY);
        if (victim != null)
        {
            PlayerAttackPlayer(player, (Player)victim);
            return;
        }

        victim = player.MapInstance.HasNpc(nextX, nextY);
        if (victim != null)
        {
            PlayerAttackNpc(player, (NpcInstance)victim);
            return;
        }

    @continue:
        combatSender.Attack(player.MapInstance.Id, player.Id);
        player.AttackTimer = Environment.TickCount64;
    }

    private void PlayerAttackPlayer(Player attacker, Player victim)
    {
        if (victim.GettingMap) return;
        if (attacker.MapInstance.Data.Moral == (byte)Moral.Pacific)
        {
            chatSender.Message(attacker, "This is a peaceful area.", Color.White);
            return;
        }

        attacker.AttackTimer = Environment.TickCount64;

        var attackDamage = CombatFormulas.NetDamage(attacker.Damage, victim.PlayerDefense);
        if (attackDamage > 0)
        {
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id, victim.Id);

            if (attackDamage < victim.Vital[(byte)Vital.Hp])
            {
                victim.Vital[(byte)Vital.Hp] -= attackDamage;
                playerSender.PlayerVitals(victim);
            }
            else
            {
                GameWorld.Current.CurrentTick?.Events.Emit(new EntityDiedEvent { Entity = victim, Source = attacker });
            }
        }
        else
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id);
    }

    private void PlayerAttackNpc(Player attacker, NpcInstance victim)
    {
        if (victim.Target != attacker && !string.IsNullOrEmpty(victim.Data.SayMsg))
            chatSender.Message(attacker, victim.Data.Name + ": " + victim.Data.SayMsg, Color.White);

        switch (victim.Data.Behaviour)
        {
            case Behaviour.Friendly: return;
            case Behaviour.ShopKeeper:
                GameWorld.Current.CurrentTick?.Events.Emit(new NpcAttackedEvent { Attacker = attacker, Npc = victim });
                return;
        }

        victim.Target = attacker;
        attacker.AttackTimer = Environment.TickCount64;

        var attackDamage = CombatFormulas.NetDamage(attacker.Damage, (short)victim.Data.Attribute[(byte)Attribute.Resistance]);
        if (attackDamage > 0)
        {
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id, victim.Id);

            if (attackDamage < victim.Vital[(byte)Vital.Hp])
            {
                victim.Vital[(byte)Vital.Hp] -= attackDamage;
                npcSender.MapNpcVitals(victim);
            }
            else
            {
                Died(victim);
                GameWorld.Current.CurrentTick?.Events.Emit(new EntityDiedEvent { Entity = victim, Source = attacker });
            }
        }
        else
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id);
    }

    internal void Attack(NpcInstance npcInstance)
    {
        byte nextX = npcInstance.X, nextY = npcInstance.Y;
        npcInstance.Direction.NextTile(ref nextX, ref nextY);

        if (!npcInstance.Alive) return;
        if (Environment.TickCount64 < npcInstance.AttackTimer + AttackSpeed) return;
        if (npcInstance.MapInstance.TileBlocked(npcInstance.X, npcInstance.Y, npcInstance.Direction, false)) return;

        if (npcInstance.Target is Player)
            NpcAttackPlayer(npcInstance, npcInstance.MapInstance.HasPlayer(nextX, nextY));
        else if (npcInstance.Target is NpcInstance)
            NpcAttackNpc(npcInstance, npcInstance.MapInstance.HasNpc(nextX, nextY));
    }

    private void NpcAttackPlayer(NpcInstance attacker, Player victim)
    {
        if (victim == null) return;
        if (victim.GettingMap) return;

        attacker.AttackTimer = Environment.TickCount64;

        var attackDamage = CombatFormulas.NetDamage((short)attacker.Data.Attribute[(byte)Attribute.Strength], victim.PlayerDefense);
        if (attackDamage > 0)
        {
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id, victim.Id);

            if (attackDamage < victim.Vital[(byte)Vital.Hp])
            {
                victim.Vital[(byte)Vital.Hp] -= attackDamage;
                playerSender.PlayerVitals(victim);
            }
            else
            {
                attacker.Target = null;
                GameWorld.Current.CurrentTick?.Events.Emit(new EntityDiedEvent { Entity = victim, Source = attacker });
            }
        }
        else
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id);
    }

    private void NpcAttackNpc(NpcInstance attacker, NpcInstance victim)
    {
        if (victim == null) return;
        if (!victim.Alive) return;

        attacker.AttackTimer = Environment.TickCount64;
        victim.Target = attacker;

        var attackDamage = CombatFormulas.NetDamage(
            (short)attacker.Data.Attribute[(byte)Attribute.Strength],
            (short)victim.Data.Attribute[(byte)Attribute.Resistance]);
        if (attackDamage > 0)
        {
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id, victim.Id);

            if (attackDamage < victim.Vital[(byte)Vital.Hp])
            {
                victim.Vital[(byte)Vital.Hp] -= attackDamage;
                npcSender.MapNpcVitals(victim);
            }
            else
            {
                attacker.Target = null;
                Died(victim);
                GameWorld.Current.CurrentTick?.Events.Emit(new EntityDiedEvent { Entity = victim, Source = attacker });
            }
        }
        else
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id);
    }

    internal void Died(NpcInstance npcInstance)
    {
        for (byte i = 0; i < npcInstance.Data.Drop.Count; i++)
            if (npcInstance.Data.Drop[i].Item != null)
                if (MyRandom.Next(1, 99) <= npcInstance.Data.Drop[i].Chance)
                    npcInstance.MapInstance.Item.Add(new MapItemInstance(npcInstance.Data.Drop[i].Item, npcInstance.Data.Drop[i].Amount, npcInstance.X, npcInstance.Y));

        MapSender.Instance.MapItems(npcInstance.MapInstance);

        npcInstance.Alive = false;
        npcInstance.Target = null;
        npcInstance.SpawnTimer = Environment.TickCount64;
        npcSender.MapNpcDied(npcInstance);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers()) continue;

            foreach (var npc in map.Npc)
            {
                if (!npc.Alive) continue;
                if (npc.Target == null) continue;
                Attack(npc);
            }
        }
    }
}
