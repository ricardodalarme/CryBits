using CryBits.Client.Components;
using CryBits.Client.Spawners;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.State;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Network.Handlers;

internal class PlayerHandler(GameContext context, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    [PacketHandler]
    internal void PlayerData(PlayerDataPacket packet)
    {
        var name = packet.Name;
        var isLocal = packet.NetworkId == context.LocalPlayer.Id;

        // Destroy old entity if present (re-spawn on map transition).
        var old = context.GetNetworkEntity(packet.NetworkId);
        if (old is not null)
        {
            return;
        }

        EntityId entity;
        if (isLocal)
        {
            var equipmentItems = new Item?[(byte)Equipment.Count];
            for (byte n = 0; n < (byte)Equipment.Count; n++) equipmentItems[n] = _catalog.Items.Get(packet.Equipment[n]);

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

        if (isLocal && context.LocalPlayer.Entity is null)
        {
            context.LocalPlayer.Entity = entity;
            BarsView.Update();
            CharacterView.Update();
        }
    }

    [PacketHandler]
    internal void PlayerPosition(PlayerPositionPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);
        if (entity is null) return;

        var movement = context.World.Get<MovementComponent>(entity.Value);
        if (movement is null) return;
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
        if (entity is null) return;

        var vitals = context.World.Get<Vitals>(entity.Value);
        if (vitals is null) return;
        vitals.Hp = packet.Vital[(byte)Vital.Hp];
        vitals.MaxHp = packet.MaxVital[(byte)Vital.Hp];
        vitals.Mp = packet.Vital[(byte)Vital.Mp];
        vitals.MaxMp = packet.MaxVital[(byte)Vital.Mp];

        if (packet.NetworkId == context.LocalPlayer.Id) BarsView.Update();
    }

    [PacketHandler]
    internal void PlayerEquipments(PlayerEquipmentsPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);
        if (entity is null) return;

        // Update player's equipped items
        var equipment = context.World.Get<EquipmentState>(entity.Value);
        if (equipment is null) return;
        for (byte i = 0; i < (byte)Equipment.Count; i++) equipment.Slots[i] = packet.Equipments[i];
    }

    [PacketHandler]
    internal void PlayerLeave(PlayerLeavePacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);
        if (entity is not null)
        {
            context.UnregisterNetworkEntity(packet.NetworkId);
            context.World.Destroy(entity.Value);
        }
    }

    [PacketHandler]
    internal void PlayerMove(PlayerMovePacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);
        if (entity is null) return;

        var movement = context.World.Get<MovementComponent>(entity.Value);
        if (movement is null) return;
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
        if (entity is null) return;

        var movement = context.World.Get<MovementComponent>(entity.Value);
        if (movement is not null) movement.Direction = (Direction)packet.Direction;
    }

    [PacketHandler]
    internal void PlayerExperience(PlayerExperiencePacket packet)
    {
        if (context.LocalPlayer.Entity is null) return;
        var level = context.LocalPlayer.GetLevel();
        if (level is null) return;
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
        if (context.LocalPlayer.Entity is null) return;
        var inventory = context.LocalPlayer.GetInventory();
        if (inventory is null) return;
        for (byte i = 0; i < MaxInventory; i++)
            inventory.Slots[i] = new ItemSlot(packet.ItemIds[i], packet.Amounts[i]);
    }

    [PacketHandler]
    internal void PlayerHotbar(PlayerHotbarPacket packet)
    {
        if (context.LocalPlayer.Entity is null) return;
        var hotbar = context.LocalPlayer.GetHotbar();
        if (hotbar is null) return;
        for (byte i = 0; i < MaxHotbar; i++)
            hotbar.Slots[i] = new HotbarSlot((SlotType)packet.Types[i], packet.Slots[i]);
    }
}
