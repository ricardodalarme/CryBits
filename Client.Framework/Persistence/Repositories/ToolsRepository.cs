using System.Drawing;
using System.Text.Json;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Client.Framework.Persistence.Repositories;

public static class ToolsRepository
{
    // ─── Read ────────────────────────────────────────────────────────────────

    /// <summary>Read tools from the JSON file into memory.</summary>
    public static void Read()
    {
        var file = new FileInfo(Directories.ToolsData.FullName);
        if (!file.Exists) return;

        var root = JsonSerializer.Deserialize<ToolsJsonRoot>(file.OpenRead(), JsonConfig.Options)
                   ?? new ToolsJsonRoot();

        foreach (var screenDto in root.Screens)
        {
            var screen = new Screen { Name = screenDto.Name };
            Screens.List.Add(screen.Name, screen);
            LoadChildren(null, screen.Body, screenDto.Children);
        }
    }

    private static void LoadChildren(Component? parent, List<Component> node, List<ComponentDto> dtos)
    {
        foreach (var dto in dtos)
        {
            Component component = dto switch
            {
                ButtonDto b => new Button
                {
                    Name = b.Name,
                    Position = new Point(b.X, b.Y),
                    Visible = b.Visible,
                    TextureNum = b.TextureNum
                },
                TextBoxDto tb => new TextBox
                {
                    Name = tb.Name,
                    Position = new Point(tb.X, tb.Y),
                    Visible = tb.Visible,
                    MaxCharacters = tb.MaxCharacters,
                    Width = tb.Width,
                    Password = tb.Password
                },
                PanelDto p => new Panel
                {
                    Name = p.Name,
                    Position = new Point(p.X, p.Y),
                    Visible = p.Visible,
                    TextureNum = p.TextureNum
                },
                CheckBoxDto cb => new CheckBox
                {
                    Name = cb.Name,
                    Position = new Point(cb.X, cb.Y),
                    Visible = cb.Visible,
                    Text = cb.Text,
                    Checked = cb.Checked
                },
                _ => throw new InvalidOperationException($"Unknown component DTO type: {dto.GetType().Name}")
            };

            switch (component)
            {
                case Button btn: Buttons.List.TryAdd(btn.Name, btn); break;
                case TextBox tb: TextBoxes.List.TryAdd(tb.Name, tb); break;
                case Panel pnl: Panels.List.TryAdd(pnl.Name, pnl); break;
                case CheckBox chk: CheckBoxes.List.TryAdd(chk.Name, chk); break;
            }

            component.Parent = parent;
            node.Add(component);

            // Recursion into children
            LoadChildren(component, component.Children, dto.Children);
        }
    }

    // ─── DTO conversion helpers (used by Editors to build the JSON tree) ─────

    public static ButtonDto ButtonDto(Button tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        TextureNum = tool.TextureNum
    };

    public static TextBoxDto TextBoxDto(TextBox tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        MaxCharacters = tool.MaxCharacters,
        Width = tool.Width,
        Password = tool.Password
    };

    public static PanelDto PanelDto(Panel tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        TextureNum = tool.TextureNum
    };

    public static CheckBoxDto CheckBoxDto(CheckBox tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        Text = tool.Text,
        Checked = tool.Checked
    };
}
