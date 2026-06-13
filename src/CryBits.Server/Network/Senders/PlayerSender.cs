using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Network.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Server.World;
using CryBits.Simulation.Formulas;
using LiteNetLib;
using System;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.State;

namespace CryBits.Server.Network.Senders;

internal sealed class PlayerSender(PackageSender packageSender)
{
    public static PlayerSender Instance { get; } = new(PackageSender.Instance);

    public void Join(EntityId entityId)
    {
        packageSender.ToPlayer(entityId, new JoinPacket { PlayerId = entityId.Value });
    }

    public void JoinGame(EntityId entityId)
    {
        packageSender.ToPlayer(entityId, new JoinGamePacket());
    }

    public void JoinMap(EntityId entityId)
    {
        packageSender.ToPlayer(entityId, new JoinMapPacket());
    }

    public void PlayerLeaveMap(EntityId entityId, Guid mapId)
    {
        packageSender.ToMapBut(mapId, entityId, new PlayerLeavePacket { NetworkId = entityId.Value },
            DeliveryMethod.ReliableUnordered);
    }

    public void PlayerPosition(EntityId entityId)
    {
        var pos = GameWorld.Current.Entities.Get(entityId)!.Get<Position>()!;
        packageSender.ToMap(pos.MapId,
            new PlayerPositionPacket
            { NetworkId = entityId.Value, X = pos.X, Y = pos.Y, Direction = (byte)pos.Direction },
            DeliveryMethod.Sequenced);
    }

    public void PlayerVitals(EntityId entityId)
    {
        var entity = GameWorld.Current.Entities.Get(entityId)!;
        var vitals = entity.Get<Vitals>()!;
        var pos = entity.Get<Position>()!;
        var packet = new PlayerVitalsPacket
        { NetworkId = entityId.Value, Vital = new short[(byte)Vital.Count], MaxVital = new short[(byte)Vital.Count] };
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            packet.Vital[i] = i == 0 ? vitals.Hp : vitals.Mp;
            packet.MaxVital[i] = i == 0 ? vitals.MaxHp : vitals.MaxMp;
        }

        packageSender.ToMap(pos.MapId, packet, DeliveryMethod.ReliableSequenced);
    }

    public void PlayerLeave(EntityId entityId)
    {
        packageSender.ToAllBut(entityId, new PlayerLeavePacket { NetworkId = entityId.Value },
            DeliveryMethod.ReliableUnordered);
    }

    public void PlayerMove(EntityId entityId, byte movement)
    {
        var pos = GameWorld.Current.Entities.Get(entityId)!.Get<Position>()!;
        var speed = movement == (byte)Movement.Moving
            ? RunSpeedPixelsPerSecond
            : WalkSpeedPixelsPerSecond;

        packageSender.ToMapBut(pos.MapId, entityId,
            new PlayerMovePacket
            {
                NetworkId = entityId.Value,
                X = pos.X,
                Y = pos.Y,
                Direction = (byte)pos.Direction,
                Movement = movement,
                Speed = speed
            }, DeliveryMethod.Sequenced);
    }

    public void PlayerDirection(EntityId entityId)
    {
        var pos = GameWorld.Current.Entities.Get(entityId)!.Get<Position>()!;
        packageSender.ToMapBut(pos.MapId, entityId,
            new PlayerDirectionPacket { NetworkId = entityId.Value, Direction = (byte)pos.Direction },
            DeliveryMethod.Sequenced);
    }

    public void PlayerExperience(EntityId entityId)
    {
        var stats = GameWorld.Current.Entities.Get(entityId)!.Get<StatBlock>()!;
        short total = 0;
        for (byte i = 0; i < (byte)CryBits.Definitions.Characters.Attribute.Count; i++) total += stats.Attribute[i];
        var expNeeded = LevelingFormulas.ExperienceNeeded(stats.Level, total, stats.Points);
        packageSender.ToPlayer(entityId,
            new PlayerExperiencePacket
            { Experience = stats.Experience, ExpNeeded = expNeeded, Points = stats.Points });
    }

    public void PlayerEquipments(EntityId entityId)
    {
        var entity = GameWorld.Current.Entities.Get(entityId)!;
        var equip = entity.Get<EquipmentState>()!;
        var pos = entity.Get<Position>()!;
        var packet = new PlayerEquipmentsPacket
        { NetworkId = entityId.Value, Equipments = new Guid[(byte)Equipment.Count] };
        for (byte i = 0; i < (byte)Equipment.Count; i++) packet.Equipments[i] = equip.Slots[i];
        packageSender.ToMap(pos.MapId, packet, DeliveryMethod.ReliableUnordered);
    }

    public void PlayerInventory(EntityId entityId)
    {
        var inv = GameWorld.Current.Entities.Get(entityId)!.Get<InventoryState>()!;
        var packet = new PlayerInventoryPacket
        { ItemIds = new Guid[MaxInventory], Amounts = new short[MaxInventory] };
        for (byte i = 0; i < MaxInventory; i++)
        {
            packet.ItemIds[i] = inv.Slots[i].ItemId;
            packet.Amounts[i] = inv.Slots[i].Amount;
        }

        packageSender.ToPlayer(entityId, packet);
    }

    public void PlayerHotbar(EntityId entityId)
    {
        var hotbar = GameWorld.Current.Entities.Get(entityId)!.Get<HotbarState>()!;
        var packet = new PlayerHotbarPacket { Types = new byte[MaxHotbar], Slots = new byte[MaxHotbar] };
        for (byte i = 0; i < MaxHotbar; i++)
        {
            packet.Types[i] = (byte)hotbar.Slots[i].Type;
            packet.Slots[i] = (byte)hotbar.Slots[i].Slot;
        }

        packageSender.ToPlayer(entityId, packet);
    }
}
