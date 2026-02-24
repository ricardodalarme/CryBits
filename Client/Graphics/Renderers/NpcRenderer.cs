using System;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Utils;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Graphics.Renderers;

internal static class NpcRenderer
{
    /// <summary>
    /// Render a temporary NPC (sprite, name and bars).
    /// </summary>
    /// <param name="npcInstance">NPC instance to render.</param>
    public static void Npc(NpcInstance npcInstance)
    {
        byte column = 0;
        var hurt = false;

        if (npcInstance.Data.Texture <= 0 || npcInstance.Data.Texture > Textures.Characters.Count) return;

        if (npcInstance.Attacking && npcInstance.AttackTimer + AttackSpeed / 2 > Environment.TickCount)
            column = AnimationAttack;
        else
        {
            if (npcInstance.X2 > 8 && npcInstance.X2 < Grid) column = npcInstance.Animation;
            else if (npcInstance.X2 < -8 && npcInstance.X2 > Grid * -1) column = npcInstance.Animation;
            else if (npcInstance.Y2 > 8 && npcInstance.Y2 < Grid) column = npcInstance.Animation;
            else if (npcInstance.Y2 < -8 && npcInstance.Y2 > Grid * -1) column = npcInstance.Animation;
        }

        if (npcInstance.Hurt > 0) hurt = true;

        CharacterRenderer.Character(npcInstance.Data.Texture,
            new Point(CameraUtils.ConvertX(npcInstance.PixelX), CameraUtils.ConvertY(npcInstance.PixelY)),
            npcInstance.Direction, column, hurt);
        NpcBars(npcInstance);
    }

    private static void NpcBars(NpcInstance npcInstance)
    {
        var texture = Textures.Characters[npcInstance.Data.Texture];
        var value = npcInstance.Vital[(byte)Vital.Hp];

        // No bar needed when full or dead.
        if (value <= 0 || value >= npcInstance.Data.Vital[(byte)Vital.Hp]) return;

        // Compute bar position.
        var position = new Point(CameraUtils.ConvertX(npcInstance.PixelX),
            CameraUtils.ConvertY(npcInstance.PixelY) + texture.ToSize().Height / AnimationAmount + 4);
        var fullWidth = texture.ToSize().Width / AnimationAmount;
        var width = value * fullWidth / npcInstance.Data.Vital[(byte)Vital.Hp];

        // Draw the health bar.
        Renders.Render(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        Renders.Render(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }
}
