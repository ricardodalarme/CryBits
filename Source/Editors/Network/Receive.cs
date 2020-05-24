using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

partial class Receive
{
    // Pacotes do servidor
    private enum Packets
    {
        Alert,
        Connect,
        Server_Data,
        Classes,
        Maps,
        Map,
        NPCs,
        Items,
        Shops
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
            case Packets.Map: Map(Data); break;
            case Packets.NPCs: NPCs(Data); break;
            case Packets.Items: Items(Data); break;
            case Packets.Shops: Shops(Data); break;
        }
    }

    private static void Alert(NetIncomingMessage Data)
    {
        // Mostra a mensagem
        MessageBox.Show(Data.ReadString());
    }

    private static void Connect()
    {
        // Abre a janela principal
        Login.Form.Visible = false;
        Editor_Maps.Form = new Editor_Maps();
    }

    private static void Server_Data(NetIncomingMessage Data)
    {
        // Lê os dados
        Lists.Server_Data.Game_Name = Data.ReadString();
        Lists.Server_Data.Welcome = Data.ReadString();
        Lists.Server_Data.Port = Data.ReadInt16();
        Lists.Server_Data.Max_Players = Data.ReadByte();
        Lists.Server_Data.Max_Characters = Data.ReadByte();
        Lists.Server_Data.Max_Party_Members = Data.ReadByte();
        Lists.Server_Data.Max_Map_Items = Data.ReadByte();
        Lists.Server_Data.Num_Points = Data.ReadByte();
        Lists.Server_Data.Min_Name_Length = Data.ReadByte();
        Lists.Server_Data.Max_Name_Length = Data.ReadByte();
        Lists.Server_Data.Min_Password_Length = Data.ReadByte();
        Lists.Server_Data.Max_Password_Length = Data.ReadByte();
    }

    private static void Classes(NetIncomingMessage Data)
    {
        // Classes a serem removidas
        Dictionary<Guid, Lists.Structures.Class> ToRemove = new Dictionary<Guid, Lists.Structures.Class>(Lists.Class);

        // Quantidade de classes
        short Count = Data.ReadByte();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Lists.Structures.Class Class;

            // Obtém o dado
            if (Lists.Class.ContainsKey(ID))
            {
                Class = Lists.Class[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Class = new Lists.Structures.Class(ID);
                Lists.Class.Add(Class.ID, Class);
            }

            // Reseta os valores necessários
            Class.Tex_Male.Clear();
            Class.Tex_Female.Clear();

            // Lê os dados
            Class.Name = Data.ReadString();
            Class.Description = Data.ReadString();
            for (byte t = 0, Size = Data.ReadByte(); t < Size; t++) Class.Tex_Male.Add(Data.ReadInt16());
            for (byte t = 0, Size = Data.ReadByte(); t < Size; t++) Class.Tex_Female.Add(Data.ReadInt16());
            Class.Spawn_Map = (Lists.Structures.Map)Lists.GetData(Lists.Map, new Guid(Data.ReadString()));
            Class.Spawn_Direction = Data.ReadByte();
            Class.Spawn_X = Data.ReadByte();
            Class.Spawn_Y = Data.ReadByte();
            for (byte v = 0; v < (byte)Globals.Vitals.Count; v++) Class.Vital[v] = Data.ReadInt16();
            for (byte a = 0; a < (byte)Globals.Attributes.Count; a++) Class.Attribute[a] = Data.ReadInt16();
            byte Num_Items = Data.ReadByte();
            for (byte a = 0; a < Num_Items; a++) Class.Item.Add(new Lists.Structures.Inventory((Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())), Data.ReadInt16()));
        }

        // Remove as classes que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys) Lists.Class.Remove(Remove);
    }

    private static void Map(NetIncomingMessage Data)
    {
        Guid ID = new Guid(Data.ReadString());
        Lists.Structures.Map Map;

        // Obtém o dado
        if (Lists.Map.ContainsKey(ID)) Map = Lists.Map[ID];
        else
        {
            Map = new Lists.Structures.Map(ID);
            Lists.Map.Add(ID, Map);
        }

        // Dados básicos
        Map.Revision = Data.ReadInt16();
        Map.Name = Data.ReadString();
        Map.Width = Data.ReadByte();
        Map.Height = Data.ReadByte();
        Map.Moral = Data.ReadByte();
        Map.Panorama = Data.ReadByte();
        Map.Music = Data.ReadByte();
        Map.Color = Data.ReadInt32();
        Map.Weather.Type = Data.ReadByte();
        Map.Weather.Intensity = Data.ReadByte();
        Map.Fog.Texture = Data.ReadByte();
        Map.Fog.Speed_X = Data.ReadSByte();
        Map.Fog.Speed_Y = Data.ReadSByte();
        Map.Fog.Alpha = Data.ReadByte();
        Map.Lighting = Data.ReadByte();

        // Ligações
        for (short n = 0; n < (short)Globals.Directions.Count; n++)
            Map.Link[n] = (Lists.Structures.Map)Lists.GetData(Lists.Map, new Guid(Data.ReadString()));

        // Quantidade de camadas
        byte Num_Layers = Data.ReadByte();

        // Camadas
        for (byte n = 0; n < Num_Layers; n++)
        {
            // Dados básicos
            Map.Layer.Add(new Lists.Structures.Map_Layer());
            Map.Layer[n].Name = Data.ReadString();
            Map.Layer[n].Type = Data.ReadByte();

            // Redimensiona os azulejos
            Map.Layer[n].Tile = new Lists.Structures.Map_Tile_Data[Map.Width, Map.Height];

            // Azulejos
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                {
                    Map.Layer[n].Tile[x, y] = new Lists.Structures.Map_Tile_Data();
                    Map.Layer[n].Tile[x, y].X = Data.ReadByte();
                    Map.Layer[n].Tile[x, y].Y = Data.ReadByte();
                    Map.Layer[n].Tile[x, y].Tile = Data.ReadByte();
                    Map.Layer[n].Tile[x, y].Auto = Data.ReadBoolean();
                }
        }

        // Dados específicos dos azulejos
        Map.Tile = new Lists.Structures.Map_Tile[Map.Width, Map.Height];
        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
            {
                Map.Tile[x, y] = new Lists.Structures.Map_Tile();
                Map.Tile[x, y].Attribute = Data.ReadByte();
                Map.Tile[x, y].Data_1 = Data.ReadString();
                Map.Tile[x, y].Data_2 = Data.ReadInt16();
                Map.Tile[x, y].Data_3 = Data.ReadInt16();
                Map.Tile[x, y].Data_4 = Data.ReadInt16();
                Map.Tile[x, y].Zone = Data.ReadByte();

                for (byte n = 0; n < (byte)Globals.Directions.Count; n++)
                    Map.Tile[x, y].Block[n] = Data.ReadBoolean();
            }

        // Luzes
        byte Num_Lights = Data.ReadByte();
        Map.Light = new List<Lists.Structures.Map_Light>();
        if (Num_Lights > 0)
            for (byte n = 0; n < Num_Lights; n++)
                Map.Light.Add(new Lists.Structures.Map_Light(new Rectangle(Data.ReadByte(), Data.ReadByte(), Data.ReadByte(), Data.ReadByte())));

        // NPCs
        byte Num_NPCs = Data.ReadByte();
        Lists.Structures.Map_NPC NPC = new Lists.Structures.Map_NPC();
        if (Num_NPCs > 0)
            for (byte n = 0; n < Num_NPCs; n++)
            {
                NPC.NPC = (Lists.Structures.NPC)Lists.GetData(Lists.NPC, new Guid(Data.ReadString()));
                NPC.Zone = Data.ReadByte();
                NPC.Spawn = Data.ReadBoolean();
                NPC.X = Data.ReadByte();
                NPC.Y = Data.ReadByte();
                Map.NPC.Add(NPC);
            }
    }

    private static void NPCs(NetIncomingMessage Data)
    {
        // NPCs a serem removidas
        Dictionary<Guid, Lists.Structures.NPC> ToRemove = new Dictionary<Guid, Lists.Structures.NPC>(Lists.NPC);

        // Quantidade de NPCs
        short Count = Data.ReadInt16();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Lists.Structures.NPC NPC;

            // Obtém o dado
            if (Lists.NPC.ContainsKey(ID))
            {
                NPC = Lists.NPC[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                NPC = new Lists.Structures.NPC(ID);
                Lists.NPC.Add(NPC.ID, NPC);
            }

            // Lê os dados
            NPC.Name = Data.ReadString();
            NPC.SayMsg = Data.ReadString();
            NPC.Texture = Data.ReadInt16();
            NPC.Behaviour = Data.ReadByte();
            for (byte n = 0; n < (byte)Globals.Vitals.Count; n++) NPC.Vital[n] = Data.ReadInt16();
            NPC.SpawnTime = Data.ReadByte();
            NPC.Sight = Data.ReadByte();
            NPC.Experience = Data.ReadInt32();
            for (byte n = 0; n < (byte)Globals.Attributes.Count; n++) NPC.Attribute[n] = Data.ReadInt16();
            for (byte n = 0, Size = Data.ReadByte(); n < Size; n++) NPC.Drop.Add(new Lists.Structures.NPC_Drop((Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())), Data.ReadInt16(), Data.ReadByte()));
            NPC.AttackNPC = Data.ReadBoolean();
            for (byte n = 0, Size = Data.ReadByte(); n < Size; n++) NPC.Allie.Add((Lists.Structures.NPC)Lists.GetData(Lists.NPC, new Guid(Data.ReadString())));
            NPC.Movement = (Globals.NPC_Movements)Data.ReadByte();
            NPC.Flee_Helth = Data.ReadByte();
            NPC.Shop = (Lists.Structures.Shop)Lists.GetData(Lists.Shop, new Guid(Data.ReadString()));
        }

        // Remove os NPCs que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys) Lists.NPC.Remove(Remove);
    }

    private static void Items(NetIncomingMessage Data)
    {
        // Itens a serem removidas
        Dictionary<Guid, Lists.Structures.Item> ToRemove = new Dictionary<Guid, Lists.Structures.Item>(Lists.Item);

        // Quantidade de itens
        short Count = Data.ReadInt16();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Lists.Structures.Item Item;

            // Obtém o dado
            if (Lists.Item.ContainsKey(ID))
            {
                Item = Lists.Item[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Item = new Lists.Structures.Item(ID);
                Lists.Item.Add(Item.ID, Item);
            }

            // Lê os dados
            Item.Name = Data.ReadString();
            Item.Description = Data.ReadString();
            Item.Texture = Data.ReadInt16();
            Item.Type = Data.ReadByte();
            Item.Stackable = Data.ReadBoolean();
            Item.Bind = Data.ReadByte();
            Item.Rarity = Data.ReadByte();
            Item.Req_Level = Data.ReadInt16();
            Item.Req_Class = (Lists.Structures.Class)Lists.GetData(Lists.Class, new Guid(Data.ReadString()));
            Item.Potion_Experience = Data.ReadInt32();
            for (byte v = 0; v < (byte)Globals.Vitals.Count; v++) Item.Potion_Vital[v] = Data.ReadInt16();
            Item.Equip_Type = Data.ReadByte();
            for (byte a = 0; a < (byte)Globals.Attributes.Count; a++) Item.Equip_Attribute[a] = Data.ReadInt16();
            Item.Weapon_Damage = Data.ReadInt16();
        }

        // Remove os itens que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys) Lists.Item.Remove(Remove);
    }

    private static void Shops(NetIncomingMessage Data)
    {
        // Lojas a serem removidas
        Dictionary<Guid, Lists.Structures.Shop> ToRemove = new Dictionary<Guid, Lists.Structures.Shop>(Lists.Shop);

        // Quantidade de lojas
        short Count = Data.ReadInt16();

        while (--Count >= 0)
        {
            Guid ID = new Guid(Data.ReadString());
            Lists.Structures.Shop Shop;

            // Obtém o dado
            if (Lists.Shop.ContainsKey(ID))
            {
                Shop = Lists.Shop[ID];
                ToRemove.Remove(ID);
            }
            else
            {
                Shop = new Lists.Structures.Shop(ID);
                Lists.Shop.Add(Shop.ID, Shop);
            }

            // Reseta os valores necessários
            Shop.Sold.Clear();
            Shop.Bought.Clear();

            // Lê os dados
            Shop.Name = Data.ReadString();
            Shop.Currency = (Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
            for (byte j = 0, Size = Data.ReadByte(); j < Size; j++) Shop.Sold.Add(new Lists.Structures.Shop_Item((Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())), Data.ReadInt16(), Data.ReadInt16()));
            for (byte j = 0, Size = Data.ReadByte(); j < Size; j++) Shop.Bought.Add(new Lists.Structures.Shop_Item((Lists.Structures.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString())), Data.ReadInt16(), Data.ReadInt16()));
        }

        // Remove as lojas que não tiveram os dados atualizados
        foreach (Guid Remove in ToRemove.Keys) Lists.Shop.Remove(Remove);
    }
}