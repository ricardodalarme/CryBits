using Lidgren.Network;

partial class Send
{
    // Pacotes do cliente
    public enum Packets
    {
        Connect,
        Write_Server_Data,
        Write_Classes,
        Write_Maps,
        Write_NPCs,
        Write_Items,
        Write_Shops,
        Request_Server_Data,
        Request_Classes,
        Request_Map,
        Request_Maps,
        Request_NPCs,
        Request_Items,
        Request_Shops
    }

    private static void Packet(NetOutgoingMessage Data)
    {
        // Envia os dados ao servidor
        Socket.Device.SendMessage(Data, NetDeliveryMethod.ReliableOrdered);
    }

    public static void Connect()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Connect);
        Data.Write(Login.Objects.txtName.Text);
        Data.Write(Login.Objects.txtPassword.Text);
        Data.Write(true); // Acesso pelo editor
        Packet(Data);
    }

    public static void Request_Server_Data()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Server_Data);
        Packet(Data);
    }

    public static void Request_Classes()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Classes);
        Packet(Data);
    }

    public static void Request_Map(Lists.Structures.Map Map)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Map);
        Data.Write(Map.ID.ToString());
        Packet(Data);
    }

    public static void Request_Maps()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Maps);
        Packet(Data);
    }

    public static void Request_NPCs()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_NPCs);
        Packet(Data);
    }

    public static void Request_Items()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Items);
        Packet(Data);
    }

    public static void Request_Shops()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Shops);
        Packet(Data);
    }

    public static void Write_Server_Data()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Server_Data);
        Data.Write(Lists.Server_Data.Game_Name);
        Data.Write(Lists.Server_Data.Welcome);
        Data.Write(Lists.Server_Data.Port);
        Data.Write(Lists.Server_Data.Max_Players);
        Data.Write(Lists.Server_Data.Max_Characters);
        Data.Write(Lists.Server_Data.Max_Party_Members);
        Data.Write(Lists.Server_Data.Max_Map_Items);
        Data.Write(Lists.Server_Data.Num_Points);
        Packet(Data);
    }

    public static void Write_Classes()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Classes);
        Data.Write((byte)Lists.Class.Count);
        foreach (Lists.Structures.Class Class in Lists.Class.Values)
        {
            // Escreve os dados
            Data.Write(Class.ID.ToString());
            Data.Write((byte)Class.Tex_Male.Count);
            Data.Write((byte)Class.Tex_Female.Count);
            Data.Write((byte)Class.Item.Count);
            Data.Write(Class.Name);
            Data.Write(Class.Description);
            for (byte i = 0; i < Class.Tex_Male.Count; i++) Data.Write(Class.Tex_Male[i]);
            for (byte i = 0; i < Class.Tex_Female.Count; i++) Data.Write(Class.Tex_Female[i]);
            Data.Write(Lists.GetID(Class.Spawn_Map));
            Data.Write(Class.Spawn_Direction);
            Data.Write(Class.Spawn_X);
            Data.Write(Class.Spawn_Y);
            for (byte i = 0; i < (byte)Globals.Vitals.Count; i++) Data.Write(Class.Vital[i]);
            for (byte i = 0; i < (byte)Globals.Attributes.Count; i++) Data.Write(Class.Attribute[i]);
            for (byte i = 0; i < Class.Item.Count; i++)
            {
                Data.Write(Lists.GetID(Class.Item[i].Item1));
                Data.Write(Class.Item[i].Item2);
            }
        }
        Packet(Data);
    }

    public static void Write_Maps()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Maps);
        Data.Write((byte)Lists.Map.Count);
        foreach (Lists.Structures.Map Map in Lists.Map.Values)
        {
            // Escreve os dados
            Data.Write(Map.ID.ToString());
            Data.Write((short)(Map.Revision + 1));
            Data.Write(Map.Name);
            Data.Write(Map.Width);
            Data.Write(Map.Height);
            Data.Write(Map.Moral);
            Data.Write(Map.Panorama);
            Data.Write(Map.Music);
            Data.Write(Map.Color);
            Data.Write(Map.Weather.Type);
            Data.Write(Map.Weather.Intensity);
            Data.Write(Map.Fog.Texture);
            Data.Write(Map.Fog.Speed_X);
            Data.Write(Map.Fog.Speed_Y);
            Data.Write(Map.Fog.Alpha);
            Data.Write(Map.Light_Global);
            Data.Write(Map.Lighting);

            // Ligações
            for (short i = 0; i < (short)Globals.Directions.Count; i++)
                Data.Write(Lists.GetID(Map.Link[i]));

            // Camadas
            Data.Write((byte)(Map.Layer.Count - 1));
            for (byte i = 0; i < Map.Layer.Count; i++)
            {
                Data.Write(Map.Layer[i].Name);
                Data.Write(Map.Layer[i].Type);

                // Azulejos
                for (byte x = 0; x <= Map.Width; x++)
                    for (byte y = 0; y <= Map.Height; y++)
                    {
                        Data.Write(Map.Layer[i].Tile[x, y].X);
                        Data.Write(Map.Layer[i].Tile[x, y].Y);
                        Data.Write(Map.Layer[i].Tile[x, y].Tile);
                        Data.Write(Map.Layer[i].Tile[x, y].Auto);
                    }
            }


            // Dados específicos dos azulejos
            for (byte x = 0; x <= Map.Width; x++)
                for (byte y = 0; y <= Map.Height; y++)
                {
                    Data.Write(Map.Tile[x, y].Attribute);
                    Data.Write(Map.Tile[x, y].Data_1);
                    Data.Write(Map.Tile[x, y].Data_2);
                    Data.Write(Map.Tile[x, y].Data_3);
                    Data.Write(Map.Tile[x, y].Data_4);
                    Data.Write(Map.Tile[x, y].Data_5);
                    Data.Write(Map.Tile[x, y].Zone);

                    // Bloqueio direcional
                    for (byte i = 0; i < (byte)Globals.Directions.Count; i++)
                        Data.Write(Map.Tile[x, y].Block[i]);
                }

            // Luzes
            Data.Write((byte)Map.Light.Count);
            for (byte i = 0; i < Map.Light.Count; i++)
            {
                Data.Write(Map.Light[i].X);
                Data.Write(Map.Light[i].Y);
                Data.Write(Map.Light[i].Width);
                Data.Write(Map.Light[i].Height);
            }

            // NPCs
            Data.Write((byte)Map.NPC.Count);
            for (byte i = 0; i < Map.NPC.Count; i++)
            {
                Data.Write(Lists.GetID(Map.NPC[i].NPC));
                Data.Write(Map.NPC[i].Zone);
                Data.Write(Map.NPC[i].Spawn);
                Data.Write(Map.NPC[i].X);
                Data.Write(Map.NPC[i].Y);
            }
        }
        Packet(Data);
    }

    public static void Write_NPCs()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_NPCs);
        Data.Write((short)Lists.NPC.Count);
        foreach (Lists.Structures.NPC NPC in Lists.NPC.Values)
        {
            // Geral
            Data.Write(NPC.ID.ToString());
            Data.Write(NPC.Name);
            Data.Write(NPC.SayMsg);
            Data.Write(NPC.Texture);
            Data.Write(NPC.Behaviour);
            Data.Write(NPC.SpawnTime);
            Data.Write(NPC.Sight);
            Data.Write(NPC.Experience);
            for (byte i = 0; i < (byte)Globals.Vitals.Count; i++) Data.Write(NPC.Vital[i]);
            for (byte i = 0; i < (byte)Globals.Attributes.Count; i++) Data.Write(NPC.Attribute[i]);
            Data.Write((byte)NPC.Drop.Count);
            for (byte i = 0; i < NPC.Drop.Count; i++)
            {
                Data.Write(Lists.GetID(NPC.Drop[i].Item));
                Data.Write(NPC.Drop[i].Amount);
                Data.Write(NPC.Drop[i].Chance);
            }
            Data.Write(NPC.AttackNPC);
            Data.Write((byte)NPC.Allie.Count);
            for (byte i = 0; i < NPC.Allie.Count; i++) Data.Write(Lists.GetID(NPC.Allie[i]));
            Data.Write((byte)NPC.Movement);
            Data.Write(NPC.Flee_Helth);
            Data.Write(Lists.GetID(NPC.Shop));
        }
        Packet(Data);
    }

    public static void Write_Items()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Items);
        Data.Write((short)Lists.Item.Count);
        foreach (Lists.Structures.Item Item in Lists.Item.Values)
        {
            // Geral
            Data.Write(Item.ID.ToString());
            Data.Write(Item.Name);
            Data.Write(Item.Description);
            Data.Write(Item.Texture);
            Data.Write(Item.Type);
            Data.Write(Item.Stackable);
            Data.Write(Item.Bind);
            Data.Write(Item.Rarity);
            Data.Write(Item.Req_Level);
            Data.Write(Lists.GetID(Item.Req_Class));
            Data.Write(Item.Potion_Experience);
            for (byte i = 0; i < (byte)Globals.Vitals.Count; i++) Data.Write(Item.Potion_Vital[i]);
            Data.Write(Item.Equip_Type);
            for (byte i = 0; i < (byte)Globals.Attributes.Count; i++) Data.Write(Item.Equip_Attribute[i]);
            Data.Write(Item.Weapon_Damage);
        }
        Packet(Data);
    }

    public static void Write_Shops()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Shops);
        Data.Write((short)Lists.Shop.Count);
        foreach (Lists.Structures.Shop Shop in Lists.Shop.Values)
        {
            // Geral
            Data.Write(Shop.ID.ToString());
            Data.Write((byte)Shop.Sold.Count);
            Data.Write((byte)Shop.Bought.Count);
            Data.Write(Shop.Name);
            Data.Write(Lists.GetID(Shop.Currency));
            for (byte j = 0; j < Shop.Sold.Count; j++)
            {
                Data.Write(Lists.GetID(Shop.Sold[j].Item));
                Data.Write(Shop.Sold[j].Amount);
                Data.Write(Shop.Sold[j].Price);
            }
            for (byte j = 0; j < Shop.Bought.Count; j++)
            {
                Data.Write(Lists.GetID(Shop.Bought[j].Item));
                Data.Write(Shop.Bought[j].Amount);
                Data.Write(Shop.Bought[j].Price);
            }
        }

        Packet(Data);
    }
}