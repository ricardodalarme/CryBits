using System.IO;
using System.Windows.Forms;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Editors.Forms;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using FrameworkWrite = CryBits.Client.Framework.Library.Write;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using Screen = CryBits.Client.Framework.Interfacily.Components.Screen;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.Library;

internal static class Write
{
    public static void Tools()
    {
        // Cria um sistema binário para a manipulação dos dados
        var file = new FileInfo(Directories.ToolsData.FullName);
        using var data = new BinaryWriter(file.OpenWrite());
        data.Write((short)EditorInterface.Tree.Nodes.Count);
        for (short n = 0; n < EditorInterface.Tree.Nodes.Count; n++)
        {
            data.Write(((Screen)EditorInterface.Tree.Nodes[n].Tag).Name);
            Tools(EditorInterface.Tree.Nodes[n], data);
        }
    }

    private static void Tools(TreeNode node, BinaryWriter data)
    {
        // Escreve a ferramenta
        data.Write((byte)node.Nodes.Count);
        for (byte i = 0; i < node.Nodes.Count; i++)
        {
            // Salva de acordo com a ferramenta
            var tool = (Component)node.Nodes[i].Tag;
            switch (tool)
            {
                case Button button:
                    data.Write((byte)ToolType.Button);
                    FrameworkWrite.Button(data, button);
                    break;
                case TextBox textBox:
                    data.Write((byte)ToolType.TextBox);
                    FrameworkWrite.TextBox(data, textBox);
                    break;
                case CheckBox checkBox:
                    data.Write((byte)ToolType.CheckBox);
                    FrameworkWrite.CheckBox(data, checkBox);
                    break;
                case Panel panel:
                    data.Write((byte)ToolType.Panel);
                    FrameworkWrite.Panel(data, panel);
                    break;
            }

            // Pula pra próxima ferramenta
            Tools(node.Nodes[i], data);
        }
    }
}