using CryBits.Client.Framework.Graphics;
using CryBits.Editors.Forms;
using SFML.Graphics;
using System.Drawing;

namespace CryBits.Editors.Graphics.Renderers;

internal class CharacterRenderer(Renderer renderer)
{
    public static CharacterRenderer Instance { get; } = new(Renderer.Instance);

    public RenderTexture? WinCharacter;

    /// <summary>
    /// Render a character preview.
    /// </summary>
    public void Character()
    {
        if (WinCharacter == null) return;

        WinCharacter.Clear();
        Character(WinCharacter, EditorNpcsWindow.CurrentTextureIndex);
        WinCharacter.Display();
    }

    private void Character(RenderTexture target, short textureNum)
    {
        var texture = Textures.Characters[textureNum];
        var size = new Size(texture.ToSize().Width / 4, texture.ToSize().Height / 4);

        if (textureNum > 0 && textureNum < Textures.Characters.Count)
            renderer.Draw(target, texture, (int)(target.Size.X - size.Width) / 2, (int)(target.Size.Y - size.Height) / 2, 0, 0,
                size.Width, size.Height);
    }
}
