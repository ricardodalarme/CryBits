using System;
using System.Drawing;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
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
        byte nextX = player.X, nextY = player.Y;
        NextTile(player.Direction, ref nextX, ref nextY);

        if (player.Trade != null) return;
        if (player.Shop != null) return;
        if (Environment.TickCount64 < player.AttackTimer + AttackSpeed) return;
        if (player.Map.TileBlocked(player.X, player.Y, player.Direction, false)) goto @continue;

        Character victim = player.Map.HasPlayer(nextX, nextY);
        if (victim != null)
        {
            PlayerAttackPlayer(player, (Player)victim);
            return;
        }

        victim = player.Map.HasNpc(nextX, nextY);
        if (victim != null)
        {
            PlayerAttackNpc(player, (TempNpc)victim);
            return;
        }

        @continue:
        PlayerSender.PlayerAttack(player, null);
        player.AttackTimer = Environment.TickCount64;
    }

    private static void PlayerAttackPlayer(Player attacker, Player victim)
    {
        if (victim.GettingMap) return;
        if (attacker.Map.Data.Moral == (byte)Moral.Pacific)
        {
            ChatSender.Message(attacker, "This is a peaceful area.", Color.White);
            return;
        }

        attacker.AttackTimer = Environment.TickCount64;

        var attackDamage = (short)(attacker.Damage - victim.PlayerDefense);
        if (attackDamage > 0)
        {
            PlayerSender.PlayerAttack(attacker, victim.Name, Target.Player);

            if (attackDamage < victim.Vital[(byte)Vital.Hp])
            {
                victim.Vital[(byte)Vital.Hp] -= attackDamage;
                PlayerSender.PlayerVitals(victim);
            }
            else
            {
                LevelingSystem.GiveExperience(attacker, victim.Experience / 10);
                Died(victim);
            }
        }
        else
            PlayerSender.PlayerAttack(attacker);
    }

    private static void PlayerAttackNpc(Player attacker, TempNpc victim)
    {
        if (victim.Target != attacker && !string.IsNullOrEmpty(victim.Data.SayMsg))
            ChatSender.Message(attacker, victim.Data.Name + ": " + victim.Data.SayMsg, Color.White);

        switch (victim.Data.Behaviour)
        {
            case Behaviour.Friendly: return;
            case Behaviour.ShopKeeper:
                ShopSystem.Open(attacker, victim.Data.Shop);
                return;
        }

        victim.Target = attacker;
        attacker.AttackTimer = Environment.TickCount64;

        var attackDamage = (short)(attacker.Damage - victim.Data.Attribute[(byte)Attribute.Resistance]);
        if (attackDamage > 0)
        {
            PlayerSender.PlayerAttack(attacker, victim.Index.ToString(), Target.Npc);

            if (attackDamage < victim.Vital[(byte)Vital.Hp])
            {
                victim.Vital[(byte)Vital.Hp] -= attackDamage;
                NpcSender.MapNpcVitals(victim);
            }
            else
            {
                LevelingSystem.GiveExperience(attacker, victim.Data.Experience);
                Died(victim);
            }
        }
        else
            PlayerSender.PlayerAttack(attacker);
    }

    /// <summary>Kills <paramref name="player"/>: restores vitals, penalises XP, warps to spawn.</summary>
    internal static void Died(Player player)
    {
        for (byte n = 0; n < (byte)Vital.Count; n++) player.Vital[n] = player.MaxVital(n);
        PlayerSender.PlayerVitals(player);

        player.Experience /= 10;
        PlayerSender.PlayerExperience(player);

        player.Direction = (Direction)player.Class.SpawnDirection;
        MovementSystem.Warp(player, TempMap.List.Get(player.Class.SpawnMap.Id), player.Class.SpawnX,
            player.Class.SpawnY);
    }

    /// <summary>Initiates an attack for <paramref name="npc"/> against its current target.</summary>
    internal static void Attack(TempNpc npc)
    {
        byte nextX = npc.X, nextY = npc.Y;
        NextTile(npc.Direction, ref nextX, ref nextY);

        if (!npc.Alive) return;
        if (Environment.TickCount64 < npc.AttackTimer + AttackSpeed) return;
        if (npc.Map.TileBlocked(npc.X, npc.Y, npc.Direction, false)) return;

        if (npc.Target is Player)
            NpcAttackPlayer(npc, npc.Map.HasPlayer(nextX, nextY));
        else if (npc.Target is TempNpc)
            NpcAttackNpc(npc, npc.Map.HasNpc(nextX, nextY));
    }

    private static void NpcAttackPlayer(TempNpc attacker, Player victim)
    {
        if (victim == null) return;
        if (victim.GettingMap) return;

        attacker.AttackTimer = Environment.TickCount64;

        var attackDamage = (short)(attacker.Data.Attribute[(byte)Attribute.Strength] - victim.PlayerDefense);
        if (attackDamage > 0)
        {
            NpcSender.MapNpcAttack(attacker, victim.Name, Target.Player);

            if (attackDamage < victim.Vital[(byte)Vital.Hp])
            {
                victim.Vital[(byte)Vital.Hp] -= attackDamage;
                PlayerSender.PlayerVitals(victim);
            }
            else
            {
                attacker.Target = null;
                Died(victim);
            }
        }
        else
            NpcSender.MapNpcAttack(attacker);
    }

    private static void NpcAttackNpc(TempNpc attacker, TempNpc victim)
    {
        if (victim == null) return;
        if (!victim.Alive) return;

        attacker.AttackTimer = Environment.TickCount64;
        victim.Target = attacker;

        var attackDamage = (short)(attacker.Data.Attribute[(byte)Attribute.Strength] -
                                   victim.Data.Attribute[(byte)Attribute.Resistance]);
        if (attackDamage > 0)
        {
            NpcSender.MapNpcAttack(attacker, victim.Index.ToString(), Target.Npc);

            if (attackDamage < victim.Vital[(byte)Vital.Hp])
            {
                victim.Vital[(byte)Vital.Hp] -= attackDamage;
                NpcSender.MapNpcVitals(victim);
            }
            else
            {
                attacker.Target = null;
                Died(victim);
            }
        }
        else
            NpcSender.MapNpcAttack(attacker);
    }

    /// <summary>Kills <paramref name="npc"/>: drops items, resets spawn state, notifies the map.</summary>
    internal static void Died(TempNpc npc)
    {
        for (byte i = 0; i < npc.Data.Drop.Count; i++)
            if (npc.Data.Drop[i].Item != null)
                if (MyRandom.Next(1, 99) <= npc.Data.Drop[i].Chance)
                    npc.Map.Item.Add(new TempMapItems(npc.Data.Drop[i].Item, npc.Data.Drop[i].Amount, npc.X, npc.Y));

        MapSender.MapItems(npc.Map);

        npc.SpawnTimer = Environment.TickCount64;
        npc.Target = null;
        npc.Alive = false;
        NpcSender.MapNpcDied(npc);
    }
}
