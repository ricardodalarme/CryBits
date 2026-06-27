using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using SFML.Graphics;
using MovementState = CryBits.Definitions.Common.Movement;

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
        int x, int y,
        Direction direction)
    {
        var texture = Textures.Characters[textureNum];

        return world.SpawnBuilder()
            .With(new NetworkId(networkId))
            .With(new PlayerAppearance(Name: name, ClassId: Guid.Empty, TextureNum: textureNum, Gender: Gender.Male))
            .With(new NameColorComponent(Color.White))
            .With(new TransformComponent(x * Globals.Grid, y * Globals.Grid))
            .With(new SpriteComponent(texture, null, Color.White))
            .With(new AnimationState(0, 0, 0f, CharacterAnimation.Idle))
            .With(new MovementComponent(x, y, 0f, 0f, Globals.WalkSpeedPixelsPerSecond, MovementState.Stopped, direction))
            .With(new AttackComponent())
            .With(new PlayerTag())
            .With(new CollidableTag())
            .With(new Vitals(Hp: vitals[0], Mp: vitals[1], MaxHp: maxVitals[0], MaxMp: maxVitals[1]))
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
        int x, int y,
        Direction direction)
    {
        var entity = Spawn(world, networkId, name, textureNum, vitals, maxVitals, x, y, direction);

        world.Set(entity, new NameColorComponent(Color.Yellow));

        var equipmentSlots = new Guid[equipment.Length];
        for (var i = 0; i < equipment.Length; i++)
            equipmentSlots[i] = equipment[i]?.Id ?? Guid.Empty;

        var attrsArray = new short[attributes.Length];
        attributes.CopyTo(attrsArray, 0);

        world.Set(entity, new LevelComponent(Level: level));
        world.Set(entity, new AttributesComponent(attrsArray));
        world.Set(entity, new EquipmentState(equipmentSlots));
        world.Set(entity, new InventoryState(new ItemSlot[Globals.MaxInventory]));
        world.Set(entity, new HotbarState(new HotbarSlot[Globals.MaxHotbar]));
        world.Set(entity, new LocalPlayerTag());

        return entity;
    }
}
