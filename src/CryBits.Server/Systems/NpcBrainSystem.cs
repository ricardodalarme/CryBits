using CryBits.Definitions.Maps;
using CryBits.Definitions.Common;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Npcs;
using CryBits.Server.Entities;
using CryBits.Server.Network;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.World;
using System;
using System.Drawing;
using static CryBits.Utils.DirectionUtils;
using static CryBits.Utils.RandomUtils;

namespace CryBits.Server.Systems;

internal sealed class NpcBrainSystem(
    NpcSender npcSender,
    ChatSender chatSender,
    NetworkServer networkServer) : ISimulationSystem
{
    public static NpcBrainSystem Instance { get; } = new(
        NpcSender.Instance,
        ChatSender.Instance,
        NetworkServer.Instance);

    private long _lastTick;

    public void Execute(GameWorld world, Tick tick)
    {
        if (Environment.TickCount64 <= _lastTick + 500) return;
        _lastTick = Environment.TickCount64;

        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers()) continue;

            foreach (var npc in map.Npc)
            {
                if (!npc.Alive) continue;

                TickAlive(npc);
            }
        }
    }

    private void TickAlive(NpcInstance npcInstance)
    {
        byte targetX = 0, targetY = 0;
        var canMove = new bool[(byte)Direction.Count];
        var moved = false;
        var move = false;

        if (npcInstance.Data.Behaviour == Behaviour.AttackOnSight)
        {
            short distance;

            if (npcInstance.Target == null)
                foreach (var session in GameWorld.Current.Sessions)
                {
                    if (!session.IsPlaying) continue;
                    if (session.Character!.MapInstance != npcInstance.MapInstance) continue;

                    distance = (short)Math.Sqrt(Math.Pow(npcInstance.X - session.Character.X, 2) +
                                                Math.Pow(npcInstance.Y - session.Character.Y, 2));
                    if (distance <= npcInstance.Data.Sight)
                    {
                        npcInstance.Target = session.Character;
                        if (!string.IsNullOrEmpty(npcInstance.Data.SayMsg))
                            chatSender.Message(session.Character, npcInstance.Data.Name + ": " + npcInstance.Data.SayMsg, Color.White);
                        break;
                    }
                }

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

        if (npcInstance.Target != null)
        {
            if (npcInstance.Target is Player p && !p.Session.IsPlaying || npcInstance.Target.MapInstance != npcInstance.MapInstance)
                npcInstance.Target = null;
            else if (npcInstance.Target is NpcInstance { Alive: false })
                npcInstance.Target = null;
        }

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
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    if (npcInstance.MapInstance.Data.Attribute[x, y].Zone == npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone &&
                        !npcInstance.MapInstance.Data.TileBlocked(x, y))
                    {
                        targetX = x;
                        targetY = y;
                        move = true;
                        break;
                    }
        }

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

        if (npcInstance.Data.Behaviour == (byte)Behaviour.Friendly || npcInstance.Target == null)
            if (MyRandom.Next(0, 3) == 0 && !moved)
            {
                if (npcInstance.Data.Movement == MovementStyle.MoveRandomly)
                    Move(npcInstance, (Direction)MyRandom.Next(0, 4), 1, true);
                else if (npcInstance.Data.Movement == MovementStyle.TurnRandomly)
                {
                    npcInstance.Direction = (Direction)MyRandom.Next(0, 4);
                    npcSender.MapNpcDirection(npcInstance);
                }
            }
    }

    internal void Spawn(NpcInstance npcInstance)
    {
        if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Spawn)
        {
            SpawnAt(npcInstance, npcInstance.MapInstance.Data.Npc[npcInstance.Index].X, npcInstance.MapInstance.Data.Npc[npcInstance.Index].Y);
            return;
        }

        for (byte i = 0; i < 50; i++)
        {
            var x = (byte)MyRandom.Next(0, Map.Width - 1);
            var y = (byte)MyRandom.Next(0, Map.Height - 1);

            if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone > 0 &&
                npcInstance.MapInstance.Data.Attribute[x, y].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone)
                continue;

            if (!npcInstance.MapInstance.Data.TileBlocked(x, y))
            {
                SpawnAt(npcInstance, x, y);
                return;
            }
        }

        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                if (!npcInstance.MapInstance.Data.TileBlocked(x, y))
                {
                    if (npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone > 0 &&
                        npcInstance.MapInstance.Data.Attribute[x, y].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone)
                        continue;

                    SpawnAt(npcInstance, x, y);
                    return;
                }
    }

    private void SpawnAt(NpcInstance npcInstance, byte x, byte y, Direction direction = 0)
    {
        npcInstance.Alive = true;
        npcInstance.X = x;
        npcInstance.Y = y;
        npcInstance.Direction = direction;
        for (byte i = 0; i < (byte)Vital.Count; i++) npcInstance.Vital[i] = npcInstance.Data.Vital[i];
        if (networkServer.Device != null) npcSender.MapNpc(npcInstance.MapInstance.Npc[npcInstance.Index]);
    }

    private bool Move(NpcInstance npcInstance, Direction direction, byte movement = 1, bool checkZone = false)
    {
        byte nextX = npcInstance.X, nextY = npcInstance.Y;

        npcInstance.Direction = direction;
        npcSender.MapNpcDirection(npcInstance);
        NextTile(direction, ref nextX, ref nextY);

        if (Map.OutLimit(nextX, nextY)) return false;
        if (npcInstance.MapInstance.TileBlocked(npcInstance.X, npcInstance.Y, direction)) return false;
        if (checkZone && npcInstance.MapInstance.Data.Attribute[nextX, nextY].Zone != npcInstance.MapInstance.Data.Npc[npcInstance.Index].Zone) return false;

        npcInstance.X = nextX;
        npcInstance.Y = nextY;
        npcSender.MapNpcMovement(npcInstance, movement);
        return true;
    }
}
