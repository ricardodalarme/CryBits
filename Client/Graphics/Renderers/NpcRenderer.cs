using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Graphics.Renderers;

internal static class NpcRenderer
{
    /// <summary>
    /// Render an NPC's shadow and health bar at its world position.
    /// Draws in world space — the SFML view handles panning.
    /// </summary>
    public static void Npc(NpcInstance npcInstance)
    {
        if (npcInstance.Data.Texture <= 0 || npcInstance.Data.Texture > Textures.Characters.Count) return;

        NpcBars(npcInstance);
        CharacterRenderer.CharacterShadow(npcInstance.Data.Texture,
            new Point(npcInstance.PixelX, npcInstance.PixelY));
    }

    private static void NpcBars(NpcInstance npcInstance)
    {
        var texture = Textures.Characters[npcInstance.Data.Texture];
        var value = npcInstance.Vital[(byte)Vital.Hp];

        if (value <= 0 || value >= npcInstance.Data.Vital[(byte)Vital.Hp]) return;

        var position = new Point(
            npcInstance.PixelX,
            npcInstance.PixelY + texture.ToSize().Height / AnimationAmountY + 4);
        var fullWidth = texture.ToSize().Width / AnimationAmountX;
        var width = value * fullWidth / npcInstance.Data.Vital[(byte)Vital.Hp];

        Renders.Render(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        Renders.Render(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }
}
