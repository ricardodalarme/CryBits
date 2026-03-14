using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Equipment;
using CryBits.Client.Components.Hotbar;
using CryBits.Client.Components.Inventory;
using CryBits.Client.Components.Movement;
using CryBits.Client.Components.Party;
using CryBits.Client.Components.Player;
using CryBits.Client.Components.Trade;
using CryBits.Client.Framework.Graphics;
using CryBits.Entities;
using CryBits.Enums;
using SFML.Graphics;
using Entity = Arch.Core.Entity;

namespace CryBits.Client.Spawners;

/// <summary>
/// Creates a new player entity.
/// </summary>
internal static class PlayerSpawner
{
    /// <summary>
    /// Spawns a remote player with the components shared by all players.
    /// </summary>
    public static Entity Spawn(
        World world,
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

        var vitalsComponent = new VitalsComponent();
        vitals.CopyTo(vitalsComponent.Current, 0);
        maxVitals.CopyTo(vitalsComponent.Max, 0);

        return world.Create(
            new NameComponent { Value = name, NameColor = Color.White },
            new TransformComponent(x * Globals.Grid, y * Globals.Grid),
            new SpriteComponent(texture),
            new AnimatedSpriteComponent(frameWidth, frameHeight, 0.25f, Globals.AnimationAmountX),
            new MovementComponent { TileX = x, TileY = y, Direction = direction, SpeedPixelsPerSecond = Globals.WalkSpeedPixelsPerSecond },
            new CharacterStateComponent(),
            new DamageComponent(),
            new PlayerTagComponent(),
            new CollidableComponent(),
            vitalsComponent
        );
    }

    /// <summary>
    /// Spawns the local player: calls <see cref="Spawn"/> then attaches local-only components.
    /// </summary>
    public static Entity SpawnLocal(
        World world,
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
        var entity = Spawn(world, name, textureNum, vitals, maxVitals, x, y, direction);

        // Override name colour for the local player.
        ref var nameComponent = ref world.Get<NameComponent>(entity);
        nameComponent.NameColor = Color.Yellow;

        var attributesComponent = new AttributesComponent();
        attributes.CopyTo(attributesComponent.Values, 0);

        var equipmentComponent = new EquipmentComponent();
        equipment.CopyTo(equipmentComponent.Slots, 0);

        world.Add(
            entity,
            attributesComponent,
            equipmentComponent,
            new InventoryComponent(),
            new HotbarComponent(),
            new AppearanceComponent { TextureNum = textureNum },
            new LevelComponent { Level = level },
            new TradeComponent(),
            new LocalPlayerTagComponent(),
            new PartyComponent()
        );

        return entity;
    }
}
