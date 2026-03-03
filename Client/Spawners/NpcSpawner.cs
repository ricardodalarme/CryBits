using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Movement;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using SFML.Graphics;

namespace CryBits.Client.Spawners;

/// <summary>
/// Creates a new NPC entity.
/// </summary>
internal static class NpcSpawner
{
    public static Entity Spawn(World world, NpcInstance npc)
    {
        var texture = Textures.Characters[npc.Data.Texture];
        var size = texture.ToSize();
        var frameWidth = size.Width / Globals.AnimationAmountX;
        var frameHeight = size.Height / Globals.AnimationAmountY;

        var textColor = npc.Data.Behaviour switch
        {
            Behaviour.Friendly => Color.White,
            Behaviour.AttackOnSight => Color.Red,
            Behaviour.AttackWhenAttacked => new Color(228, 120, 51),
            _ => Color.White
        };

        // NPC current vitals come from the instance; max vitals come from the NPC template.
        var vitalsComponent = new VitalsComponent();
        npc.Vital.CopyTo(vitalsComponent.Current, 0);
        npc.Data.Vital.CopyTo(vitalsComponent.Max, 0);

        return world.Create(
            new TransformComponent(npc.PixelX, npc.PixelY),
            new SpriteComponent(texture),
            new AnimatedSpriteComponent(frameWidth, frameHeight, 0.25f, Globals.AnimationAmountX),
            new CharacterStateComponent { Direction = npc.Direction },
            new DamageTintComponent(),
            new ShadowComponent(Textures.Shadow),
            new NpcTagComponent(),
            vitalsComponent,
            new TextComponent(npc.Data.Name, textColor, frameWidth / 2, -frameHeight / 2)
        );
    }
}
