using System.IO;
using System.Windows.Forms;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Editors.Forms;
using FrameworkRead = CryBits.Client.Framework.Library.Read;

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

        // Cria um sistema binário para a manipulação dos dados
        using var data = new BinaryReader(file.OpenRead());

        var windowsCount = data.ReadInt16();
        for (byte n = 0; n < windowsCount; n++)
        {
            var temp = new Client.Framework.Interfacily.Components.Screen { Name = data.ReadString() };
            EditorInterface.Tree.Nodes.Add("[Window] " + temp.Name);
            EditorInterface.Tree.LastNode.Tag = temp;
            Tools(EditorInterface.Tree.Nodes[n], data);
        }
    }

    public static void Tools(TreeNode node, BinaryReader data)
    {
        // Lê todos os filhos
        var size = data.ReadByte();
        for (byte i = 0; i < size; i++)
        {
            Component temp;
            var type = (ToolType)data.ReadByte();

            // Lê a ferramenta
            switch (type)
            {
                case ToolType.Button: temp = FrameworkRead.Button(data); break;
                case ToolType.CheckBox: temp = FrameworkRead.CheckBox(data); break;
                case ToolType.Panel: temp = FrameworkRead.Panel(data); break;
                case ToolType.TextBox: temp = FrameworkRead.TextBox(data); break;
                default: return;
            }

            // Adiciona o nó
            node.Nodes.Add("[" + type + "] " + temp.Name);
            node.LastNode.Tag = temp;

            // Pula pro próximo
            Tools(node.Nodes[i], data);
        }
    }
}