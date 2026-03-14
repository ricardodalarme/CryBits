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
using Entity = Arch.Core.Entity;

namespace CryBits.Client.Network.Handlers;

internal class PlayerHandler(GameContext context)
{
    [PacketHandler]
    internal void PlayerData(PlayerDataPacket packet)
    {
        var name = packet.Name;
        var isLocal = packet.NetworkId == context.LocalPlayer.Id;

        // Destroy old entity if present (re-spawn on map transition).
        var old = context.GetNetworkEntity(packet.NetworkId);
        if (old != Entity.Null)
        {
            context.UnregisterNetworkEntity(packet.NetworkId);
            context.World.Destroy(old);
        }

        Entity entity;
        if (isLocal)
        {
            var equipmentItems = new Item?[(byte)Equipment.Count];
            for (byte n = 0; n < (byte)Equipment.Count; n++) equipmentItems[n] = Item.List.Get(packet.Equipment[n]);

            entity = PlayerSpawner.SpawnLocal(
                context.World,
                packet.NetworkId,
                name,
                packet.TextureNum,
                packet.Level,
                packet.Vital,
                packet.MaxVital,
                packet.Attribute,
                equipmentItems,
                packet.X, packet.Y,
                (Direction)packet.Direction);
        }
        else
        {
            entity = PlayerSpawner.Spawn(
                context.World,
                packet.NetworkId,
                name,
                packet.TextureNum,
                packet.Vital,
                packet.MaxVital,
                packet.X, packet.Y,
                (Direction)packet.Direction);
        }

        context.RegisterNetworkEntity(packet.NetworkId, entity);

        if (isLocal)
        {
            context.LocalPlayer = new LocalPlayer(context.World, entity);
            BarsView.Update();
            CharacterView.Update();
        }
    }

    [PacketHandler]
    internal void PlayerPosition(PlayerPositionPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);

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
        var entity = context.GetNetworkEntity(packet.NetworkId);

        ref var vitals = ref context.World.Get<VitalsComponent>(entity);
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            vitals.Current[i] = packet.Vital[i];
            vitals.Max[i] = packet.MaxVital[i];
        }

        if (packet.NetworkId == context.LocalPlayer.Id) BarsView.Update();
    }

    [PacketHandler]
    internal void PlayerEquipments(PlayerEquipmentsPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);

        // Update player's equipped items
        ref var equipment = ref context.World.Get<EquipmentComponent>(entity);
        for (byte i = 0; i < (byte)Equipment.Count; i++) equipment.Slots[i] = Item.List.Get(packet.Equipments[i]);
    }

    [PacketHandler]
    internal void PlayerLeave(PlayerLeavePacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);
        if (entity != Entity.Null)
        {
            context.UnregisterNetworkEntity(packet.NetworkId);
            context.World.Destroy(entity);
        }
    }

    [PacketHandler]
    internal void PlayerMove(PlayerMovePacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);

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
        var entity = context.GetNetworkEntity(packet.NetworkId);
        context.World.Get<MovementComponent>(entity).Direction = (Direction)packet.Direction;
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
        if (context.LocalPlayer.Entity == Entity.Null) return;
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
