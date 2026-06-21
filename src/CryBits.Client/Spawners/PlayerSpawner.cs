using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Client.Spawners;

internal static class PlayerSpawner
{
    public static EntityId Spawn(
        World world,
        long networkId,
        string name,
        short textureNum,
        short[] vitals,
        short[] maxVitals,
        byte x, byte y,
        Direction direction)
    {
        var texture = Textures.Characters[textureNum];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        return world.SpawnBuilder()
            .With(new NetworkId { Value = networkId })
            .With(new PlayerAppearance { Name = name, TextureNum = textureNum })
            .With(new TransformComponent { X = x * Globals.Grid, Y = y * Globals.Grid })
            .With(new SpriteComponent { Texture = texture })
            .With(new AnimatedSpriteComponent { FrameWidth = frameWidth, FrameHeight = frameHeight, TimePerFrame = 0.25f, FrameCount = Globals.AnimationAmountX })
            .With(new MovementComponent { TileX = x, TileY = y, Direction = direction, SpeedPixelsPerSecond = Globals.WalkSpeedPixelsPerSecond })
            .With(new AttackComponent())
            .With(new PlayerTag())
            .With(new CollidableComponent())
            .With(new Vitals { Hp = vitals[0], Mp = vitals[1], MaxHp = maxVitals[0], MaxMp = maxVitals[1] })
            .Id;
    }

    public static EntityId SpawnLocal(
        World world,
        long networkId,
        string name,
        short textureNum,
        short level,
        short[] vitals,
        short[] maxVitals,
        short[] attributes,
        Item?[] equipment,
        byte x, byte y,
        Direction direction)
    {
        var entity = Spawn(world, networkId, name, textureNum, vitals, maxVitals, x, y, direction);

        var equipmentSlots = new Guid[equipment.Length];
        for (var i = 0; i < equipment.Length; i++)
            equipmentSlots[i] = equipment[i]?.Id ?? Guid.Empty;

        var attrComp = new AttributesComponent();
        attributes.CopyTo(attrComp.Values, 0);

        world.Set(entity, attrComp);
        world.Set(entity, new EquipmentState { Slots = equipmentSlots });
        world.Set(entity, new InventoryState());
        world.Set(entity, new HotbarState());
        world.Set(entity, new LevelComponent { Level = level });
        world.Set(entity, new LocalPlayerTagComponent());

        return entity;
    }
}
