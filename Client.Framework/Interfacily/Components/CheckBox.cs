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
        var boxWidth = textureSize.X / 2 + MeasureString(Text) + Margin;

        if (!IsAbove(new Rectangle(Position.X, Position.Y, boxWidth, textureSize.Y))) return;

        Checked = !Checked;

        OnMouseUp?.Invoke();
        AudioManager.Instance.PlaySound(Sounds.Click);
    }

    public override string ToString() => "[CheckBox] " + Name;
}
