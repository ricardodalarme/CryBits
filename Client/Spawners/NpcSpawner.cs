using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Movement;
using CryBits.Client.Framework.Graphics;
using CryBits.Entities.Npc;
using CryBits.Enums;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

/// <summary>
/// Creates a new NPC entity.
/// </summary>
internal static class NpcSpawner
{
    public static Entity Spawn(World world, Npc data, byte x, byte y, Direction direction, short[] currentVitals)
    {
        var texture = Textures.Characters[data.Texture];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        var textColor = data.Behaviour switch
        {
            Behaviour.Friendly => Color.White,
            Behaviour.AttackOnSight => Color.Red,
            Behaviour.AttackWhenAttacked => new Color(228, 120, 51),
            _ => Color.White
        };

        var vitalsComponent = new VitalsComponent();
        currentVitals.CopyTo(vitalsComponent.Current, 0);
        data.Vital.CopyTo(vitalsComponent.Max, 0);

        return world.Create(
            new NameComponent { Value = data.Name, NameColor = textColor },
            new TransformComponent(x * Globals.Grid, y * Globals.Grid),
            new SpriteComponent(texture),
            new AnimatedSpriteComponent(frameWidth, frameHeight, 0.25f, Globals.AnimationAmountX),
            new MovementComponent { TileX = x, TileY = y, Direction = direction, SpeedPixelsPerSecond = Globals.WalkSpeedPixelsPerSecond },
            new AttackComponent(),
            new DamageComponent(),
            new NpcTagComponent(),
            new CollidableComponent(),
            vitalsComponent
        );
    }
}
