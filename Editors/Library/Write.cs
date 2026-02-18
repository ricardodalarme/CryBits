using System;
using System.Collections.Generic;
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

namespace CryBits.Editors.Library;

internal static class Write
{
    public static void Tools()
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
                Button button => CryBits.Client.Framework.Library.Write.ButtonDto(button),
                TextBox textBox => CryBits.Client.Framework.Library.Write.TextBoxDto(textBox),
                CheckBox checkBox => CryBits.Client.Framework.Library.Write.CheckBoxDto(checkBox),
                Panel panel => CryBits.Client.Framework.Library.Write.PanelDto(panel),
                _ => throw new InvalidOperationException($"Unknown component type: {component.GetType().Name}")
            };

            // Recurse into children
            BuildChildren(t, dto.Children);
            dtos.Add(dto);
        }
    }
}