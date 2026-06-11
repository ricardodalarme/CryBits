using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Editors.Entities;
using SFML.Graphics;
using System.Drawing;
using Color = SFML.Graphics.Color;

namespace CryBits.Editors.Graphics.Renderers;

internal class InterfaceRenderer(Renderer renderer)
{
    public static InterfaceRenderer Instance { get; } = new(Renderer.Instance);

    public RenderTexture? WinInterface;

    /// <summary>
    /// Render the editor interface tree to the interface render target.
    /// </summary>
    public void Interface(InterfaceNode node)
    {
        if (WinInterface == null) return;
        if (node.Nodes.Count == 0) return;

        WinInterface.Clear();
        InterfaceOrder(WinInterface, node);
        WinInterface.Display();
    }

    private void InterfaceOrder(IRenderTarget target, InterfaceNode node)
    {
        for (byte i = 0; i < node.Nodes.Count; i++)
        {
            var tool = (Component)node.Nodes[i].Tag!;
            if (tool.Visible)
            {
                if (tool is Label label) Label(target, label);
                else if (tool is ProgressBar progressBar) ProgressBar(target, progressBar);
                else if (tool is SlotGrid slotGrid) SlotGrid(target, slotGrid);
                else if (tool is Picture picture) Picture(target, picture);
                else if (tool is Panel panel) Panel(target, panel);
                else if (tool is TextBox textBox) TextBox(target, textBox);
                else if (tool is Button button) Button(target, button);
                else if (tool is CheckBox checkBox) CheckBox(target, checkBox);

                InterfaceOrder(target, node.Nodes[i]);
            }
        }
    }

    private void Label(IRenderTarget target, Label tool)
    {
        var color = new Color((byte)(tool.Color >> 16), (byte)(tool.Color >> 8), (byte)tool.Color);
        renderer.DrawText(target, tool.Text, tool.Position.X + 1, tool.Position.Y + 1, color, tool.Alignment);
    }

    private void ProgressBar(IRenderTarget target, ProgressBar tool)
    {
        if (tool.Width <= 0 || tool.Height <= 0) return;
        // Show the full bar in the editor as a layout preview.
        renderer.Draw(target, Textures.BarsPanel,
            new Rectangle(0, tool.SourceY, tool.Width, tool.Height),
            new Rectangle(tool.Position.X, tool.Position.Y, tool.Width, tool.Height));
    }

    private void SlotGrid(IRenderTarget target, SlotGrid tool)
    {
        if (tool.SlotSize <= 0) return;
        for (var i = 0; i < tool.SlotCount; i++)
        {
            var pos = tool.GetSlotPosition(i);
            renderer.DrawRectangle(target, pos.X, pos.Y, tool.SlotSize, tool.SlotSize, Color.White);
        }
    }

    private void Picture(IRenderTarget target, Picture tool)
    {
        if (tool.Width <= 0 || tool.Height <= 0) return;
        renderer.DrawRectangle(target, tool.Position.X, tool.Position.Y, tool.Width, tool.Height, new Color(150, 150, 255, 180));
    }

    private void Button(IRenderTarget target, Button tool)
    {
        if (tool.TextureNum < Textures.Buttons.Count)
            renderer.Draw(target, Textures.Buttons[tool.TextureNum], tool.Position, new Color(255, 255, 225, 225));
    }

    private void Panel(IRenderTarget target, Panel tool)
    {
        if (tool.TextureNum <= 0 || tool.TextureNum > Textures.Panels.Count) return;
        renderer.Draw(target, Textures.Panels[tool.TextureNum], tool.Position);
    }

    private void CheckBox(IRenderTarget target, CheckBox tool)
    {
        // Configure source/destination rectangles.
        var recSource = new Rectangle(new Point(),
            new Size(Textures.CheckBox.ToSize().Width / 2, Textures.CheckBox.ToSize().Height));
        var recDestiny = new Rectangle(tool.Position, recSource.Size);

        // Use checked state to select marker sprite.
        if (tool.Checked)
            recSource.Location = new Point(Textures.CheckBox.ToSize().Width / 2, 0);

        byte margin = 4;
        renderer.Draw(target, Textures.CheckBox, recSource, recDestiny);
        renderer.DrawText(target, tool.Text, recDestiny.Location.X + Textures.CheckBox.ToSize().Width / 2 + margin,
            recDestiny.Location.Y + 1, Color.White);
    }

    private void TextBox(IRenderTarget target, TextBox tool)
    {
        renderer.DrawBox(target, Textures.TextBox, 3, tool.Position, new Size(tool.Width, Textures.TextBox.ToSize().Height));
    }
}
