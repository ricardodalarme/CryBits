using System;
using System.Collections.Generic;
using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Logic;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.Views;
using CryBits.Enums;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class UIRenderer(
    Renderer renderer,
    ToolsRenderer toolsRenderer
)
{
    public static UIRenderer Instance { get; } = new(Renderer.Instance, ToolsRenderer.Instance);

    /// <summary>
    /// Recursively render a tree of UI components.
    /// </summary>
    /// <param name="node">Top-level component list to render.</param>
    public void DrawInterface(List<Component> node)
    {
        foreach (var tool in node)
            if (tool.Visible)
            {
                switch (tool)
                {
                    case Label label: toolsRenderer.DrawLabel(label); break;
                    case Panel panel: toolsRenderer.DrawPanel(panel); break;
                    case TextBox textBox: toolsRenderer.DrawTextBox(textBox); break;
                    case Button button: toolsRenderer.DrawButton(button); break;
                    case CheckBox checkBox: toolsRenderer.DrawCheckBox(checkBox); break;
                    case ProgressBar progressBar: toolsRenderer.DrawProgressBar(progressBar); break;
                    case SlotGrid slotGrid: toolsRenderer.DrawSlotGrid(slotGrid); break;
                    case Picture picture: toolsRenderer.DrawPicture(picture); break;
                }

                DrawInterface(tool.Children);
            }
    }

    /// <summary>
    /// Render chat messages and prompt if chat is not focused.
    /// </summary>
    public void DrawChat()
    {
        var tool = ChatView.Panel;
        tool.Visible = TextBox.Focused != null && TextBox.Focused.Name.Equals("Chat");

        if (tool.Visible || GameLoop.ChatTimer >= Environment.TickCount && Options.Chat)
            for (var i = Chat.LinesFirst; i <= Chat.LinesVisible + Chat.LinesFirst; i++)
                if (Chat.Order.Count > i)
                    renderer.DrawText(Chat.Order[i].Text, 16, 461 + 11 * (i - Chat.LinesFirst),
                        Chat.Order[i].Color);

        if (!tool.Visible)
            renderer.DrawText("Press [Enter] to open chat.", ChatView.MessageTextBox.Position.X + 5,
                ChatView.MessageTextBox.Position.Y + 3,
                Color.White);
    }

    /// <summary>
    /// Render the party member bars and names.
    /// </summary>
    public void DrawParty()
    {
        for (byte i = 0; i < Player.Me.Party.Length; i++)
        {
            renderer.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 0, 82, 8);
            renderer.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 0, 82, 8);
            if (Player.Me.Party[i].Vital[(byte)Vital.Hp] > 0)
                renderer.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 8,
                    Player.Me.Party[i].Vital[(byte)Vital.Hp] * 82 / Player.Me.Party[i].MaxVital[(byte)Vital.Hp],
                    8);
            if (Player.Me.Party[i].Vital[(byte)Vital.Mp] > 0)
                renderer.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 16,
                    Player.Me.Party[i].Vital[(byte)Vital.Mp] * 82 / Player.Me.Party[i].MaxVital[(byte)Vital.Mp],
                    8);

            renderer.DrawText(Player.Me.Party[i].Name, 10, 79 + 27 * i, Color.White);
        }
    }
}
