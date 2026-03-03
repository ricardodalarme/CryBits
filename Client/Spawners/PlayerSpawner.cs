using System;
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
/// Creates player entities in one of two archetypes:
///
/// <b>RemotePlayerArchetype</b> — shared visual/simulation state visible for every player on screen.
///   Name · Transform · Sprite · AnimatedSprite · Movement · CharacterState · DamageTint
///   · Shadow · PlayerTag · Vitals · Text · MapId
///
/// <b>LocalPlayerArchetype</b> — extends the remote archetype with data that is meaningful
///   only for the locally controlled character.
///   … + Inventory · Hotbar · Trade · Party · Level · Attributes · Equipment · Appearance
///       · LocalPlayerTag
/// </summary>
internal static class PlayerSpawner
{
    /// <summary>Creates a remote player entity (minimal archetype).</summary>
    public static Entity Spawn(
        World world,
        string name,
        short textureNum,
        short[] vitals,
        short[] maxVitals,
        byte x, byte y,
        Direction direction,
        Guid mapId)
    {
        var texture = Textures.Characters[textureNum];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        var vitalsComponent = new VitalsComponent();
        vitals.CopyTo(vitalsComponent.Current, 0);
        maxVitals.CopyTo(vitalsComponent.Max, 0);

        return world.Create(
            new NameComponent { Value = name },
            new TransformComponent(x * Globals.Grid, y * Globals.Grid),
            new SpriteComponent(texture),
            new AnimatedSpriteComponent(frameWidth, frameHeight, 0.25f, Globals.AnimationAmountX),
            new MovementComponent { TileX = x, TileY = y, Direction = direction, SpeedPixelsPerSecond = Globals.WalkSpeedPixelsPerSecond },
            new CharacterStateComponent(),
            new DamageTintComponent(),
            new ShadowComponent(),
            new PlayerTagComponent(),
            vitalsComponent,
            new TextComponent(name, Color.White, frameWidth / 2, -frameHeight / 2),
            new MapIdComponent { Value = mapId }
        );
    }

    /// <summary>Creates the local player entity (full archetype).</summary>
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
        Direction direction,
        Guid mapId)
    {
        var texture = Textures.Characters[textureNum];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        var vitalsComponent = new VitalsComponent();
        vitals.CopyTo(vitalsComponent.Current, 0);
        maxVitals.CopyTo(vitalsComponent.Max, 0);

        var attributesComponent = new AttributesComponent();
        attributes.CopyTo(attributesComponent.Values, 0);

        var equipmentComponent = new EquipmentComponent();
        equipment.CopyTo(equipmentComponent.Slots, 0);

        return world.Create(
            new NameComponent { Value = name },
            new TransformComponent(x * Globals.Grid, y * Globals.Grid),
            new SpriteComponent(texture),
            new AnimatedSpriteComponent(frameWidth, frameHeight, 0.25f, Globals.AnimationAmountX),
            new MovementComponent { TileX = x, TileY = y, Direction = direction, SpeedPixelsPerSecond = Globals.WalkSpeedPixelsPerSecond },
            new CharacterStateComponent(),
            new DamageTintComponent(),
            new ShadowComponent(),
            new PlayerTagComponent(),
            vitalsComponent,
            attributesComponent,
            equipmentComponent,
            new InventoryComponent(),
            new HotbarComponent(),
            new AppearanceComponent { TextureNum = textureNum },
            new LevelComponent { Level = level },
            new TradeComponent(),
            new PartyComponent(),
            new LocalPlayerTagComponent(),
            new TextComponent(name, Color.Yellow, frameWidth / 2, -frameHeight / 2),
            new MapIdComponent { Value = mapId }
        );
    }
}
