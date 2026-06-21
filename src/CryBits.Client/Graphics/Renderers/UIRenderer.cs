using CryBits.Client.Framework;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Simulation.Components;
using Color = SFML.Graphics.Color;
using Component = CryBits.Client.Framework.Interfacily.Components.Component;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class UIRenderer(
    Renderer renderer,
    ToolsRenderer toolsRenderer,
    GameContext context
)
{
    public static UIRenderer Instance { get; } = new(Renderer.Instance, ToolsRenderer.Instance, GameContext.Instance);

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

    public void DrawChat()
    {
        var tool = ChatView.Panel;
        tool.Visible = TextBox.Focused != null && TextBox.Focused.Name.Equals("Chat");

        if (tool.Visible || Chat.VisibilityTimer >= Environment.TickCount64 && Options.Instance.Chat)
            for (var i = Chat.LinesFirst; i <= Chat.LinesVisible + Chat.LinesFirst; i++)
                if (Chat.Order.Count > i)
                    renderer.DrawText(Chat.Order[i].Text, 16, 461 + 11 * (i - Chat.LinesFirst),
                        Chat.Order[i].Color);

        if (!tool.Visible)
            renderer.DrawText("Press [Enter] to open chat.", ChatView.MessageTextBox.Position.X + 5,
                ChatView.MessageTextBox.Position.Y + 3,
                Color.White);
    }

    public void DrawParty()
    {
        if (context.LocalPlayer.Entity == null) return;

        var world = context.World;
        var party = context.LocalPlayer.GetParty();
        if (party == null) return;

        for (byte i = 0; i < party.Members.Count; i++)
        {
            var entity = context.GetNetworkEntity(party.Members[i].Value);
            renderer.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 0, 82, 8);
            renderer.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 0, 82, 8);
            if (entity != null)
            {
                var vitals = world.Get<Vitals>(entity.Value);
                if (vitals != null)
                {
                    if (vitals.Hp > 0)
                        renderer.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 8,
                            vitals.Hp * 82 / vitals.MaxHp, 8);
                    if (vitals.Mp > 0)
                        renderer.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 16,
                            vitals.Mp * 82 / vitals.MaxMp, 8);
                }
            }
            var name = entity != null
                ? world.Get<PlayerAppearance>(entity.Value)?.Name ?? string.Empty
                : string.Empty;
            renderer.DrawText(name, 10, 79 + 27 * i, Color.White);
        }
    }
}
