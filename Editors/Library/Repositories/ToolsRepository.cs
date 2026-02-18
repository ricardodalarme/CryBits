using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Library;
using CryBits.Editors.Entities;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using Screen = CryBits.Client.Framework.Interfacily.Components.Screen;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.Library.Repositories;

internal static class ToolsRepository
{
    // ─── Read ────────────────────────────────────────────────────────────────

    public static void Read()
    {
        var file = new FileInfo(Directories.ToolsData.FullName);

        // Cria o arquivo caso ele não existir
        if (!file.Exists)
        {
            Write();
            return;
        }

        // Lê todas as ferramentas a partir de JSON
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
                _ => throw new InvalidDataException($"Unknown component DTO: {dto.GetType().Name}")
            };

            var typeName = component.GetType().Name;
            var child = node.Nodes.Add($"[{typeName}] {component.Name}");
            child.Tag = component;
            body.Add(component);

            // Recurse
            LoadChildren(child, component.Children, dto.Children);
        }
    }

    // ─── Write ───────────────────────────────────────────────────────────────

    public static void Write()
    {
        // Constrói a árvore completa como DTO JSON
        var root = new ToolsJsonRoot();

        foreach (var screenNode in InterfaceData.Tree.Nodes)
        {
            var screenDto = new ScreenDto { Name = ((Screen)screenNode.Tag!).Name };
            BuildChildren(screenNode, screenDto.Children);
            root.Screens.Add(screenDto);
        }

        // Serializa para o arquivo
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
                Button button => ButtonDto(button),
                TextBox textBox => TextBoxDto(textBox),
                CheckBox checkBox => CheckBoxDto(checkBox),
                Panel panel => PanelDto(panel),
                _ => throw new InvalidOperationException($"Unknown component type: {component.GetType().Name}")
            };

            // Recurse into children
            BuildChildren(t, dto.Children);
            dtos.Add(dto);
        }
    }

    // Delegate to Client.Framework ToolsRepository for DTO conversions
    private static ButtonDto ButtonDto(Button tool) => Client.Framework.Library.Repositories.ToolsRepository.ButtonDto(tool);
    private static TextBoxDto TextBoxDto(TextBox tool) => Client.Framework.Library.Repositories.ToolsRepository.TextBoxDto(tool);
    private static PanelDto PanelDto(Panel tool) => Client.Framework.Library.Repositories.ToolsRepository.PanelDto(tool);
    private static CheckBoxDto CheckBoxDto(CheckBox tool) => Client.Framework.Library.Repositories.ToolsRepository.CheckBoxDto(tool);
}
