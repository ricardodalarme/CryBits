using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class NpcRenderer(Renderer renderer, CharacterRenderer characterRenderer)
{
    public static NpcRenderer Instance { get; } = new(Renderer.Instance, CharacterRenderer.Instance);

    public void DrawNpc(NpcInstance npcInstance)
    {
        if (npcInstance.Data.Texture <= 0 || npcInstance.Data.Texture > Textures.Characters.Count) return;

        DrawBars(npcInstance);
        characterRenderer.DrawShadow(npcInstance.Data.Texture,
            new Point(npcInstance.PixelX, npcInstance.PixelY));
    }

    private void DrawBars(NpcInstance npcInstance)
    {
        var texture = Textures.Characters[npcInstance.Data.Texture];
        var value = npcInstance.Vital[(byte)Vital.Hp];

        if (value <= 0 || value >= npcInstance.Data.Vital[(byte)Vital.Hp]) return;

        var position = new Point(
            npcInstance.PixelX,
            npcInstance.PixelY + texture.ToSize().Height / AnimationAmountY + 4);
        var fullWidth = texture.ToSize().Width / AnimationAmountX;
        var width = value * fullWidth / npcInstance.Data.Vital[(byte)Vital.Hp];

        renderer.Draw(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
        renderer.Draw(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
    }
}
