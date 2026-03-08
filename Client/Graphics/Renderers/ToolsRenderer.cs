using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using SFML.Graphics;
using SFML.System;
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

        renderer.Draw(Textures.Buttons[tool.TextureNum], new Vector2i(tool.Position.X, tool.Position.Y), new Color(255, 255, 225, alpha));
    }

    public void DrawPanel(Panel tool)
    {
        if (tool.TextureNum <= 0 || tool.TextureNum > Textures.Panels.Count) return;
        renderer.Draw(Textures.Panels[tool.TextureNum], new Vector2i(tool.Position.X, tool.Position.Y));
    }

    public void DrawCheckBox(CheckBox tool)
    {
        var texSize = Textures.CheckBox.ToSize();
        var halfW = texSize.X / 2;
        var recSource = new IntRect(new Vector2i(0, 0), new Vector2i(halfW, texSize.Y));
        var recDestiny = new IntRect(new Vector2i(tool.Position.X, tool.Position.Y), recSource.Size);

        if (tool.Checked) recSource = new IntRect(new Vector2i(halfW, 0), recSource.Size);

        renderer.Draw(Textures.CheckBox, recSource, recDestiny);
        renderer.DrawText(tool.Text,
            recDestiny.Position.X + halfW + CheckBox.Margin,
            recDestiny.Position.Y + 1,
            Color.White);
    }

    public void DrawSlotGrid(SlotGrid tool) => tool.RenderSlots();

    public void DrawPicture(Picture tool) => tool.Render();

    public void DrawTextBox(TextBox tool)
    {
        var pos = new Vector2i(tool.Position.X, tool.Position.Y);
        var text = tool.Text;

        renderer.DrawBox(Textures.TextBox, 3, pos,
            new Vector2i(tool.Width, Textures.TextBox.ToSize().Y));

        if (tool.Password && !string.IsNullOrEmpty(text)) text = new string('•', text.Length);

        text = TextBreak(text, tool.Width - 10);

        if (TextBox.Focused != null &&
            TextBox.Focused == tool && TextBox.BlinkSignal) text += "|";
        renderer.DrawText(text, pos.X + 4, pos.Y + 2, Color.White);
    }
}
