using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using static CryBits.Client.Utils.TextUtils;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class ToolsRenderer(Renderer renderer)
{
    public static ToolsRenderer Instance { get; } = new(Renderer.Instance);

    public void DrawLabel(Label tool)
    {
        var color = new Color((byte)(tool.Color >> 16), (byte)(tool.Color >> 8), (byte)tool.Color);
        if (tool.MaxWidth > 0)
            renderer.DrawText(tool.FormattedText(), tool.Position.X, tool.Position.Y, color, tool.MaxWidth);
        else
            renderer.DrawText(tool.FormattedText(), tool.Position.X, tool.Position.Y, color, tool.Alignment);
    }

    public void DrawProgressBar(ProgressBar tool)
    {
        if (tool.FillWidth <= 0) return;
        renderer.Draw(Textures.BarsPanel, tool.Position.X, tool.Position.Y, 0, tool.SourceY, tool.FillWidth, tool.Height);
    }

    public void DrawButton(Button tool)
    {
        byte alpha = tool.ButtonState switch
        {
            ButtonState.Above => 250,
            ButtonState.Click => 200,
            _ => 225
        };

        renderer.Draw(Textures.Buttons[tool.TextureNum], tool.Position, new Color(255, 255, 225, alpha));
    }

    public void DrawPanel(Panel tool)
    {
        renderer.Draw(Textures.Panels[tool.TextureNum], tool.Position);
    }

    public void DrawCheckBox(CheckBox tool)
    {
        var recSource = new Rectangle(new Point(),
            new Size(Textures.CheckBox.ToSize().Width / 2, Textures.CheckBox.ToSize().Height));
        var recDestiny = new Rectangle(tool.Position, recSource.Size);

        if (tool.Checked) recSource.Location = new Point(Textures.CheckBox.ToSize().Width / 2, 0);

        renderer.Draw(Textures.CheckBox, recSource, recDestiny);
        renderer.DrawText(tool.Text,
            recDestiny.Location.X + Textures.CheckBox.ToSize().Width / 2 +
            CheckBox.Margin, recDestiny.Location.Y + 1, Color.White);
    }

    public void DrawSlotGrid(SlotGrid tool) => tool.RenderSlots();

    public void DrawPicture(Picture tool) => tool.Render();

    public void DrawTextBox(TextBox tool)
    {
        var position = tool.Position;
        var text = tool.Text;

        renderer.DrawBox(Textures.TextBox, 3, tool.Position,
            new Size(tool.Width, Textures.TextBox.ToSize().Height));

        if (tool.Password && !string.IsNullOrEmpty(text)) text = new string('•', text.Length);

        text = TextBreak(text, tool.Width - 10);

        if (TextBox.Focused != null &&
            TextBox.Focused == tool && TextBox.BlinkSignal) text += "|";
        renderer.DrawText(text, position.X + 4, position.Y + 2, Color.White);
    }
}
