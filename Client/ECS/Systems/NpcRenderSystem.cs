using System;
using System.Drawing;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Utils;
using CryBits.Enums;
using static CryBits.Client.Utils.TextUtils;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Renders all NPC entities (sprite, name label, HP bar).
/// Queries the world for entities that have both <see cref="NpcDataComponent"/>
/// and <see cref="TransformComponent"/> — the render data lives entirely in
/// components, keeping the system free of any entity-class coupling.
/// </summary>
internal sealed class NpcRenderSystem : IRenderSystem
{
    public void Render(GameContext ctx)
    {
        foreach (var (id, npc, transform) in ctx.World.Query<NpcDataComponent, TransformComponent>())
        {
            if (npc.Data == null) continue;
            if (npc.Data.Texture <= 0 || npc.Data.Texture > Textures.Characters.Count) continue;

            ctx.World.TryGet<AnimationComponent>(id, out var animation);
            ctx.World.TryGet<CharacterSpriteComponent>(id, out var sprite);
            ctx.World.TryGet<VitalsComponent>(id, out var vitals);

            DrawNpc(npc, transform, animation, sprite, vitals);
        }
    }

    private static void DrawNpc(
        NpcDataComponent npc,
        TransformComponent transform,
        AnimationComponent? animation,
        CharacterSpriteComponent? sprite,
        VitalsComponent? vitals)
    {
        var column = DetermineAnimationColumn(transform, animation);
        var hurt = sprite != null && sprite.HurtTimer > 0;
        var screenPos = new Point(CameraUtils.ConvertX(transform.PixelX), CameraUtils.ConvertY(transform.PixelY));

        CharacterRenderer.Character(npc.Data!.Texture, screenPos, transform.Direction, column, hurt);
        DrawName(npc, transform);
        DrawBars(npc, transform, vitals);
    }

    private static byte DetermineAnimationColumn(TransformComponent transform, AnimationComponent? animation)
    {
        if (animation == null) return 0;

        if (animation.IsAttacking && animation.AttackTimer + AttackSpeed / 2 > Environment.TickCount)
            return AnimationAttack;

        if (transform.PixelOffsetX > 8 && transform.PixelOffsetX < Grid) return animation.Frame;
        if (transform.PixelOffsetX < -8 && transform.PixelOffsetX > -Grid) return animation.Frame;
        if (transform.PixelOffsetY > 8 && transform.PixelOffsetY < Grid) return animation.Frame;
        if (transform.PixelOffsetY < -8 && transform.PixelOffsetY > -Grid) return animation.Frame;

        return 0;
    }

    private static void DrawName(NpcDataComponent npc, TransformComponent transform)
    {
        var texture = Textures.Characters[npc.Data!.Texture];
        var nameSize = MeasureString(npc.Data.Name);
        var spriteW = texture.ToSize().Width / AnimationAmount;
        var spriteH = texture.ToSize().Height / AnimationAmount;

        var posX = transform.PixelX + spriteW / 2 - nameSize / 2;
        var posY = transform.PixelY - spriteH / 2;

        var color = npc.Data.Behaviour switch
        {
            Behaviour.Friendly => Color.White,
            Behaviour.AttackOnSight => Color.Red,
            Behaviour.AttackWhenAttacked => new Color(228, 120, 51),
            _ => Color.White
        };

        Renders.DrawText(npc.Data.Name, CameraUtils.ConvertX(posX), CameraUtils.ConvertY(posY), color);
    }

    private static void DrawBars(NpcDataComponent npc, TransformComponent transform, VitalsComponent? vitals)
    {
        if (vitals == null) return;

        var hp = vitals.Current[(byte)Vital.Hp];
        var maxHp = npc.Data!.Vital[(byte)Vital.Hp];

        if (hp <= 0 || hp >= maxHp) return;

        var texture = Textures.Characters[npc.Data.Texture];
        var fullWidth = texture.ToSize().Width / AnimationAmount;
        var barWidth = hp * fullWidth / maxHp;

        var posX = CameraUtils.ConvertX(transform.PixelX);
        var posY = CameraUtils.ConvertY(transform.PixelY) + texture.ToSize().Height / AnimationAmount + 4;

        Renders.Render(Textures.Bars, posX, posY, 0, 4, fullWidth, 4);
        Renders.Render(Textures.Bars, posX, posY, 0, 0, barWidth, 4);
    }
}
