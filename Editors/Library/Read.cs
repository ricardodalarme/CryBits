using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using CryBits.Editors.Entities.Tools;
using CryBits.Editors.Media.Graphics;
using CryBits.Entities.Tile;
using CryBits.Enums;
using static CryBits.Editors.Logic.Options;
using Button = CryBits.Editors.Entities.Tools.Button;
using CheckBox = CryBits.Editors.Entities.Tools.CheckBox;
using Panel = CryBits.Editors.Entities.Tools.Panel;
using TextBox = CryBits.Editors.Entities.Tools.TextBox;

namespace CryBits.Editors.Library;

internal static class Read
{
    public static void Options()
    {
        // Cria o arquivo se ele não existir
        if (!Directories.Options.Exists)
        {
            Write.Options();
            return;
        }

        // Carrega as configurações
        using var data = new BinaryReader(Directories.Options.OpenRead());
        PreMapGrid = data.ReadBoolean();
        PreMapView = data.ReadBoolean();
        PreMapAudio = data.ReadBoolean();
        Username = data.ReadString();
    }

    public static void Tools()
    {
        var file = new FileInfo(Directories.Tools.FullName);

        // Limpa a árvore de ordem
        Tool.Tree = new TreeNode();
        for (byte i = 0; i < (byte)Window.Count; i++) Tool.Tree.Nodes.Add(((Window)i).ToString());

        // Cria o arquivo caso ele não existir
        if (!file.Exists)
        {
            Write.Tools();
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        using var data = new BinaryReader(file.OpenRead());
        for (byte n = 0; n < Tool.Tree.Nodes.Count; n++)
            Tools(Tool.Tree.Nodes[n], data);
    }

    public static void Tools(TreeNode node, BinaryReader data)
    {
        // Lê todos os filhos
        var size = data.ReadByte();
        for (byte i = 0; i < size; i++)
        {
            var temp = new Tool();
            var type = (ToolType)data.ReadByte();

            // Lê a ferramenta
            if (type == ToolType.Button) temp = Button(data);
            else if (type == ToolType.CheckBox) temp = CheckBox(data);
            else if (type == ToolType.Panel) temp = Panel(data);
            else if (type == ToolType.TextBox) temp = TextBox(data);

            // Adiciona o nó
            node.Nodes.Add("[" + type + "] " + temp.Name);
            node.LastNode.Tag = temp;

            // Pula pro próximo
            Tools(node.Nodes[i], data);
        }
    }

    private static Button Button(BinaryReader data)
    {
        // Lê os dados
        return new()
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            Window = (Window)data.ReadByte(),
            TextureNum = data.ReadByte()
        };
    }

    private static TextBox TextBox(BinaryReader data)
    {
        // Lê os dados
        return new()
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            Window = (Window)data.ReadByte(),
            MaxCharacters = data.ReadInt16(),
            Width = data.ReadInt16(),
            Password = data.ReadBoolean()
        };
    }

    private static Panel Panel(BinaryReader data)
    {
        // Carrega os dados
        return new()
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            Window = (Window)data.ReadByte(),
            TextureNum = data.ReadByte()
        };
    }

    private static CheckBox CheckBox(BinaryReader data)
    {
        // Carrega os dados
        return new()
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            Window = (Window)data.ReadByte(),
            Text = data.ReadString(),
            Checked = data.ReadBoolean()
        };
    }

    public static void Tiles()
    {
        // Lê os dados
        CryBits.Entities.Tile.Tile.List = new Tile[Textures.Tiles.Count];
        for (byte i = 1; i < CryBits.Entities.Tile.Tile.List.Length; i++) Tile(i);
    }

    private static void Tile(byte index)
    {
        var file = new FileInfo(Directories.Tiles.FullName + index + Directories.Format);

        // Evita erros
        if (!file.Exists)
        {
            var textureSize =  Textures.Tiles[index].ToSize();
            CryBits.Entities.Tile.Tile.List[index] = new Tile(textureSize);
            Write.Tile(index);
            return;
        }

        // Lê os dados
        using var stream = file.OpenRead();
        CryBits.Entities.Tile.Tile.List[index] = (Tile)new BinaryFormatter().Deserialize(stream);
    }
}