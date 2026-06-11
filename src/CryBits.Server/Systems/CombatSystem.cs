using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Formulas;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using System;
using System.Drawing;
using static CryBits.Globals;
using static CryBits.Utils.DirectionUtils;
using static CryBits.Utils.RandomUtils;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Server.Systems;

/// <summary>
/// Owns all combat resolution logic for players and NPCs.
/// MovementSystem and ShopSystem are accessed via their Instances to avoid circular constructor dependencies.
/// </summary>
internal sealed class CombatSystem(
    CombatSender combatSender,
    PlayerSender playerSender,
    NpcSender npcSender,
    LevelingSystem levelingSystem,
    ChatSender chatSender)
{
    public static CombatSystem Instance { get; } = new(
        CombatSender.Instance,
        PlayerSender.Instance,
        NpcSender.Instance,
        LevelingSystem.Instance,
        ChatSender.Instance);

    /// <summary>Initiates an attack for <paramref name="player"/> against whatever is in front of them.</summary>
    internal void Attack(Player player)
    {
        byte nextX = player.X, nextY = player.Y;
        NextTile(player.Direction, ref nextX, ref nextY);

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
                levelingSystem.GiveExperience(attacker, victim.Experience / 10);
                Died(victim);
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
                ShopSystem.Instance.Open(attacker, victim.Data.Shop);
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
                levelingSystem.GiveExperience(attacker, victim.Data.Experience);
                Died(victim);
            }
        }
        else
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id);
    }

    /// <summary>Kills <paramref name="player"/>: restores vitals, penalises XP, warps to spawn.</summary>
    internal void Died(Player player)
    {
        for (byte n = 0; n < (byte)Vital.Count; n++) player.Vital[n] = player.MaxVital(n);
        playerSender.PlayerVitals(player);

        player.Experience /= 10;
        playerSender.PlayerExperience(player);

        player.Direction = (Direction)player.Class.SpawnDirection;
        MovementSystem.Instance.Warp(player, GameWorld.Current.Maps.Get(player.Class.SpawnMap.Id), player.Class.SpawnX,
            player.Class.SpawnY);
    }

    /// <summary>Initiates an attack for <paramref name="npcInstance"/> against its current target.</summary>
    internal void Attack(NpcInstance npcInstance)
    {
        byte nextX = npcInstance.X, nextY = npcInstance.Y;
        NextTile(npcInstance.Direction, ref nextX, ref nextY);

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
                Died(victim);
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
            }
        }
        else
            combatSender.Attack(attacker.MapInstance.Id, attacker.Id);
    }

    /// <summary>Kills <paramref name="npcInstance"/>: drops items, resets spawn state, notifies the map.</summary>
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
}
