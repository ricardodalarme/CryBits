using System;
using Arch.Core;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Equipment;
using CryBits.Client.Components.Movement;
using CryBits.Client.Spawners;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;
using ArchEntity = Arch.Core.Entity;

namespace CryBits.Client.Network.Handlers;

internal class PlayerHandler(GameContext context)
{
    [PacketHandler]
    internal void PlayerData(PlayerDataPacket packet)
    {
        var name = packet.Name;
        var isLocal = name == context.LocalPlayerName;

        context.CurrentMap = context.Maps[packet.MapId];

        var equipmentItems = new Item?[(byte)Equipment.Count];
        for (byte n = 0; n < (byte)Equipment.Count; n++) equipmentItems[n] = Item.List.Get(packet.Equipment[n]);

        // Destroy old entity if present (re-spawn on map transition).
        var old = context.GetPlayerEntity(name);
        if (old != ArchEntity.Null) context.World.Destroy(old);

        var entity = PlayerSpawner.Spawn(
            context.World,
            name,
            packet.TextureNum,
            packet.Level,
            packet.Vital,
            packet.MaxVital,
            packet.Attribute,
            equipmentItems,
            packet.X, packet.Y,
            (Direction)packet.Direction,
            isLocal,
            packet.MapId);

        if (isLocal)
        {
            context.LocalPlayer = new LocalPlayer(entity);
            BarsView.Update();
            CharacterView.Update();
        }
    }

    [PacketHandler]
    internal void PlayerPosition(PlayerPositionPacket packet)
    {
        var entity = context.GetPlayerEntity(packet.Name);

        ref var movement = ref context.World.Get<MovementComponent>(entity);
        movement.TileX = packet.X;
        movement.TileY = packet.Y;
        movement.Direction = (Direction)packet.Direction;
        movement.OffsetX = 0f;
        movement.OffsetY = 0f;
        movement.MovementState = Movement.Stopped;
    }

    [PacketHandler]
    internal void PlayerVitals(PlayerVitalsPacket packet)
    {
        var entity = context.GetPlayerEntity(packet.Name);

        ref var vitals = ref context.World.Get<VitalsComponent>(entity);
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            vitals.Current[i] = packet.Vital[i];
            vitals.Max[i] = packet.MaxVital[i];
        }

        if (packet.Name == context.LocalPlayerName) BarsView.Update();
    }

    [PacketHandler]
    internal void PlayerEquipments(PlayerEquipmentsPacket packet)
    {
        var entity = context.GetPlayerEntity(packet.Name);

        // Update player's equipped items
        ref var equipment = ref context.World.Get<EquipmentComponent>(entity);
        for (byte i = 0; i < (byte)Equipment.Count; i++) equipment.Slots[i] = Item.List.Get(packet.Equipments[i]);
    }

    [PacketHandler]
    internal void PlayerLeave(PlayerLeavePacket packet)
    {
        var entity = context.GetPlayerEntity(packet.Name);
        if (entity != ArchEntity.Null) context.World.Destroy(entity);
    }

    [PacketHandler]
    internal void PlayerMove(PlayerMovePacket packet)
    {
        var entity = context.GetPlayerEntity(packet.Name);

        ref var movement = ref context.World.Get<MovementComponent>(entity);
        movement.TileX = packet.X;
        movement.TileY = packet.Y;
        movement.Direction = (Direction)packet.Direction;
        movement.MovementState = (Movement)packet.Movement;
        movement.SpeedPixelsPerSecond = packet.Speed;
        movement.OffsetX = 0f;
        movement.OffsetY = 0f;

        switch (movement.Direction)
        {
            case Direction.Up: movement.OffsetY = Grid; break;
            case Direction.Down: movement.OffsetY = -Grid; break;
            case Direction.Right: movement.OffsetX = -Grid; break;
            case Direction.Left: movement.OffsetX = Grid; break;
        }
    }

    [PacketHandler]
    internal void PlayerDirection(PlayerDirectionPacket packet)
    {
        var entity = context.GetPlayerEntity(packet.Name);
        context.World.Get<MovementComponent>(entity).Direction = (Direction)packet.Direction;
    }

    [PacketHandler]
    internal void PlayerAttack(PlayerAttackPacket packet)
    {
        var entity = context.GetPlayerEntity(packet.Name);
        var victim = packet.Victim;
        var victimType = (Target)packet.VictimType;

        ref var state = ref context.World.Get<CharacterStateComponent>(entity);
        state.IsAttacking = true;
        state.AttackTimer = Environment.TickCount;

        if (victim == string.Empty || victimType == Target.None) return;

        var victimEntity = victimType switch
        {
            Target.Player => context.GetPlayerEntity(victim),
            Target.Npc => context.CurrentMap.Npcs[byte.Parse(victim)],
            _ => throw new ArgumentOutOfRangeException()
        };

        var world = context.World;
        ref var victimMovement = ref world.Get<MovementComponent>(victimEntity);
        BloodSplatSpawner.Spawn(world, victimMovement.TileX, victimMovement.TileY);
        ref var tint = ref context.World.Get<DamageTintComponent>(victimEntity);
        tint.IsHurt = true;
        tint.HurtTimestamp = Environment.TickCount;
    }

    [PacketHandler]
    internal void PlayerExperience(PlayerExperiencePacket packet)
    {
        ref var level = ref context.LocalPlayer.GetLevel();
        level.Experience = packet.Experience;
        level.ExpNeeded = packet.ExpNeeded;
        level.Points = packet.Points;

        CharacterView.AddStrengthButton.Visible = level.Points > 0;
        CharacterView.AddResistanceButton.Visible = level.Points > 0;
        CharacterView.AddIntelligenceButton.Visible = level.Points > 0;
        CharacterView.AddAgilityButton.Visible = level.Points > 0;
        CharacterView.AddVitalityButton.Visible = level.Points > 0;

        BarsView.Update();
        CharacterView.Update();
    }

    [PacketHandler]
    internal void PlayerInventory(PlayerInventoryPacket packet)
    {
        ref var inventory = ref context.LocalPlayer.GetInventory();
        for (byte i = 0; i < MaxInventory; i++)
            inventory.Slots[i] = new ItemSlot(Item.List.Get(packet.ItemIds[i]), packet.Amounts[i]);
    }

    [PacketHandler]
    internal void PlayerHotbar(PlayerHotbarPacket packet)
    {
        ref var hotbar = ref context.LocalPlayer.GetHotbar();
        for (byte i = 0; i < MaxHotbar; i++)
            hotbar.Slots[i] = new HotbarSlot((SlotType)packet.Types[i], packet.Slots[i]);
    }
}
