using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Core;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Enums;

namespace CryBits.Client.Systems.Character;

/// <summary>
/// Draws a HP bar below each character that is currently damaged.
/// The bar is intentionally hidden when the entity is at full health to keep
/// the screen clean — it only appears to communicate "this character took damage".
///
/// Bar anatomy (matches the legacy renderer's Bars texture layout):
///   Row Y=4 (4 px tall) — grey background track drawn at full frame width.
///   Row Y=0 (4 px tall) — coloured fill drawn proportional to remaining HP%.
///
/// Positioned directly beneath the sprite frame (frameHeight + 4 px gap),
/// which places it just outside the character's feet.
/// </summary>
internal sealed class VitalBarRenderSystem(World world, Renderer renderer) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<TransformComponent, VitalsComponent, AnimatedSpriteComponent>();

    public override void Update(in int t)
    {
        World.Query(in _query,
            (ref TransformComponent transform, ref VitalsComponent vitals, ref AnimatedSpriteComponent anim) =>
            {
                var hp = vitals.Current[(byte)Vital.Hp];
                var maxHp = vitals.Max[(byte)Vital.Hp];

                // Bar is hidden at full health or when the entity is dead (hp == 0 edge case).
                if (hp <= 0 || hp >= maxHp) return;

                var barX = transform.X;
                var barY = transform.Y + anim.FrameHeight + 4;
                var fullWidth = anim.FrameWidth;
                var fillWidth = hp * fullWidth / maxHp;

                // Background track (source row 1, offset y=4).
                renderer.Draw(Textures.Bars, barX, barY, 0, 4, fullWidth, 4);

                // HP fill (source row 0, offset y=0).
                renderer.Draw(Textures.Bars, barX, barY, 0, 0, fillWidth, 4);
            });
    }
}
