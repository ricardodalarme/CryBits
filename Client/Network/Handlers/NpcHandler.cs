using System;
using System.Collections.Generic;
using CryBits.Client.ECS;
using CryBits.Client.ECS.Components;
using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Packets.Server;
using static CryBits.Globals;
using static CryBits.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class NpcHandler
{
    private static GameContext Ctx => GameContext.Instance;

    [PacketHandler]
    internal static void Npcs(NpcsPacket packet)
    {
        Npc.List = packet.List;
    }

    [PacketHandler]
    internal static void MapNpcs(MapNpcsPacket packet)
    {
        Ctx.ResetNpcSlots(packet.Npcs.Length);

        for (byte i = 0; i < packet.Npcs.Length; i++)
        {
            var id = Ctx.GetOrCreateNpcEntity(i);
            ApplyNpcState(id, i, packet.Npcs[i].NpcId, packet.Npcs[i].X, packet.Npcs[i].Y,
                (Direction)packet.Npcs[i].Direction, packet.Npcs[i].Vital);
        }
    }

    [PacketHandler]
    internal static void MapNpc(MapNpcPacket packet)
    {
        var i = packet.Index;
        var id = Ctx.GetOrCreateNpcEntity(i);
        ApplyNpcState(id, i, packet.NpcId, packet.X, packet.Y,
            (Direction)packet.Direction, packet.Vital);
    }

    [PacketHandler]
    internal static void MapNpcMovement(MapNpcMovementPacket packet)
    {
        var i = packet.Index;
        if (i >= Ctx.NpcSlots.Length) return;
        var id = Ctx.NpcSlots[i];
        if (id < 0) return;

        var transform = Ctx.World.Get<TransformComponent>(id);
        var movement = Ctx.World.Get<MovementComponent>(id);

        var prevX = transform.TileX;
        var prevY = transform.TileY;

        transform.TileX = packet.X;
        transform.TileY = packet.Y;
        transform.Direction = (Direction)packet.Direction;
        transform.PixelOffsetX = 0;
        transform.PixelOffsetY = 0;

        movement.Current = (Movement)packet.Movement;

        if (prevX != transform.TileX || prevY != transform.TileY)
            switch (transform.Direction)
            {
                case Direction.Up: transform.PixelOffsetY = Grid; break;
                case Direction.Down: transform.PixelOffsetY = Grid * -1; break;
                case Direction.Right: transform.PixelOffsetX = Grid * -1; break;
                case Direction.Left: transform.PixelOffsetX = Grid; break;
            }
    }

    [PacketHandler]
    internal static void MapNpcAttack(MapNpcAttackPacket packet)
    {
        var i = packet.Index;
        if (i >= Ctx.NpcSlots.Length) return;
        var id = Ctx.NpcSlots[i];
        if (id < 0) return;

        var animation = Ctx.World.Get<AnimationComponent>(id);
        animation.IsAttacking = true;
        animation.AttackTimer = Environment.TickCount;

        if (packet.Victim == string.Empty) return;

        if (packet.VictimType == (byte)Target.Player)
        {
            var victimId = Ctx.FindPlayer(packet.Victim);
            if (victimId < 0) return;

            if (Ctx.World.TryGet<CharacterSpriteComponent>(victimId, out var vs)) vs.HurtTimer = Environment.TickCount;
            if (Ctx.World.TryGet<TransformComponent>(victimId, out var vt))
                SpawnBlood(vt.TileX, vt.TileY);
        }
        else if (packet.VictimType == (byte)Target.Npc)
        {
            var npcVictimIndex = byte.Parse(packet.Victim);
            var npcVictimId = npcVictimIndex < Ctx.NpcSlots.Length ? Ctx.NpcSlots[npcVictimIndex] : -1;
            if (npcVictimId < 0) return;

            if (Ctx.World.TryGet<CharacterSpriteComponent>(npcVictimId, out var ns)) ns.HurtTimer = Environment.TickCount;
            if (Ctx.World.TryGet<TransformComponent>(npcVictimId, out var nt))
                SpawnBlood(nt.TileX, nt.TileY);
        }
    }

    [PacketHandler]
    internal static void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        var i = packet.Index;
        if (i >= Ctx.NpcSlots.Length) return;
        var id = Ctx.NpcSlots[i];
        if (id < 0) return;

        var transform = Ctx.World.Get<TransformComponent>(id);
        transform.Direction = (Direction)packet.Direction;
        transform.PixelOffsetX = 0;
        transform.PixelOffsetY = 0;
    }

    [PacketHandler]
    internal static void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var i = packet.Index;
        if (i >= Ctx.NpcSlots.Length) return;
        var id = Ctx.NpcSlots[i];
        if (id < 0) return;

        var vitals = Ctx.World.Get<VitalsComponent>(id);
        for (byte n = 0; n < (byte)Vital.Count; n++)
            vitals.Current[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal static void MapNpcDied(MapNpcDiedPacket packet)
    {
        Ctx.ClearNpcSlot(packet.Index);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static void ApplyNpcState(int entityId, byte slotIndex, Guid npcId,
        byte x, byte y, Direction direction, short[] vital)
    {
        var npc = Ctx.World.Get<NpcDataComponent>(entityId);
        var transform = Ctx.World.Get<TransformComponent>(entityId);
        var vitals = Ctx.World.Get<VitalsComponent>(entityId);

        npc.Data = Npc.List.GetValueOrDefault(npcId);
        transform.TileX = x;
        transform.TileY = y;
        transform.Direction = direction;
        transform.PixelOffsetX = 0;
        transform.PixelOffsetY = 0;

        Ctx.World.Get<MovementComponent>(entityId).Current = Movement.Stopped;

        for (byte n = 0; n < (byte)Vital.Count; n++)
            vitals.Current[n] = vital[n];
    }

    private static void SpawnBlood(byte tileX, byte tileY)
    {
        var bloodId = Ctx.World.Create();
        Ctx.World.Add(bloodId, new BloodSplatComponent
        {
            TextureNum = (byte)MyRandom.Next(0, 3),
            TileX = tileX,
            TileY = tileY,
            Opacity = 255,
            NextFadeAt = Environment.TickCount + 100
        });
    }
}
