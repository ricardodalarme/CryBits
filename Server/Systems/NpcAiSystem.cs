using System;
using System.Drawing;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Network;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using static CryBits.Utils;

namespace CryBits.Server.Systems;

/// <summary>Tick-driven system that runs NPC spawn, targeting, movement and delegates combat.</summary>
internal static class NpcAiSystem
{
    /// <summary>Runs one AI tick for <paramref name="npcInstance"/>: spawn check, regen, targeting, movement, attack.</summary>
    internal static void Tick(NpcInstance npcInstance)
    {
        if (!npcInstance.Alive)
        {
            if (Environment.TickCount64 > npcInstance.SpawnTimer + npcInstance.Data.SpawnTime * 1000) Spawn(npcInstance);
            return;
        }

        RegenerationSystem.Tick(npcInstance);

        byte targetX = 0, targetY = 0;
        var canMove = new bool[(byte)Direction.Count];
        var moved = false;
        var move = false;

        // Target acquisition — attack on sight
        if (npcInstance.Data.Behaviour == Behaviour.AttackOnSight)
        {
            short distance;

            // Scan for a player in range
            if (npcInstance.Target == null)
                foreach (var account in GameWorld.Current.Accounts)
                {
                    if (!account.IsPlaying) continue;
                    if (account.Character.MapInstance != npcInstance.MapInstance) continue;

                    distance = (short)Math.Sqrt(Math.Pow(npcInstance.X - account.Character.X, 2) +
                                                Math.Pow(npcInstance.Y - account.Character.Y, 2));
                    if (distance <= npcInstance.Data.Sight)
                    {
                        npcInstance.Target = account.Character;
                        if (!string.IsNullOrEmpty(npcInstance.Data.SayMsg))
                            ChatSender.Message(account.Character, npcInstance.Data.Name + ": " + npcInstance.Data.SayMsg, Color.White);
                        break;
                    }
                }

            // Scan for a hostile NPC in range
            if (npcInstance.Data.AttackNpc && npcInstance.Target == null)
                for (byte i = 0; i < npcInstance.MapInstance.Npc.Length; i++)
                {
                    if (i == npcInstance.Index) continue;
                    if (!npcInstance.MapInstance.Npc[i].Alive) continue;
                    if (npcInstance.Data.IsAllied(npcInstance.MapInstance.Npc[i].Data)) continue;

                    distance = (short)Math.Sqrt(Math.Pow(npcInstance.X - npcInstance.MapInstance.Npc[i].X, 2) +
                                                Math.Pow(npcInstance.Y - npcInstance.MapInstance.Npc[i].Y, 2));
                    if (distance <= npcInstance.Data.Sight)
                    {
                        npcInstance.Target = npcInstance.MapInstance.Npc[i];
                        break;
                    }
                }
        }

        // Validate existing target
        if (npcInstance.Target != null)
        {
            if (npcInstance.Target is Player p && !p.Account.IsPlaying || npcInstance.Target.MapInstance != npcInstance.MapInstance)
                npcInstance.Target = null;
            else if (npcInstance.Target is NpcInstance { Alive: false })
                npcInstance.Target = null;
        }

        // Determine movement destination
        if (npcInstance.Target != null)
        {
            targetX = npcInstance.Target.X;
            targetY = npcInstance.Target.Y;

            if (npcInstance.Data.Sight < Math.Sqrt(Math.Pow(npcInstance.X - targetX, 2) + Math.Pow(npcInstance.Y - targetY, 2)))
                npcInstance.Target = null;
            else
                move = true;
        }
        else if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone > 0 &&
                 npcInstance.MapInstance.Data.Attribute[npcInstance.X, npcInstance.Y].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone)
        {
            // Return to designated zone
            for (byte x = 0; x < CryBits.Entities.Map.Map.Width; x++)
                for (byte y = 0; y < CryBits.Entities.Map.Map.Height; y++)
                    if (npcInstance.MapInstance.Data.Attribute[x, y].Zone == npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone &&
                        !npcInstance.MapInstance.Data.TileBlocked(x, y))
                    {
                        targetX = x;
                        targetY = y;
                        move = true;
                        break;
                    }
        }

        // Move toward or away from target
        if (move)
        {
            if (npcInstance.Vital[(byte)Vital.Hp] > npcInstance.Data.Vital[(byte)Vital.Hp] * (npcInstance.Data.FleeHealth / 100.0))
            {
                canMove[(byte)Direction.Up] = npcInstance.Y > targetY;
                canMove[(byte)Direction.Down] = npcInstance.Y < targetY;
                canMove[(byte)Direction.Left] = npcInstance.X > targetX;
                canMove[(byte)Direction.Right] = npcInstance.X < targetX;
            }
            else
            {
                canMove[(byte)Direction.Up] = npcInstance.Y < targetY;
                canMove[(byte)Direction.Down] = npcInstance.Y > targetY;
                canMove[(byte)Direction.Left] = npcInstance.X < targetX;
                canMove[(byte)Direction.Right] = npcInstance.X > targetX;
            }

            if (MyRandom.Next(0, 2) == 0)
            {
                for (byte d = 0; d < (byte)Direction.Count; d++)
                    if (!moved && canMove[d] && Move(npcInstance, (Direction)d))
                        moved = true;
            }
            else
            {
                for (short d = (byte)Direction.Count - 1; d >= 0; d--)
                    if (!moved && canMove[d] && Move(npcInstance, (Direction)d))
                        moved = true;
            }
        }

        // Random idle movement
        if (npcInstance.Data.Behaviour == (byte)Behaviour.Friendly || npcInstance.Target == null)
            if (MyRandom.Next(0, 3) == 0 && !moved)
            {
                if (npcInstance.Data.Movement == MovementStyle.MoveRandomly)
                    Move(npcInstance, (Direction)MyRandom.Next(0, 4), 1, true);
                else if (npcInstance.Data.Movement == MovementStyle.TurnRandomly)
                {
                    npcInstance.Direction = (Direction)MyRandom.Next(0, 4);
                    NpcSender.MapNpcDirection(npcInstance);
                }
            }

        CombatSystem.Attack(npcInstance);
    }

    /// <summary>Attempts to spawn <paramref name="npcInstance"/> at its configured or a random location.</summary>
    internal static void Spawn(NpcInstance npcInstance)
    {
        if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Spawn)
        {
            SpawnAt(npcInstance, npcInstance.MapInstance.Data.Npc[npcInstance.Index].X, npcInstance.MapInstance.Data.Npc[npcInstance.Index].Y);
            return;
        }

        // Try up to 50 random positions
        for (byte i = 0; i < 50; i++)
        {
            var x = (byte)MyRandom.Next(0, CryBits.Entities.Map.Map.Width - 1);
            var y = (byte)MyRandom.Next(0, CryBits.Entities.Map.Map.Height - 1);

            if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone > 0 &&
                npcInstance.MapInstance.Data.Attribute[x, y].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone)
                continue;

            if (!npcInstance.MapInstance.Data.TileBlocked(x, y))
            {
                SpawnAt(npcInstance, x, y);
                return;
            }
        }

        // Fallback: first walkable tile in zone
        for (byte x = 0; x < CryBits.Entities.Map.Map.Width; x++)
            for (byte y = 0; y < CryBits.Entities.Map.Map.Height; y++)
                if (!npcInstance.MapInstance.Data.TileBlocked(x, y))
                {
                    if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone > 0 &&
                        npcInstance.MapInstance.Data.Attribute[x, y].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone)
                        continue;

                    SpawnAt(npcInstance, x, y);
                    return;
                }
    }

    private static void SpawnAt(NpcInstance npcInstance, byte x, byte y, Direction direction = 0)
    {
        npcInstance.Alive = true;
        npcInstance.X = x;
        npcInstance.Y = y;
        npcInstance.Direction = direction;
        for (byte i = 0; i < (byte)Vital.Count; i++) npcInstance.Vital[i] = npcInstance.Data.Vital[i];
        if (Socket.Device != null) NpcSender.MapNpc(npcInstance.MapInstance.Npc[npcInstance.Index]);
    }

    private static bool Move(NpcInstance npcInstance, Direction direction, byte movement = 1, bool checkZone = false)
    {
        byte nextX = npcInstance.X, nextY = npcInstance.Y;

        npcInstance.Direction = direction;
        NpcSender.MapNpcDirection(npcInstance);
        NextTile(direction, ref nextX, ref nextY);

        if (CryBits.Entities.Map.Map.OutLimit(nextX, nextY)) return false;
        if (npcInstance.MapInstance.TileBlocked(npcInstance.X, npcInstance.Y, direction)) return false;
        if (checkZone && npcInstance.MapInstance.Data.Attribute[nextX, nextY].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone) return false;

        npcInstance.X = nextX;
        npcInstance.Y = nextY;
        NpcSender.MapNpcMovement(npcInstance, movement);
        return true;
    }
}
