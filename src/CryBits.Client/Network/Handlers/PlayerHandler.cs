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
using MovementState = CryBits.Definitions.Common.Movement;
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
        context.World.Set(entity.Value, new MovementComponent(
            packet.X, packet.Y, 0f, 0f, movement.SpeedPixelsPerSecond, MovementState.Stopped, (Direction)packet.Direction
        ));
    }

    [PacketHandler]
    internal void PlayerVitals(PlayerVitalsPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);
        if (entity is null) return;

        var vitals = context.World.Get<Vitals>(entity.Value);
        if (vitals is null) return;
        context.World.Set(entity.Value, new Vitals(
            Hp: packet.Vital[(byte)Vital.Hp],
            Mp: packet.Vital[(byte)Vital.Mp],
            MaxHp: packet.MaxVital[(byte)Vital.Hp],
            MaxMp: packet.MaxVital[(byte)Vital.Mp]
        ));

        if (packet.NetworkId == context.LocalPlayer.Id) BarsView.Update();
    }

    [PacketHandler]
    internal void PlayerEquipments(PlayerEquipmentsPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);
        if (entity is null) return;

        var equipment = context.World.Get<EquipmentState>(entity.Value);
        if (equipment is null) return;
        var newSlots = (Guid[])equipment.Slots.Clone();
        for (byte i = 0; i < (byte)Equipment.Count; i++) newSlots[i] = packet.Equipments[i];
        context.World.Set(entity.Value, new EquipmentState(newSlots));
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

        var dir = (Direction)packet.Direction;
        var offsetX = 0f;
        var offsetY = 0f;
        switch (dir)
        {
            case Direction.Up: offsetY = Grid; break;
            case Direction.Down: offsetY = -Grid; break;
            case Direction.Right: offsetX = -Grid; break;
            case Direction.Left: offsetX = Grid; break;
        }

        context.World.Set(entity.Value, new MovementComponent(
            packet.X, packet.Y, offsetX, offsetY, packet.Speed, (MovementState)packet.Movement, dir
        ));
    }

    [PacketHandler]
    internal void PlayerDirection(PlayerDirectionPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.NetworkId);
        if (entity is null) return;

        var movement = context.World.Get<MovementComponent>(entity.Value);
        if (movement is not null)
            context.World.Set(entity.Value, movement with { Direction = (Direction)packet.Direction });
    }

    [PacketHandler]
    internal void PlayerExperience(PlayerExperiencePacket packet)
    {
        if (context.LocalPlayer.Entity is null) return;
        var level = context.LocalPlayer.GetLevel();
        if (level is null) return;
        context.World.Set(context.LocalPlayer.Entity.Value, new LevelComponent(
            Level: level.Level, Experience: packet.Experience, Points: packet.Points, ExpNeeded: packet.ExpNeeded
        ));

        CharacterView.AddStrengthButton.Visible = packet.Points > 0;
        CharacterView.AddResistanceButton.Visible = packet.Points > 0;
        CharacterView.AddIntelligenceButton.Visible = packet.Points > 0;
        CharacterView.AddAgilityButton.Visible = packet.Points > 0;
        CharacterView.AddVitalityButton.Visible = packet.Points > 0;

        BarsView.Update();
        CharacterView.Update();
    }

    [PacketHandler]
    internal void PlayerInventory(PlayerInventoryPacket packet)
    {
        if (context.LocalPlayer.Entity is null) return;
        var invSlots = new ItemSlot[MaxInventory];
        for (byte i = 0; i < MaxInventory; i++)
            invSlots[i] = new ItemSlot(packet.ItemIds[i], packet.Amounts[i]);
        context.World.Set(context.LocalPlayer.Entity.Value, new InventoryState(invSlots));
    }

    [PacketHandler]
    internal void PlayerHotbar(PlayerHotbarPacket packet)
    {
        if (context.LocalPlayer.Entity is null) return;
        var hotbarSlots = new HotbarSlot[MaxHotbar];
        for (byte i = 0; i < MaxHotbar; i++)
            hotbarSlots[i] = new HotbarSlot((SlotType)packet.Types[i], packet.Slots[i]);
        context.World.Set(context.LocalPlayer.Entity.Value, new HotbarState(hotbarSlots));
    }
}
