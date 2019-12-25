using Lidgren.Network;
using System.Drawing;
using System.Windows.Forms;

partial class Receive
{
    // Pacotes do servidor
    public enum Packets
    {
        Alert,
        Connect,
        Server_Data,
        Classes,
        Tiles,
        Maps,
        NPCs,
        Items
    }

    public static void Handle(NetIncomingMessage Data)
    {
        // Manuseia os dados recebidos
        switch ((Packets)Data.ReadByte())
        {
            case Packets.Alert: Alert(Data); break;
            case Packets.Connect: Connect(); break;
            case Packets.Server_Data: Server_Data(Data); break;
            case Packets.Classes: Classes(Data); break;
            case Packets.Maps: Maps(Data); break;
            case Packets.NPCs: NPCs(Data); break;
            case Packets.Items: Items(Data); break;
        }
    }

    private static void Alert(NetIncomingMessage Data)
    {
        // Mostra a mensagem
        MessageBox.Show(Data.ReadString());
    }

    private static void Connect()
    {
        // Abre a janela de edição
        Selection.Objects.Visible = true;
    }

    public static void Server_Data(NetIncomingMessage Data)
    {
        // Lê os dados
        Lists.Server_Data.GameName = Data.ReadString();
        Lists.Server_Data.Welcome = Data.ReadString();
        Lists.Server_Data.Port = Data.ReadInt16();
        Lists.Server_Data.Max_Players = Data.ReadByte();
        Lists.Server_Data.Max_Characters = Data.ReadByte();
        Lists.Server_Data.Num_Classes = Data.ReadByte();
        Lists.Server_Data.Num_Tiles = Data.ReadByte();
        Lists.Server_Data.Num_Maps = Data.ReadInt16();
        Lists.Server_Data.Num_NPCs = Data.ReadInt16();
        Lists.Server_Data.Num_Items = Data.ReadInt16();
    }

    public static void Classes(NetIncomingMessage Data)
    {
        // Quantidade
        Lists.Class = new Lists.Structures.Class[Data.ReadByte() + 1];

        for (short i = 1; i <= Lists.Class.GetUpperBound(0); i++)
        {
            // Redimensiona os valores necessários 
            Lists.Class[i].Vital = new short[(byte)Globals.Vitals.Amount];
            Lists.Class[i].Attribute = new short[(byte)Globals.Attributes.Amount];

            // Lê os dados
            Lists.Class[i].Name = Data.ReadString();
            Lists.Class[i].Texture_Male = Data.ReadInt16();
            Lists.Class[i].Texture_Female = Data.ReadInt16();
            Lists.Class[i].Spawn_Map = Data.ReadInt16();
            Lists.Class[i].Spawn_Direction = Data.ReadByte();
            Lists.Class[i].Spawn_X = Data.ReadByte();
            Lists.Class[i].Spawn_Y = Data.ReadByte();
            for (byte v = 0; v < (byte)Globals.Vitals.Amount; v++) Lists.Class[i].Vital[v] = Data.ReadInt16();
            for (byte a = 0; a < (byte)Globals.Attributes.Amount; a++) Lists.Class[i].Attribute[a] = Data.ReadInt16();
        }

        // Abre o editor
        if (Data.ReadBoolean()) Editor_Classes.Open();
    }

    public static void Maps(NetIncomingMessage Data)
    {
        // Lê os dados
        short Index = Data.ReadInt16();
        Lists.Map[Index].Revision = Data.ReadInt16();
        Lists.Map[Index].Name = Data.ReadString();
        Lists.Map[Index].Width = Data.ReadByte();
        Lists.Map[Index].Height = Data.ReadByte();
        Lists.Map[Index].Moral = Data.ReadByte();
        Lists.Map[Index].Panorama = Data.ReadByte();
        Lists.Map[Index].Music = Data.ReadByte();
        Lists.Map[Index].Color = Data.ReadInt32();
        Lists.Map[Index].Weather.Type = Data.ReadByte();
        Lists.Map[Index].Weather.Intensity = Data.ReadByte();
        Lists.Map[Index].Fog.Texture = Data.ReadByte();
        Lists.Map[Index].Fog.Speed_X = Data.ReadSByte();
        Lists.Map[Index].Fog.Speed_Y = Data.ReadSByte();
        Lists.Map[Index].Fog.Alpha = Data.ReadByte();
        Lists.Map[Index].Light_Global = Data.ReadByte();
        Lists.Map[Index].Lighting = Data.ReadByte();

        // Ligações
        for (short i = 0; i <= (short)Globals.Directions.Amount - 1; i++)
            Lists.Map[Index].Link[i] = Data.ReadInt16();

        // Quantidade de camadas
        byte Num_Layers = Data.ReadByte();
        Lists.Map[Index].Layer = new System.Collections.Generic.List<Lists.Structures.Map_Layer>();

        // Camadas
        for (byte i = 0; i <= Num_Layers; i++)
        {
            // Dados básicos
            Lists.Map[Index].Layer.Add(new Lists.Structures.Map_Layer());
            Lists.Map[Index].Layer[i].Name = Data.ReadString();
            Lists.Map[Index].Layer[i].Type = Data.ReadByte();

            // Redimensiona os azulejos
            Lists.Map[Index].Layer[i].Tile = new Lists.Structures.Map_Tile_Data[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];

            // Azulejos
            for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                {
                    Lists.Map[Index].Layer[i].Tile[x, y].x = Data.ReadByte();
                    Lists.Map[Index].Layer[i].Tile[x, y].y = Data.ReadByte();
                    Lists.Map[Index].Layer[i].Tile[x, y].Tile = Data.ReadByte();
                    Lists.Map[Index].Layer[i].Tile[x, y].Auto = Data.ReadBoolean();
                    Lists.Map[Index].Layer[i].Tile[x, y].Mini = new Point[4];
                }
        }

        // Dados específicos dos azulejos
        Lists.Map[Index].Tile = new Lists.Structures.Map_Tile[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
            {
                Lists.Map[Index].Tile[x, y].Attribute = Data.ReadByte();
                Lists.Map[Index].Tile[x, y].Data_1 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_2 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_3 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_4 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Zone = Data.ReadByte();
                Lists.Map[Index].Tile[x, y].Block = new bool[(byte)Globals.Directions.Amount];

                for (byte i = 0; i <= (byte)Globals.Directions.Amount - 1; i++)
                    Lists.Map[Index].Tile[x, y].Block[i] = Data.ReadBoolean();
            }

        // Luzes
        byte Num_Lights = Data.ReadByte();
        Lists.Map[Index].Light = new System.Collections.Generic.List<Lists.Structures.Map_Light>();
        if (Num_Lights > 0)
            for (byte i = 0; i <= Num_Lights - 1; i++)
                Lists.Map[Index].Light.Add(new Lists.Structures.Map_Light(new Rectangle(Data.ReadByte(), Data.ReadByte(), Data.ReadByte(), Data.ReadByte())));

        // NPCs
        byte Num_NPCs = Data.ReadByte();
        Lists.Map[Index].NPC = new System.Collections.Generic.List<Lists.Structures.Map_NPC>();
        Lists.Structures.Map_NPC NPC = new Lists.Structures.Map_NPC();
        if (Num_NPCs > 0)
            for (byte i = 0; i <= Num_NPCs - 1; i++)
            {
                NPC.Index = Data.ReadInt16();
                NPC.Zone = Data.ReadByte();
                NPC.Spawn = Data.ReadBoolean();
                NPC.X = Data.ReadByte();
                NPC.Y = Data.ReadByte();
                Lists.Map[Index].NPC.Add(NPC);
            }
    }

    public static void NPCs(NetIncomingMessage Data)
    {

        // Abre o editor
        if (Data.ReadBoolean()) Editor_NPCs.Open();
    }

    public static void Items(NetIncomingMessage Data)
    {
        // Quantidade de itens
        Lists.Item = new Lists.Structures.Item[Data.ReadInt16() + 1];

        for (short i = 1; i <= Lists.Item.GetUpperBound(0); i++)
        {
            // Redimensiona os valores necessários 
            Lists.Item[i].Potion_Vital = new short[(byte)Globals.Vitals.Amount];
            Lists.Item[i].Equip_Attribute = new short[(byte)Globals.Attributes.Amount];

            // Lê os dados
            Lists.Item[i].Name = Data.ReadString();
            Lists.Item[i].Description = Data.ReadString();
            Lists.Item[i].Texture = Data.ReadInt16();
            Lists.Item[i].Type = Data.ReadByte();
            Lists.Item[i].Price = Data.ReadInt16();
            Lists.Item[i].Stackable = Data.ReadBoolean();
            Lists.Item[i].Bind = Data.ReadBoolean();
            Lists.Item[i].Req_Level = Data.ReadInt16();
            Lists.Item[i].Req_Class = Data.ReadByte();
            Lists.Item[i].Potion_Experience = Data.ReadInt16();
            for (byte v = 0; v < (byte)Globals.Vitals.Amount; v++) Lists.Item[i].Potion_Vital[v] = Data.ReadInt16();
            Lists.Item[i].Equip_Type = Data.ReadByte();
            for (byte a = 0; a < (byte)Globals.Attributes.Amount; a++) Lists.Item[i].Equip_Attribute[a] = Data.ReadInt16();
            Lists.Item[i].Weapon_Damage = Data.ReadInt16();
        }

        // Abre o editor
        if (Data.ReadBoolean()) Editor_Items.Open();
    }
}