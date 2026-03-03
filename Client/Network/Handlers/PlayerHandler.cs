using System;
using Arch.Core;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Movement;
using CryBits.Client.Entities;
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
        Player player;

        if (name != Player.Me.Name)
        {
            player = new Player(name);
            Player.List.Add(player);
        }
        else
            player = Player.Me;

        player.TextureNum = packet.TextureNum;
        player.Level = packet.Level;
        player.MapInstance = MapInstance.List[packet.MapId];
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            player.Vital[n] = packet.Vital[n];
            player.MaxVital[n] = packet.MaxVital[n];
        }

        for (byte n = 0; n < (byte)Attribute.Count; n++) player.Attribute[n] = packet.Attribute[n];
        for (byte n = 0; n < (byte)Equipment.Count; n++) player.Equipment[n] = Item.List.Get(packet.Equipment[n]);
        context.CurrentMap = player.MapInstance;

        // Eager spawn: (re)create the ECS entity right here so every subsequent
        // handler and the input loop can always reference a valid entity.
        if (player.Entity != ArchEntity.Null) context.World.Destroy(player.Entity);
        player.Entity = PlayerSpawner.Spawn(context.World, player, packet.X, packet.Y, (Direction)packet.Direction);

        if (player == Player.Me)
        {
            context.LocalPlayer = new LocalPlayer(player.Entity);
            BarsView.Update();
            CharacterView.Update();
        }
    }

    [PacketHandler]
    internal void PlayerPosition(PlayerPositionPacket packet)
    {
        var player = Player.Get(packet.Name);

        ref var movement = ref context.World.Get<MovementComponent>(player.Entity);
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
        var player = Player.Get(packet.Name);

        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            player.Vital[i] = packet.Vital[i];
            player.MaxVital[i] = packet.MaxVital[i];
        }

        ref var vitals = ref context.World.Get<VitalsComponent>(player.Entity);
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            vitals.Current[i] = packet.Vital[i];
            vitals.Max[i] = packet.MaxVital[i];
        }

        if (player == Player.Me) BarsView.Update();
    }

    [PacketHandler]
    internal void PlayerEquipments(PlayerEquipmentsPacket packet)
    {
        var player = Player.Get(packet.Name);

        // Update player's equipped items
        for (byte i = 0; i < (byte)Equipment.Count; i++) player.Equipment[i] = Item.List.Get(packet.Equipments[i]);
    }

    [PacketHandler]
    internal void PlayerLeave(PlayerLeavePacket packet)
    {
        var player = Player.Get(packet.Name);
        if (player.Entity != ArchEntity.Null) context.World.Destroy(player.Entity);
        Player.List.Remove(player);
    }

    [PacketHandler]
    internal void PlayerMove(PlayerMovePacket packet)
    {
        var player = Player.Get(packet.Name);

        ref var movement = ref context.World.Get<MovementComponent>(player.Entity);
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
        var player = Player.Get(packet.Name);
        context.World.Get<MovementComponent>(player.Entity).Direction = (Direction)packet.Direction;
    }

    [PacketHandler]
    internal void PlayerAttack(PlayerAttackPacket packet)
    {
        var player = Player.Get(packet.Name);
        var victim = packet.Victim;
        var victimType = (Target)packet.VictimType;

        ref var state = ref context.World.Get<CharacterStateComponent>(player.Entity);
        state.IsAttacking = true;
        state.AttackTimer = Environment.TickCount;

        if (victim == string.Empty || victimType == Target.None) return;

        var victimEntity = victimType switch
        {
            Target.Player => Player.Get(victim).Entity,
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
