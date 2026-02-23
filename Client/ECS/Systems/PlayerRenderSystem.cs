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
/// Renders all player entities (sprite, name label, HP bar).
/// Remote players that are on a different map than the local player are skipped.
/// The local player is always rendered last so it appears on top of other players.
/// </summary>
internal sealed class PlayerRenderSystem : IRenderSystem
{
    public void Render(GameContext ctx)
    {
        var localId = ctx.GetLocalPlayer();
        var localMapId = GetMapId(ctx, localId);

        // Draw remote players first.
        foreach (var (id, player, transform) in ctx.World.Query<PlayerDataComponent, TransformComponent>())
        {
            if (id == localId) continue;

            // Skip players on other maps.
            if (GetMapId(ctx, id) != localMapId) continue;

            ctx.World.TryGet<AnimationComponent>(id, out var animation);
            ctx.World.TryGet<CharacterSpriteComponent>(id, out var sprite);
            ctx.World.TryGet<VitalsComponent>(id, out var vitals);

            DrawPlayer(player, transform, animation, sprite, vitals);
        }

        // Draw the local player on top.
        if (localId < 0) return;
        if (!ctx.World.TryGet<PlayerDataComponent>(localId, out var localPlayer)) return;
        if (!ctx.World.TryGet<TransformComponent>(localId, out var localTransform)) return;

        ctx.World.TryGet<AnimationComponent>(localId, out var localAnim);
        ctx.World.TryGet<CharacterSpriteComponent>(localId, out var localSprite);
        ctx.World.TryGet<VitalsComponent>(localId, out var localVitals);

        DrawPlayer(localPlayer, localTransform, localAnim, localSprite, localVitals);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static Guid? GetMapId(GameContext ctx, int entityId)
    {
        if (entityId < 0) return null;
        return ctx.World.TryGet<MapContextComponent>(entityId, out var mc) ? mc.MapId : null;
    }

    private static void DrawPlayer(
        PlayerDataComponent player,
        TransformComponent transform,
        AnimationComponent? animation,
        CharacterSpriteComponent? sprite,
        VitalsComponent? vitals)
    {
        if (sprite == null || sprite.TextureNum <= 0 || sprite.TextureNum > Textures.Characters.Count) return;

        var column = DetermineAnimationColumn(transform, animation);
        var hurt = sprite.HurtTimer > 0;
        var screen = new Point(CameraUtils.ConvertX(transform.PixelX), CameraUtils.ConvertY(transform.PixelY));

        CharacterRenderer.Character(sprite.TextureNum, screen, transform.Direction, column, hurt);
        DrawName(player, sprite, transform);
        DrawBars(sprite, transform, vitals);
    }

    private static byte DetermineAnimationColumn(TransformComponent transform, AnimationComponent? animation)
    {
        if (animation == null) return AnimationStopped;

        if (animation.IsAttacking && animation.AttackTimer + AttackSpeed / 2 > Environment.TickCount)
            return AnimationAttack;

        if (transform.PixelOffsetX > 8 && transform.PixelOffsetX < Grid) return animation.Frame;
        if (transform.PixelOffsetX < -8 && transform.PixelOffsetX > -Grid) return animation.Frame;
        if (transform.PixelOffsetY > 8 && transform.PixelOffsetY < Grid) return animation.Frame;
        if (transform.PixelOffsetY < -8 && transform.PixelOffsetY > -Grid) return animation.Frame;

        return AnimationStopped;
    }

    private static void DrawName(
        PlayerDataComponent player,
        CharacterSpriteComponent sprite,
        TransformComponent transform)
    {
        var texture = Textures.Characters[sprite.TextureNum];
        var nameSize = MeasureString(player.Name);
        var spriteW = texture.ToSize().Width / AnimationAmount;

        var posX = CameraUtils.ConvertX(transform.PixelX) + spriteW / 2 - nameSize / 2;
        var posY = CameraUtils.ConvertY(transform.PixelY) - texture.ToSize().Height / AnimationAmount / 2;

        Renders.DrawText(player.Name, posX, posY, Color.White);
    }

    private static void DrawBars(
        CharacterSpriteComponent sprite,
        TransformComponent transform,
        VitalsComponent? vitals)
    {
        if (vitals == null) return;

        var hp = vitals.Current[(byte)Vital.Hp];
        var maxHp = vitals.Max[(byte)Vital.Hp];

        if (hp <= 0 || hp >= maxHp) return;

        var texture = Textures.Characters[sprite.TextureNum];
        var fullWidth = texture.ToSize().Width / AnimationAmount;
        var barWidth = hp * fullWidth / maxHp;

        var posX = CameraUtils.ConvertX(transform.PixelX);
        var posY = CameraUtils.ConvertY(transform.PixelY) + texture.ToSize().Height / AnimationAmount + 4;

        Renders.Render(Textures.Bars, posX, posY, 0, 4, fullWidth, 4);
        Renders.Render(Textures.Bars, posX, posY, 0, 0, barWidth, 4);
    }
}
