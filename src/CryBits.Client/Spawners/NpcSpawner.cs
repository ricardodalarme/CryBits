using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using Direction = CryBits.Definitions.Common.Direction;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

internal static class NpcSpawner
{
    public static EntityId Spawn(World world, long npcId, Npc data, byte x, byte y, Direction direction, Vitals currentVitals)
    {
        var texture = Textures.Characters[data.Texture];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        var nameColor = data.Behaviour switch
        {
            Behaviour.Friendly => Color.White,
            Behaviour.AttackOnSight => Color.Red,
            Behaviour.AttackWhenAttacked => new Color(228, 120, 51),
            _ => Color.White
        };

        return world.SpawnBuilder()
            .With(new NetworkId { Value = npcId })
            .With(new PlayerAppearance { Name = data.Name })
            .With(new NameColorComponent { Value = nameColor })
            .With(new TransformComponent { X = x * Globals.Grid, Y = y * Globals.Grid })
            .With(new SpriteComponent { Texture = texture })
            .With(new AnimatedSpriteComponent { FrameWidth = frameWidth, FrameHeight = frameHeight, TimePerFrame = 0.25f, FrameCount = Globals.AnimationAmountX })
            .With(new MovementComponent { TileX = x, TileY = y, Direction = direction, SpeedPixelsPerSecond = Globals.WalkSpeedPixelsPerSecond })
            .With(new AttackComponent())
            .With(new NpcTag())
            .With(new CollidableTag())
            .With(currentVitals)
            .Id;
    }
}
