using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Client.Entities;
using CryBits.Client.UI;
using CryBits.Entities;
using CryBits.Enums;

namespace CryBits.Client.Library;

internal static class Read
{
    public static void Data()
    {
        // Carrega todos os dados
        Tools();
        Options();
    }

    private static void Options()
    {
        // Cria o arquivo se ele não existir
        if (!Directories.Options.Exists)
        {
            Write.Options();
            return;
        }

        // Carrega as configurações
        using var data = new BinaryReader(Directories.Options.OpenRead());
        Logic.Options.SaveUsername = data.ReadBoolean();
        Logic.Options.Username = data.ReadString();
        Logic.Options.Sounds = data.ReadBoolean();
        Logic.Options.Musics = data.ReadBoolean();
        Logic.Options.Chat = data.ReadBoolean();
        Logic.Options.Fps = data.ReadBoolean();
        Logic.Options.Latency = data.ReadBoolean();
        Logic.Options.Party = data.ReadBoolean();
        Logic.Options.Trade = data.ReadBoolean();
    }

    private static Buttons Button(BinaryReader data)
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

    private static TextBoxes TextBox(BinaryReader data)
    {
        // Lê os dados
        return new()
        {
            Name = data.ReadString(),
            Position = new Point(data.ReadInt32(), data.ReadInt32()),
            Visible = data.ReadBoolean(),
            Window = (Window)data.ReadByte(),
            Length = data.ReadInt16(),
            Width = data.ReadInt16(),
            Password = data.ReadBoolean()
        };
    }

    private static Panels Panel(BinaryReader data)
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

    private static CheckBoxes CheckBox(BinaryReader data)
    {
        // Lê os dados
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

    private static void Tools()
    {
        var file = new FileInfo(Directories.ToolsData.FullName);
        for (byte i = 0; i < (byte)Window.Count; i++) UI.Tools.AllOrder[i] = new List<Tools.OrderStructure>();

        // Lê todas as ferramentas
        using var data = new BinaryReader(file.OpenRead());
        for (byte n = 0; n < UI.Tools.AllOrder.Length; n++)
            Tools(null, ref UI.Tools.AllOrder[n], data);
    }

    private static void Tools(Tools.OrderStructure parent, ref List<Tools.OrderStructure> node, BinaryReader data)
    {
        // Lê todos os filhos
        var size = data.ReadByte();

        for (byte i = 0; i < size; i++)
        {
            // Lê a ferramenta
            var tempTool = new Tools.Structure();
            switch ((ToolType)data.ReadByte())
            {
                case ToolType.Button: tempTool = Button(data); Buttons.List.Add(tempTool.Name, (Buttons)tempTool); break;
                case ToolType.TextBox: tempTool = TextBox(data); TextBoxes.List.Add(tempTool.Name, (TextBoxes)tempTool); break;
                case ToolType.Panel: tempTool = Panel(data); Panels.List.Add(tempTool.Name, (Panels)tempTool); break;
                case ToolType.CheckBox: tempTool = CheckBox(data); CheckBoxes.List.Add(tempTool.Name, (CheckBoxes)tempTool); break;
            }

            // Adiciona à lista
            node.Add(new Tools.OrderStructure
            {
                Nodes = new List<Tools.OrderStructure>(),
                Parent = parent,
                Data = tempTool
            });

            // Pula pro próximo
            Tools(node[i], ref node[i].Nodes, data);
        }
    }

    public static void Map(Guid id)
    {
        var file = new FileInfo(Directories.MapsData.FullName + id + Directories.Format);

        // Lê os dados
        using (var stream = file.OpenRead())
            CryBits.Entities.Map.List.Add(id, (Map)new BinaryFormatter().Deserialize(stream));

        // Redimensiona as partículas do clima
        TempMap.Current.UpdateWeatherType();
        TempMap.Current.Data.Update();
    }
}