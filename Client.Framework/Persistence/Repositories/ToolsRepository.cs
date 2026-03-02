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
                LabelDto l => new Label
                {
                    Name = l.Name,
                    Position = new Point(l.X, l.Y),
                    Visible = l.Visible,
                    Text = l.Text,
                    Color = l.Color,
                    Alignment = l.Alignment,
                    MaxWidth = l.MaxWidth
                },
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
                ProgressBarDto pb => new ProgressBar
                {
                    Name = pb.Name,
                    Position = new Point(pb.X, pb.Y),
                    Visible = pb.Visible,
                    SourceY = pb.SourceY,
                    Width = pb.Width,
                    Height = pb.Height
                },
                SlotGridDto sg => new SlotGrid
                {
                    Name = sg.Name,
                    Position = new Point(sg.X, sg.Y),
                    Visible = sg.Visible,
                    Columns = sg.Columns,
                    SlotSize = sg.SlotSize,
                    Padding = sg.Padding,
                    Rows = sg.Rows
                },
                _ => throw new InvalidOperationException($"Unknown component DTO type: {dto.GetType().Name}")
            };

            switch (component)
            {
                case Label lbl: Tools.Labels.TryAdd(lbl.Name, lbl); break;
                case Button btn: Tools.Buttons.TryAdd(btn.Name, btn); break;
                case TextBox tb: Tools.TextBoxes.TryAdd(tb.Name, tb); break;
                case Panel pnl: Tools.Panels.TryAdd(pnl.Name, pnl); break;
                case CheckBox chk: Tools.CheckBoxes.TryAdd(chk.Name, chk); break;
                case ProgressBar bar: Tools.ProgressBars.TryAdd(bar.Name, bar); break;
                case SlotGrid sg: Tools.SlotGrids.TryAdd(sg.Name, sg); break;
            }

            component.Parent = parent;
            node.Add(component);

            // Recursion into children
            LoadChildren(component, component.Children, dto.Children);
        }
    }

    // ─── DTO conversion helpers (used by Editors to build the JSON tree) ─────
    public static LabelDto LabelDto(Label tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        Text = tool.Text,
        Color = tool.Color,
        Alignment = tool.Alignment,
        MaxWidth = tool.MaxWidth
    };

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

    public static ProgressBarDto ProgressBarDto(ProgressBar tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        SourceY = tool.SourceY,
        Width = tool.Width,
        Height = tool.Height
    };

    public static SlotGridDto SlotGridDto(SlotGrid tool) => new()
    {
        Name = tool.Name,
        X = tool.Position.X,
        Y = tool.Position.Y,
        Visible = tool.Visible,
        Columns = tool.Columns,
        SlotSize = tool.SlotSize,
        Padding = tool.Padding,
        Rows = tool.Rows
    };
}
