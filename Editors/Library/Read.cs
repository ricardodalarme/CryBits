using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Library;
using CryBits.Editors.Entities;

namespace CryBits.Editors.Library;

internal static class Read
{
    public static void Tools()
    {
        var file = new FileInfo(Directories.ToolsData.FullName);

        // Cria o arquivo caso ele não existir
        if (!file.Exists)
        {
            Write.Tools();
            return;
        }

        // Lê todas as ferramentas a partir de JSON
        var root = JsonSerializer.Deserialize<ToolsJsonRoot>(file.OpenRead(), JsonConfig.Options)
                   ?? new ToolsJsonRoot();

        foreach (var screenDto in root.Screens)
        {
            var screen = new Client.Framework.Interfacily.Components.Screen { Name = screenDto.Name };
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
}
