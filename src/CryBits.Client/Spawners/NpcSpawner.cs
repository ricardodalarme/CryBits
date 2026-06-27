using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using SFML.Graphics;
using Direction = CryBits.Definitions.Common.Direction;
using MovementState = CryBits.Definitions.Common.Movement;

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
            .With(new NetworkId(npcId))
            .With(new PlayerAppearance(Name: data.Name, ClassId: Guid.Empty, TextureNum: 0, Gender: Gender.Male))
            .With(new NameColorComponent(nameColor))
            .With(new TransformComponent(x * Globals.Grid, y * Globals.Grid))
            .With(new SpriteComponent(texture, null, SFML.Graphics.Color.White))
            .With(new AnimatedSpriteComponent(frameWidth, frameHeight, Globals.AnimationAmountX, 0.25f, 0f, 0, 0, true))
            .With(new MovementComponent(x, y, 0f, 0f, Globals.WalkSpeedPixelsPerSecond, MovementState.Stopped, direction))
            .With(new AttackComponent())
            .With(new NpcTag())
            .With(new CollidableTag())
            .With(currentVitals)
            .Id;
    }
}
