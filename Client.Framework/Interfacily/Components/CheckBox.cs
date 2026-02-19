using System.Drawing;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Interfaces;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.Framework.Interfacily.Components;

public class CheckBox : Component, IMouseUp
{
    public string Text { get; set; } = string.Empty;
    public bool Checked { get; set; }

    public event Action? OnMouseUp;

    /// <summary>Pixels between checkbox texture and label.</summary>
    public const byte Margin = 4;

    public void MouseUp()
    {
        var textureSize = Textures.CheckBox.ToSize();
        var box = new Size(textureSize.Width / 2 + MeasureString(Text) + Margin, textureSize.Height);

        if (!IsAbove(new Rectangle(Position, box))) return;

        Checked = !Checked;

        OnMouseUp?.Invoke();
        Sound.Play(Sounds.Click);
    }

    public override string ToString() => "[CheckBox] " + Name;
}
