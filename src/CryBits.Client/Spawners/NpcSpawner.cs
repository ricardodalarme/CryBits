using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using Direction = CryBits.Definitions.Common.Direction;

namespace CryBits.Client.Spawners;

internal static class NpcSpawner
{
    public static EntityId Spawn(World world, long npcId, Npc data, byte x, byte y, Direction direction, short[] currentVitals)
    {
        var texture = Textures.Characters[data.Texture];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        var vitals = new Vitals
        {
            Hp = currentVitals[0],
            MaxHp = data.Vital[0]
        };
        if (currentVitals.Length > 1) vitals.Mp = currentVitals[1];
        if (data.Vital.Length > 1) vitals.MaxMp = data.Vital[1];

        return world.SpawnBuilder()
            .With(new NetworkId { Value = npcId })
            .With(new PlayerAppearance { Name = data.Name })
            .With(new TransformComponent { X = x * Globals.Grid, Y = y * Globals.Grid })
            .With(new SpriteComponent { Texture = texture })
            .With(new AnimatedSpriteComponent { FrameWidth = frameWidth, FrameHeight = frameHeight, TimePerFrame = 0.25f, FrameCount = Globals.AnimationAmountX })
            .With(new MovementComponent { TileX = x, TileY = y, Direction = direction, SpeedPixelsPerSecond = Globals.WalkSpeedPixelsPerSecond })
            .With(new AttackComponent())
            .With(new NpcTag())
            .With(new CollidableComponent())
            .With(vitals)
            .Id;
    }
}
