using System;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Utils;
using CryBits.Enums;
using static CryBits.Client.Utils.TextUtils;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal static class NpcRenderer
{
    /// <summary>
    /// Render a temporary NPC (sprite, name and bars).
    /// </summary>
    /// <param name="npc">NPC instance to render.</param>
    public static void Npc(TempNpc npc)
    {
        byte column = 0;
        var hurt = false;

        if (npc.Data.Texture <= 0 || npc.Data.Texture > Textures.Characters.Count) return;

        if (npc.Attacking && npc.AttackTimer + AttackSpeed / 2 > Environment.TickCount)
            column = AnimationAttack;
        else
        {
            if (npc.X2 > 8 && npc.X2 < Grid) column = npc.Animation;
            else if (npc.X2 < -8 && npc.X2 > Grid * -1) column = npc.Animation;
            else if (npc.Y2 > 8 && npc.Y2 < Grid) column = npc.Animation;
            else if (npc.Y2 < -8 && npc.Y2 > Grid * -1) column = npc.Animation;
        }

        if (npc.Hurt > 0) hurt = true;

        CharacterRenderer.Character(npc.Data.Texture,
            new Point(CameraUtils.ConvertX(npc.PixelX), CameraUtils.ConvertY(npc.PixelY)),
            npc.Direction, column, hurt);
        NpcName(npc);
        NpcBars(npc);
    }

    private static void NpcName(TempNpc npc)
    {
        var position = new Point();
        int nameSize = MeasureString(npc.Data.Name);
        var texture = Textures.Characters[npc.Data.Texture];

        // Position the name text above the sprite.
        position.X = npc.PixelX + texture.ToSize().Width / AnimationAmount / 2 - nameSize / 2;
        position.Y = npc.PixelY - texture.ToSize().Height / AnimationAmount / 2;

        // Choose color according to behaviour.
        var color = npc.Data.Behaviour switch
        {
            Behaviour.Friendly => Color.White,
            Behaviour.AttackOnSight => Color.Red,
            Behaviour.AttackWhenAttacked => new Color(228, 120, 51),
            _ => Color.White
        };

        // Draw the name.
        Renders.DrawText(npc.Data.Name, CameraUtils.ConvertX(position.X), CameraUtils.ConvertY(position.Y), color);
    }

    private static void NpcBars(TempNpc npc)
    {
        var texture = Textures.Characters[npc.Data.Texture];
        var value = npc.Vital[(byte)Vital.Hp];

        // No bar needed when full or dead.
        if (value <= 0 || value >= npc.Data.Vital[(byte)Vital.Hp]) return;

        // Compute bar position.
        var position = new Point(CameraUtils.ConvertX(npc.PixelX),
            CameraUtils.ConvertY(npc.PixelY) + texture.ToSize().Height / AnimationAmount + 4);
        var fullWidth = texture.ToSize().Width / AnimationAmount;
        var width = value * fullWidth / npc.Data.Vital[(byte)Vital.Hp];

        // Draw the health bar.
        Renders.Render(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        Renders.Render(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }
}
