using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Entities.Map;

namespace CryBits.Client.Framework.Library;

public static class Read
{

    public static void Tools()
    {
        var file = new FileInfo(Directories.ToolsData.FullName);

        // Lê todas as ferramentas
        using var data = new BinaryReader(file.OpenRead());

        var windowsCount = data.ReadInt16();
        for (byte n = 0; n < windowsCount; n++)
        {
            var temp = new Screen { Name = data.ReadString() };
            Screens.List.Add(temp.Name, temp);
            Tools(null, Screens.List[temp.Name].Body, data);
        }
    }

    private static void Tools(Component? parent, List<Component> node, BinaryReader data)
    {
        // Lê todos os filhos
        var size = data.ReadByte();

        for (byte i = 0; i < size; i++)
        {
            // Lê a ferramenta
            Component tempTool;
            switch ((ToolType)data.ReadByte())
            {
                case ToolType.Button: tempTool = Button(data); Buttons.List.Add(tempTool.Name, (Button)tempTool); break;
                case ToolType.TextBox: tempTool = TextBox(data); TextBoxes.List.Add(tempTool.Name, (TextBox)tempTool); break;
                case ToolType.Panel: tempTool = Panel(data); Panels.List.Add(tempTool.Name, (Panel)tempTool); break;
                case ToolType.CheckBox: tempTool = CheckBox(data); CheckBoxes.List.Add(tempTool.Name, (CheckBox)tempTool); break;
                default: return;
            }

            tempTool.Parent = parent;
            node.Add(tempTool);

            // Pula pro próximo
            Tools(node[i], node[i].Children, data);
        }
    }

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
        Framework.Options.SaveUsername = data.ReadBoolean();
        Framework.Options.Username = data.ReadString();
        Framework.Options.Sounds = data.ReadBoolean();
        Framework.Options.Musics = data.ReadBoolean();
        Framework.Options.Chat = data.ReadBoolean();
        Framework.Options.Fps = data.ReadBoolean();
        Framework.Options.Latency = data.ReadBoolean();
        Framework.Options.Party = data.ReadBoolean();
        Framework.Options.Trade = data.ReadBoolean();
        Framework.Options.PreMapGrid = data.ReadBoolean();
        Framework.Options.PreMapView = data.ReadBoolean();
        Framework.Options.PreMapAudio = data.ReadBoolean();
    }

    public static Button Button(BinaryReader data)
    {
        // Lê os dados
        return new Button
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            TextureNum = data.ReadByte()
        };
    }

    public static TextBox TextBox(BinaryReader data)
    {
        // Lê os dados
        return new TextBox
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            MaxCharacters = data.ReadInt16(),
            Width = data.ReadInt16(),
            Password = data.ReadBoolean()
        };
    }

    public static Panel Panel(BinaryReader data)
    {
        // Carrega os dados
        return new Panel
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            TextureNum = data.ReadByte()
        };
    }

    public static CheckBox CheckBox(BinaryReader data)
    {
        // Carrega os dados
        return new CheckBox
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            Text = data.ReadString(),
            Checked = data.ReadBoolean()
        };
    }

    public static void Tiles()
    {
        // Lê os dados
        Entities.Tile.Tile.List = new Tile[Textures.Tiles.Count];
        for (byte i = 1; i < Entities.Tile.Tile.List.Length; i++) Tile(i);
    }

    private static void Tile(byte index)
    {
        var file = new FileInfo(Directories.Tiles.FullName + index + Directories.Format);

        // Evita erros
        if (!file.Exists)
        {
            var textureSize = Textures.Tiles[index].ToSize();
            Entities.Tile.Tile.List[index] = new Tile(textureSize);
            Write.Tile(index);
            return;
        }

        // Lê os dados
        using var stream = file.OpenRead();
        Entities.Tile.Tile.List[index] = (Tile)new BinaryFormatter().Deserialize(stream);
    }

    public static void Map(Guid id)
    {
        var file = new FileInfo(Directories.MapsData.FullName + id + Directories.Format);

        // Lê os dados
        using var stream = file.OpenRead();
        CryBits.Entities.Map.Map.List.Add(id, (Map)new BinaryFormatter().Deserialize(stream));
    }
}