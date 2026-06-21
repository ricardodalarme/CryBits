using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Formulas;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.State;
using CryBits.Transport;

namespace CryBits.Host.Network.Senders;

internal sealed class PlayerSender(PackageSender packageSender, EntityRegistry entities)
{
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
            DeliveryChannel.ReliableUnordered);
    }

    public void PlayerPosition(EntityId entityId)
    {
        var pos = entities.Get(entityId)!.Get<Position>()!;
        packageSender.ToMap(pos.MapId,
            new PlayerPositionPacket
            { NetworkId = entityId.Value, X = pos.X, Y = pos.Y, Direction = (byte)pos.Direction },
            DeliveryChannel.Sequenced);
    }

    public void PlayerVitals(EntityId entityId)
    {
        var entity = entities.Get(entityId)!;
        var vitals = entity.Get<Vitals>()!;
        var pos = entity.Get<Position>()!;
        var packet = new PlayerVitalsPacket
        { NetworkId = entityId.Value, Vital = new short[(byte)Vital.Count], MaxVital = new short[(byte)Vital.Count] };
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            packet.Vital[i] = i == 0 ? vitals.Hp : vitals.Mp;
            packet.MaxVital[i] = i == 0 ? vitals.MaxHp : vitals.MaxMp;
        }

        packageSender.ToMap(pos.MapId, packet, DeliveryChannel.ReliableSequenced);
    }

    public void PlayerLeave(EntityId entityId)
    {
        packageSender.ToAllBut(entityId, new PlayerLeavePacket { NetworkId = entityId.Value },
            DeliveryChannel.ReliableUnordered);
    }

    public void PlayerMove(EntityId entityId, byte movement)
    {
        var pos = entities.Get(entityId)!.Get<Position>()!;
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
            }, DeliveryChannel.Sequenced);
    }

    public void PlayerDirection(EntityId entityId)
    {
        var pos = entities.Get(entityId)!.Get<Position>()!;
        packageSender.ToMapBut(pos.MapId, entityId,
            new PlayerDirectionPacket { NetworkId = entityId.Value, Direction = (byte)pos.Direction },
            DeliveryChannel.Sequenced);
    }

    public void PlayerExperience(EntityId entityId)
    {
        var level = entities.Get(entityId)!.Get<LevelComponent>()!;
        var attrs = entities.Get(entityId)!.Get<AttributesComponent>()!;
        short total = 0;
        for (byte i = 0; i < (byte)CryBits.Definitions.Characters.Attribute.Count; i++) total += attrs.Values[i];
        var expNeeded = LevelingFormulas.ExperienceNeeded(level.Level, total, (byte)level.Points);
        packageSender.ToPlayer(entityId,
            new PlayerExperiencePacket
            { Experience = level.Experience, ExpNeeded = expNeeded, Points = (byte)level.Points });
    }

    public void PlayerEquipments(EntityId entityId)
    {
        var entity = entities.Get(entityId)!;
        var equip = entity.Get<EquipmentState>()!;
        var pos = entity.Get<Position>()!;
        var packet = new PlayerEquipmentsPacket
        { NetworkId = entityId.Value, Equipments = new Guid[(byte)Equipment.Count] };
        for (byte i = 0; i < (byte)Equipment.Count; i++) packet.Equipments[i] = equip.Slots[i];
        packageSender.ToMap(pos.MapId, packet, DeliveryChannel.ReliableUnordered);
    }

    public void PlayerInventory(EntityId entityId)
    {
        var inv = entities.Get(entityId)!.Get<InventoryState>()!;
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
        var hotbar = entities.Get(entityId)!.Get<HotbarState>()!;
        var packet = new PlayerHotbarPacket { Types = new byte[MaxHotbar], Slots = new byte[MaxHotbar] };
        for (byte i = 0; i < MaxHotbar; i++)
        {
            packet.Types[i] = (byte)hotbar.Slots[i].Type;
            packet.Slots[i] = (byte)hotbar.Slots[i].Slot;
        }

        packageSender.ToPlayer(entityId, packet);
    }
}
