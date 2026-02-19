using System;
using System.Drawing;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Network;
using CryBits.Server.Network.Senders;
using static CryBits.Utils;

namespace CryBits.Server.Systems;

/// <summary>Tick-driven system that runs NPC spawn, targeting, movement and delegates combat.</summary>
internal static class NpcAiSystem
{
    /// <summary>Runs one AI tick for <paramref name="npc"/>: spawn check, regen, targeting, movement, attack.</summary>
    internal static void Tick(TempNpc npc)
    {
        if (!npc.Alive)
        {
            if (Environment.TickCount64 > npc.SpawnTimer + npc.Data.SpawnTime * 1000) Spawn(npc);
            return;
        }

        RegenerationSystem.Tick(npc);

        byte targetX = 0, targetY = 0;
        var canMove = new bool[(byte)Direction.Count];
        var moved = false;
        var move = false;

        // Target acquisition — attack on sight
        if (npc.Data.Behaviour == Behaviour.AttackOnSight)
        {
            short distance;

            // Scan for a player in range
            if (npc.Target == null)
                foreach (var account in Account.List)
                {
                    if (!account.IsPlaying) continue;
                    if (account.Character.Map != npc.Map) continue;

                    distance = (short)Math.Sqrt(Math.Pow(npc.X - account.Character.X, 2) +
                                                Math.Pow(npc.Y - account.Character.Y, 2));
                    if (distance <= npc.Data.Sight)
                    {
                        npc.Target = account.Character;
                        if (!string.IsNullOrEmpty(npc.Data.SayMsg))
                            ChatSender.Message(account.Character, npc.Data.Name + ": " + npc.Data.SayMsg, Color.White);
                        break;
                    }
                }

            // Scan for a hostile NPC in range
            if (npc.Data.AttackNpc && npc.Target == null)
                for (byte i = 0; i < npc.Map.Npc.Length; i++)
                {
                    if (i == npc.Index) continue;
                    if (!npc.Map.Npc[i].Alive) continue;
                    if (npc.Data.IsAllied(npc.Map.Npc[i].Data)) continue;

                    distance = (short)Math.Sqrt(Math.Pow(npc.X - npc.Map.Npc[i].X, 2) +
                                                Math.Pow(npc.Y - npc.Map.Npc[i].Y, 2));
                    if (distance <= npc.Data.Sight)
                    {
                        npc.Target = npc.Map.Npc[i];
                        break;
                    }
                }
        }

        // Validate existing target
        if (npc.Target != null)
        {
            if (npc.Target is Player p && !p.Account.IsPlaying || npc.Target.Map != npc.Map)
                npc.Target = null;
            else if (npc.Target is TempNpc { Alive: false })
                npc.Target = null;
        }

        // Determine movement destination
        if (npc.Target != null)
        {
            targetX = npc.Target.X;
            targetY = npc.Target.Y;

            if (npc.Data.Sight < Math.Sqrt(Math.Pow(npc.X - targetX, 2) + Math.Pow(npc.Y - targetY, 2)))
                npc.Target = null;
            else
                move = true;
        }
        else if (npc.Map.Data.Npc[npc.Index].Zone > 0 &&
                 npc.Map.Data.Attribute[npc.X, npc.Y].Zone != npc.Map.Data.Npc[npc.Index].Zone)
        {
            // Return to designated zone
            for (byte x = 0; x < CryBits.Entities.Map.Map.Width; x++)
            for (byte y = 0; y < CryBits.Entities.Map.Map.Height; y++)
                if (npc.Map.Data.Attribute[x, y].Zone == npc.Map.Data.Npc[npc.Index].Zone &&
                    !npc.Map.Data.TileBlocked(x, y))
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
            if (npc.Vital[(byte)Vital.Hp] > npc.Data.Vital[(byte)Vital.Hp] * (npc.Data.FleeHealth / 100.0))
            {
                canMove[(byte)Direction.Up] = npc.Y > targetY;
                canMove[(byte)Direction.Down] = npc.Y < targetY;
                canMove[(byte)Direction.Left] = npc.X > targetX;
                canMove[(byte)Direction.Right] = npc.X < targetX;
            }
            else
            {
                canMove[(byte)Direction.Up] = npc.Y < targetY;
                canMove[(byte)Direction.Down] = npc.Y > targetY;
                canMove[(byte)Direction.Left] = npc.X < targetX;
                canMove[(byte)Direction.Right] = npc.X > targetX;
            }

            if (MyRandom.Next(0, 2) == 0)
            {
                for (byte d = 0; d < (byte)Direction.Count; d++)
                    if (!moved && canMove[d] && Move(npc, (Direction)d))
                        moved = true;
            }
            else
            {
                for (short d = (byte)Direction.Count - 1; d >= 0; d--)
                    if (!moved && canMove[d] && Move(npc, (Direction)d))
                        moved = true;
            }
        }

        // Random idle movement
        if (npc.Data.Behaviour == (byte)Behaviour.Friendly || npc.Target == null)
            if (MyRandom.Next(0, 3) == 0 && !moved)
            {
                if (npc.Data.Movement == MovementStyle.MoveRandomly)
                    Move(npc, (Direction)MyRandom.Next(0, 4), 1, true);
                else if (npc.Data.Movement == MovementStyle.TurnRandomly)
                {
                    npc.Direction = (Direction)MyRandom.Next(0, 4);
                    NpcSender.MapNpcDirection(npc);
                }
            }

        CombatSystem.Attack(npc);
    }

    /// <summary>Attempts to spawn <paramref name="npc"/> at its configured or a random location.</summary>
    internal static void Spawn(TempNpc npc)
    {
        if (npc.Map.Data.Npc[npc.Index].Spawn)
        {
            SpawnAt(npc, npc.Map.Data.Npc[npc.Index].X, npc.Map.Data.Npc[npc.Index].Y);
            return;
        }

        // Try up to 50 random positions
        for (byte i = 0; i < 50; i++)
        {
            var x = (byte)MyRandom.Next(0, CryBits.Entities.Map.Map.Width - 1);
            var y = (byte)MyRandom.Next(0, CryBits.Entities.Map.Map.Height - 1);

            if (npc.Map.Data.Npc[npc.Index].Zone > 0 &&
                npc.Map.Data.Attribute[x, y].Zone != npc.Map.Data.Npc[npc.Index].Zone)
                continue;

            if (!npc.Map.Data.TileBlocked(x, y))
            {
                SpawnAt(npc, x, y);
                return;
            }
        }

        // Fallback: first walkable tile in zone
        for (byte x = 0; x < CryBits.Entities.Map.Map.Width; x++)
        for (byte y = 0; y < CryBits.Entities.Map.Map.Height; y++)
            if (!npc.Map.Data.TileBlocked(x, y))
            {
                if (npc.Map.Data.Npc[npc.Index].Zone > 0 &&
                    npc.Map.Data.Attribute[x, y].Zone != npc.Map.Data.Npc[npc.Index].Zone)
                    continue;

                SpawnAt(npc, x, y);
                return;
            }
    }

    private static void SpawnAt(TempNpc npc, byte x, byte y, Direction direction = 0)
    {
        npc.Alive = true;
        npc.X = x;
        npc.Y = y;
        npc.Direction = direction;
        for (byte i = 0; i < (byte)Vital.Count; i++) npc.Vital[i] = npc.Data.Vital[i];
        if (Socket.Device != null) NpcSender.MapNpc(npc.Map.Npc[npc.Index]);
    }

    private static bool Move(TempNpc npc, Direction direction, byte movement = 1, bool checkZone = false)
    {
        byte nextX = npc.X, nextY = npc.Y;

        npc.Direction = direction;
        NpcSender.MapNpcDirection(npc);
        NextTile(direction, ref nextX, ref nextY);

        if (CryBits.Entities.Map.Map.OutLimit(nextX, nextY)) return false;
        if (npc.Map.TileBlocked(npc.X, npc.Y, direction)) return false;
        if (checkZone && npc.Map.Data.Attribute[nextX, nextY].Zone != npc.Map.Data.Npc[npc.Index].Zone) return false;

        npc.X = nextX;
        npc.Y = nextY;
        NpcSender.MapNpcMovement(npc, movement);
        return true;
    }
}
