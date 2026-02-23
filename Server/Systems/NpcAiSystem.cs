using System;
using System.Drawing;
using CryBits.Enums;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Network;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using static CryBits.Utils;

namespace CryBits.Server.Systems;

/// <summary>Tick-driven system that runs NPC spawn, targeting, movement and delegates combat.</summary>
internal static class NpcAiSystem
{
    /// <summary>Runs one AI tick for the NPC entity identified by <paramref name="entityId"/>.</summary>
    internal static void Tick(int entityId)
    {
        var world = ServerContext.Instance.World;
        var npcData = world.Get<NpcDataComponent>(entityId);
        var npcState = world.Get<NpcStateComponent>(entityId);
        var pos = world.Get<PositionComponent>(entityId);
        var dir = world.Get<DirectionComponent>(entityId);
        var target = world.Get<NpcTargetComponent>(entityId);

        var map = GameWorld.Current.Maps[npcData.MapId];

        if (!npcState.Alive)
        {
            if (Environment.TickCount64 > npcState.SpawnTimer + npcData.Data.SpawnTime * 1000)
                Spawn(entityId);
            return;
        }

        RegenerationSystem.Tick(entityId);

        byte targetX = 0, targetY = 0;
        var canMove = new bool[(byte)Direction.Count];
        var moved = false;
        var move = false;

        // Target acquisition — attack on sight
        if (npcData.Data.Behaviour == Behaviour.AttackOnSight)
        {
            short distance;

            // Scan for a player in range
            if (target.TargetEntityId == null)
                foreach (var session in GameWorld.Current.Sessions)
                {
                    if (!session.IsPlaying) continue;
                    var playerPos = world.Get<PositionComponent>(session.Character!.EntityId);
                    if (playerPos.MapId != npcData.MapId) continue;

                    distance = (short)Math.Sqrt(Math.Pow(pos.X - playerPos.X, 2) +
                                                Math.Pow(pos.Y - playerPos.Y, 2));
                    if (distance <= npcData.Data.Sight)
                    {
                        target.TargetEntityId = session.Character.EntityId;
                        if (!string.IsNullOrEmpty(npcData.Data.SayMsg))
                            ChatSender.Message(session.Character, npcData.Data.Name + ": " + npcData.Data.SayMsg,
                                Color.White);
                        break;
                    }
                }

            // Scan for a hostile NPC in range
            if (npcData.Data.AttackNpc && target.TargetEntityId == null)
            {
                foreach (var (otherId, otherNpcData) in world.Query<NpcDataComponent>())
                {
                    if (otherId == entityId) continue;
                    if (otherNpcData.MapId != npcData.MapId) continue;
                    if (!world.Get<NpcStateComponent>(otherId).Alive) continue;
                    if (npcData.Data.IsAllied(otherNpcData.Data)) continue;

                    var otherPos = world.Get<PositionComponent>(otherId);
                    distance = (short)Math.Sqrt(Math.Pow(pos.X - otherPos.X, 2) +
                                                Math.Pow(pos.Y - otherPos.Y, 2));
                    if (distance <= npcData.Data.Sight)
                    {
                        target.TargetEntityId = otherId;
                        break;
                    }
                }
            }
        }

        // Validate existing target
        if (target.TargetEntityId != null)
        {
            var tid = target.TargetEntityId.Value;
            if (world.Has<SessionComponent>(tid))
            {
                // Target is a player
                var session = world.Get<SessionComponent>(tid).Session;
                if (!session.IsPlaying)
                    target.TargetEntityId = null;
                else
                {
                    var tPos = world.Get<PositionComponent>(tid);
                    if (tPos.MapId != npcData.MapId) target.TargetEntityId = null;
                }
            }
            else if (world.Has<NpcStateComponent>(tid))
            {
                // Target is another NPC
                if (!world.Get<NpcStateComponent>(tid).Alive)
                    target.TargetEntityId = null;
            }
            else
            {
                target.TargetEntityId = null;
            }
        }

        // Determine movement destination
        if (target.TargetEntityId != null)
        {
            var tid = target.TargetEntityId.Value;
            var tPos = world.Get<PositionComponent>(tid);
            targetX = tPos.X;
            targetY = tPos.Y;

            if (npcData.Data.Sight < Math.Sqrt(Math.Pow(pos.X - targetX, 2) + Math.Pow(pos.Y - targetY, 2)))
                target.TargetEntityId = null;
            else
                move = true;
        }
        else if (map.Data.Npc[npcData.Index].Zone > 0 &&
                 map.Data.Attribute[pos.X, pos.Y].Zone != map.Data.Npc[npcData.Index].Zone)
        {
            // Return to designated zone
            for (byte x = 0; x < CryBits.Entities.Map.Map.Width; x++)
            for (byte y = 0; y < CryBits.Entities.Map.Map.Height; y++)
                if (map.Data.Attribute[x, y].Zone == map.Data.Npc[npcData.Index].Zone &&
                    !map.Data.TileBlocked(x, y))
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
            var vitals = world.Get<VitalsComponent>(entityId);
            if (vitals.Values[(byte)Vital.Hp] > npcData.Data.Vital[(byte)Vital.Hp] * (npcData.Data.FleeHealth / 100.0))
            {
                canMove[(byte)Direction.Up] = pos.Y > targetY;
                canMove[(byte)Direction.Down] = pos.Y < targetY;
                canMove[(byte)Direction.Left] = pos.X > targetX;
                canMove[(byte)Direction.Right] = pos.X < targetX;
            }
            else
            {
                canMove[(byte)Direction.Up] = pos.Y < targetY;
                canMove[(byte)Direction.Down] = pos.Y > targetY;
                canMove[(byte)Direction.Left] = pos.X < targetX;
                canMove[(byte)Direction.Right] = pos.X > targetX;
            }

            if (MyRandom.Next(0, 2) == 0)
            {
                for (byte d = 0; d < (byte)Direction.Count; d++)
                    if (!moved && canMove[d] && Move(entityId, (Direction)d))
                        moved = true;
            }
            else
            {
                for (short d = (byte)Direction.Count - 1; d >= 0; d--)
                    if (!moved && canMove[d] && Move(entityId, (Direction)d))
                        moved = true;
            }
        }

        // Random idle movement
        if (npcData.Data.Behaviour == (byte)Behaviour.Friendly || target.TargetEntityId == null)
            if (MyRandom.Next(0, 3) == 0 && !moved)
            {
                if (npcData.Data.Movement == MovementStyle.MoveRandomly)
                    Move(entityId, (Direction)MyRandom.Next(0, 4), 1, true);
                else if (npcData.Data.Movement == MovementStyle.TurnRandomly)
                {
                    dir.Value = (Direction)MyRandom.Next(0, 4);
                    NpcSender.MapNpcDirection(entityId);
                }
            }

        CombatSystem.Attack(entityId);
    }

    /// <summary>Attempts to spawn the NPC entity at its configured or a random location.</summary>
    internal static void Spawn(int entityId)
    {
        var world = ServerContext.Instance.World;
        var npcData = world.Get<NpcDataComponent>(entityId);
        var map = GameWorld.Current.Maps[npcData.MapId];
        var slot = map.Data.Npc[npcData.Index];

        if (slot.Spawn)
        {
            SpawnAt(entityId, slot.X, slot.Y);
            return;
        }

        // Try up to 50 random positions
        for (byte i = 0; i < 50; i++)
        {
            var x = (byte)MyRandom.Next(0, CryBits.Entities.Map.Map.Width - 1);
            var y = (byte)MyRandom.Next(0, CryBits.Entities.Map.Map.Height - 1);

            if (slot.Zone > 0 && map.Data.Attribute[x, y].Zone != slot.Zone) continue;
            if (!map.Data.TileBlocked(x, y))
            {
                SpawnAt(entityId, x, y);
                return;
            }
        }

        // Fallback: first walkable tile in zone
        for (byte x = 0; x < CryBits.Entities.Map.Map.Width; x++)
        for (byte y = 0; y < CryBits.Entities.Map.Map.Height; y++)
        {
            if (slot.Zone > 0 && map.Data.Attribute[x, y].Zone != slot.Zone) continue;
            if (!map.Data.TileBlocked(x, y))
            {
                SpawnAt(entityId, x, y);
                return;
            }
        }
    }

    private static void SpawnAt(int entityId, byte x, byte y, Direction direction = 0)
    {
        var world = ServerContext.Instance.World;
        var npcData = world.Get<NpcDataComponent>(entityId);
        var npcState = world.Get<NpcStateComponent>(entityId);
        var pos = world.Get<PositionComponent>(entityId);
        var dir = world.Get<DirectionComponent>(entityId);
        var vitals = world.Get<VitalsComponent>(entityId);

        npcState.Alive = true;
        pos.X = x;
        pos.Y = y;
        dir.Value = direction;

        for (byte i = 0; i < (byte)Vital.Count; i++)
            vitals.Values[i] = npcData.Data.Vital[i];

        if (Socket.Device != null)
            NpcSender.MapNpc(entityId);
    }

    private static bool Move(int entityId, Direction direction, byte movement = 1, bool checkZone = false)
    {
        var world = ServerContext.Instance.World;
        var npcData = world.Get<NpcDataComponent>(entityId);
        var pos = world.Get<PositionComponent>(entityId);
        var dir = world.Get<DirectionComponent>(entityId);
        var map = GameWorld.Current.Maps[npcData.MapId];

        dir.Value = direction;
        NpcSender.MapNpcDirection(entityId);

        byte nextX = pos.X, nextY = pos.Y;
        NextTile(direction, ref nextX, ref nextY);

        if (CryBits.Entities.Map.Map.OutLimit(nextX, nextY)) return false;
        if (map.TileBlocked(pos.X, pos.Y, direction)) return false;
        if (checkZone && map.Data.Attribute[nextX, nextY].Zone != map.Data.Npc[npcData.Index].Zone) return false;

        pos.X = nextX;
        pos.Y = nextY;
        NpcSender.MapNpcMovement(entityId, movement);
        return true;
    }
}
