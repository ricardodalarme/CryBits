using System.Drawing;
using System.Text.Json;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Persistence.Dtos;
using CryBits.Utils;

namespace CryBits.Client.Framework.Persistence.Repositories;

public class ToolsRepository
{
    public static ToolsRepository Instance { get; } = new();

    // ─── Read ────────────────────────────────────────────────────────────────

    /// <summary>Deserialize Tools.json into <see cref="Screens.List"/> and <see cref="Tools"/> dictionaries.</summary>
    public void Read()
    {
        var file = new FileInfo(Directories.ToolsData.FullName);
        if (!file.Exists) return;

        using var stream = file.OpenRead();
        var root = JsonSerializer.Deserialize<ToolsJsonRoot>(stream, JsonConfig.Options) ?? new ToolsJsonRoot();

        foreach (var screenDto in root.Screens)
        {
            var screen = new Screen { Name = screenDto.Name };
            Screens.List.Add(screen.Name, screen);
            LoadChildren(null, screen.Body, screenDto.Children);
        }
    }

    private void LoadChildren(Component? parent, List<Component> body, List<ComponentDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var component = FromDto(dto);
            Register(component);
            component.Parent = parent;
            body.Add(component);
            LoadChildren(component, component.Children, dto.Children);
        }
    }

    /// <summary>Deserialize a single <see cref="ComponentDto"/> into its runtime <see cref="Component"/>.</summary>
    private static Component FromDto(ComponentDto dto) => dto switch
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
        PictureDto pic => new Picture
        {
            Name = pic.Name,
            Position = new Point(pic.X, pic.Y),
            Visible = pic.Visible,
            Width = pic.Width,
            Height = pic.Height
        },
        _ => throw new InvalidOperationException($"Unknown component DTO type: {dto.GetType().Name}")
    };

    private void Register(Component component)
    {
        switch (component)
        {
            case Label c: Tools.Labels.TryAdd(c.Name, c); break;
            case Button c: Tools.Buttons.TryAdd(c.Name, c); break;
            case TextBox c: Tools.TextBoxes.TryAdd(c.Name, c); break;
            case Panel c: Tools.Panels.TryAdd(c.Name, c); break;
            case CheckBox c: Tools.CheckBoxes.TryAdd(c.Name, c); break;
            case ProgressBar c: Tools.ProgressBars.TryAdd(c.Name, c); break;
            case SlotGrid c: Tools.SlotGrids.TryAdd(c.Name, c); break;
            case Picture c: Tools.Pictures.TryAdd(c.Name, c); break;
        }
    }

    // ─── Write ───────────────────────────────────────────────────────────────

    /// <summary>Serialize all screens in memory back to Tools.json.</summary>
    public void Write()
    {
        var root = new ToolsJsonRoot();

        foreach (var screen in Screens.List.Values)
        {
            var screenDto = new ScreenDto { Name = screen.Name };
            BuildDtoChildren(screen.Body, screenDto.Children);
            root.Screens.Add(screenDto);
        }

        var file = new FileInfo(Directories.ToolsData.FullName);
        file.Directory?.Create();
        using var stream = file.Open(FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(stream, root, JsonConfig.Options);
    }

    private void BuildDtoChildren(List<Component> components, List<ComponentDto> dtos)
    {
        foreach (var component in components)
        {
            var dto = ToDto(component);
            BuildDtoChildren(component.Children, dto.Children);
            dtos.Add(dto);
        }
    }

    // ─── Component → DTO conversion ──────────────────────────────────────────

    /// <summary>Convert a runtime <see cref="Component"/> to its serialisable <see cref="ComponentDto"/>.</summary>
    private static ComponentDto ToDto(Component component) => component switch
    {
        Label c => Base(new LabelDto { Text = c.Text, Color = c.Color, Alignment = c.Alignment, MaxWidth = c.MaxWidth }, c),
        Button c => Base(new ButtonDto { TextureNum = c.TextureNum }, c),
        TextBox c => Base(new TextBoxDto { MaxCharacters = c.MaxCharacters, Width = c.Width, Password = c.Password }, c),
        Panel c => Base(new PanelDto { TextureNum = c.TextureNum }, c),
        CheckBox c => Base(new CheckBoxDto { Text = c.Text, Checked = c.Checked }, c),
        ProgressBar c => Base(new ProgressBarDto { SourceY = c.SourceY, Width = c.Width, Height = c.Height }, c),
        SlotGrid c => Base(new SlotGridDto { Columns = c.Columns, SlotSize = c.SlotSize, Padding = c.Padding, Rows = c.Rows }, c),
        Picture c => Base(new PictureDto { Width = c.Width, Height = c.Height }, c),
        _ => throw new InvalidOperationException($"Unknown component type: {component.GetType().Name}")
    };

    /// <summary>Populate the shared base fields of a DTO from its source component.</summary>
    private static T Base<T>(T dto, Component src) where T : ComponentDto
    {
        dto.Name = src.Name;
        dto.X = src.Position.X;
        dto.Y = src.Position.Y;
        dto.Visible = src.Visible;
        return dto;
    }
}
