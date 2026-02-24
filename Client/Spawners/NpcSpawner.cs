using Arch.Core;
using CryBits.Client.Components;
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
        var frameWidth = size.Width / Globals.AnimationAmount;
        var frameHeight = size.Height / Globals.AnimationAmount;

        var textColor = npc.Data.Behaviour switch
        {
            Behaviour.Friendly => Color.White,
            Behaviour.AttackOnSight => Color.Red,
            Behaviour.AttackWhenAttacked => new Color(228, 120, 51),
            _ => Color.White
        };

        return world.Create(
            new TransformComponent(npc.PixelX, npc.PixelY),
            new TextComponent(npc.Data.Name, textColor, frameWidth / 2, -frameHeight / 2)
        );
    }
}
