using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Persistence;
using CryBits.Editors.Entities;

namespace CryBits.Editors.Library.Repositories;

internal static class ToolsRepository
{
    public static void Read()
    {
        var file = new FileInfo(Directories.ToolsData.FullName);

        // Create the file if it doesn't exist.
        if (!file.Exists)
        {
            Write();
            return;
        }

        // Read tools from JSON.
        var root = JsonSerializer.Deserialize<ToolsJsonRoot>(file.OpenRead(), JsonConfig.Options)
                   ?? new ToolsJsonRoot();

        foreach (var screenDto in root.Screens)
        {
            var screen = new Screen { Name = screenDto.Name };
            var node = InterfaceData.Tree.Nodes.Add("[Window] " + screen.Name);
            node.Tag = screen;
            LoadChildren(node, screen.Body, screenDto.Children);
        }
    }

    private static void LoadChildren(InterfaceNode node, List<Component> body, List<ComponentDto> dtos)
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
                _ => throw new InvalidDataException($"Unknown component DTO: {dto.GetType().Name}")
            };

            var typeName = component.GetType().Name;
            var child = node.Nodes.Add($"[{typeName}] {component.Name}");
            child.Tag = component;
            body.Add(component);

            LoadChildren(child, component.Children, dto.Children);
        }
    }


    public static void Write()
    {
        // Build DTO tree for serialization.
        var root = new ToolsJsonRoot();

        foreach (var screenNode in InterfaceData.Tree.Nodes)
        {
            var screenDto = new ScreenDto { Name = ((Screen)screenNode.Tag!).Name };
            BuildChildren(screenNode, screenDto.Children);
            root.Screens.Add(screenDto);
        }

        // Serialize to file.
        var file = new FileInfo(Directories.ToolsData.FullName);
        file.Directory?.Create();
        using var stream = file.Open(FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(stream, root, JsonConfig.Options);
    }

    private static void BuildChildren(InterfaceNode node, List<ComponentDto> dtos)
    {
        foreach (var t in node.Nodes)
        {
            var component = (Component)t.Tag!;

            ComponentDto dto = component switch
            {
                Label label => LabelDto(label),
                Button button => ButtonDto(button),
                TextBox textBox => TextBoxDto(textBox),
                CheckBox checkBox => CheckBoxDto(checkBox),
                Panel panel => PanelDto(panel),
                ProgressBar bar => ProgressBarDto(bar),
                _ => throw new InvalidOperationException($"Unknown component type: {component.GetType().Name}")
            };

            BuildChildren(t, dto.Children);
            dtos.Add(dto);
        }
    }

    // Delegate to Client.Framework ToolsRepository for DTO conversions
    private static LabelDto LabelDto(Label tool) =>
        Client.Framework.Persistence.Repositories.ToolsRepository.LabelDto(tool);

    private static ButtonDto ButtonDto(Button tool) =>
        Client.Framework.Persistence.Repositories.ToolsRepository.ButtonDto(tool);

    private static TextBoxDto TextBoxDto(TextBox tool) =>
        Client.Framework.Persistence.Repositories.ToolsRepository.TextBoxDto(tool);

    private static PanelDto PanelDto(Panel tool) =>
        Client.Framework.Persistence.Repositories.ToolsRepository.PanelDto(tool);

    private static CheckBoxDto CheckBoxDto(CheckBox tool) =>
        Client.Framework.Persistence.Repositories.ToolsRepository.CheckBoxDto(tool);

    private static ProgressBarDto ProgressBarDto(ProgressBar tool) =>
        Client.Framework.Persistence.Repositories.ToolsRepository.ProgressBarDto(tool);
}
