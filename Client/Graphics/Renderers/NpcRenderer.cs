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
        if (npcInstance.Data.Texture <= 0 || npcInstance.Data.Texture > Textures.Characters.Count) return;

        NpcBars(npcInstance);
        CharacterRenderer.CharacterShadow(npcInstance.Data.Texture,
            new Point(CameraUtils.ConvertX(npcInstance.PixelX), CameraUtils.ConvertY(npcInstance.PixelY)));
    }

    private static void NpcBars(NpcInstance npcInstance)
    {
        var texture = Textures.Characters[npcInstance.Data.Texture];
        var value = npcInstance.Vital[(byte)Vital.Hp];

        // No bar needed when full or dead.
        if (value <= 0 || value >= npcInstance.Data.Vital[(byte)Vital.Hp]) return;

        // Compute bar position.
        var position = new Point(CameraUtils.ConvertX(npcInstance.PixelX),
            CameraUtils.ConvertY(npcInstance.PixelY) + texture.ToSize().Height / AnimationAmountY + 4);
        var fullWidth = texture.ToSize().Width / AnimationAmountX;
        var width = value * fullWidth / npcInstance.Data.Vital[(byte)Vital.Hp];

        // Draw the health bar.
        Renders.Render(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        Renders.Render(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }
}
