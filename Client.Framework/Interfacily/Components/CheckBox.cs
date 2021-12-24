using System.Drawing;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Interfaces;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.Framework.Interfacily.Components;

public class CheckBox : Component, IMouseUp
{
    // Propriedades
    public string Text { get; set; } = string.Empty;
    public bool Checked { get; set; }

    // Ações
    public event Action? OnMouseUp;

    // Margem da textura até o texto
    public const byte Margin = 4;

    // Eventos
    public void MouseUp()
    {
        // Tamanho do marcador
        var textureSize = Textures.CheckBox.ToSize();
        var box = new Size(textureSize.Width / 2 + MeasureString(Text) + Margin, textureSize.Height);

        // Somente se estiver sobrepondo a ferramenta
        if (!IsAbove(new Rectangle(Position, box))) return;

        // Altera o estado do marcador
        Checked = !Checked;

        // Executa o evento
        OnMouseUp?.Invoke();
        Sound.Play(Sounds.Click);
    }

    public override string ToString() => "[CheckBox] " + Name;
}