using CryBits.Editors.Forms;
using CryBits.Editors.Entities;
using CryBits;
using Lidgren.Network;
using static CryBits.Editors.Logic.Utils;

namespace CryBits.Editors.Network
{
    class Send
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
            Data.Write(Login.Form.txtUsername.Text);
            Data.Write(Login.Form.txtPassword.Text);
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

        public static void Request_Map(Map Map)
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
            Data.Write(Lists.Server_Data.Min_Name_Length);
            Data.Write(Lists.Server_Data.Max_Name_Length);
            Data.Write(Lists.Server_Data.Min_Password_Length);
            Data.Write(Lists.Server_Data.Max_Password_Length);
            Packet(Data);
        }

        public static void Write_Classes()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Write_Classes);
            Data.Write((byte)Class.List.Count);
            foreach (Class Class in Class.List.Values)
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
                Data.Write(Class.Spawn_Map.GetID());
                Data.Write(Class.Spawn_Direction);
                Data.Write(Class.Spawn_X);
                Data.Write(Class.Spawn_Y);
                for (byte i = 0; i < (byte)Vitals.Count; i++) Data.Write(Class.Vital[i]);
                for (byte i = 0; i < (byte)Attributes.Count; i++) Data.Write(Class.Attribute[i]);
                for (byte i = 0; i < Class.Item.Count; i++)
                {
                    Data.Write(Class.Item[i].Item.GetID());
                    Data.Write(Class.Item[i].Amount);
                }
            }
            Packet(Data);
        }

        public static void Write_Maps()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Write_Maps);
            Data.Write((byte)Map.List.Count);
            foreach (Map Map in Map.List.Values)
            {
                // Escreve os dados
                Data.Write(Map.ID.ToString());
                Data.Write(++Map.Revision);
                Data.Write(Map.Name);
                Data.Write((byte)Map.Moral);
                Data.Write(Map.Panorama);
                Data.Write((byte)Map.Music);
                Data.Write(Map.Color.ToArgb());
                Data.Write((byte)Map.Weather.Type);
                Data.Write(Map.Weather.Intensity);
                Data.Write(Map.Fog.Texture);
                Data.Write(Map.Fog.Speed_X);
                Data.Write(Map.Fog.Speed_Y);
                Data.Write(Map.Fog.Alpha);
                Data.Write(Map.Lighting);

                // Ligações
                for (short i = 0; i < (short)Directions.Count; i++)
                    Data.Write(Map.Link[i].GetID());

                // Camadas
                Data.Write((byte)Map.Layer.Count);
                for (byte i = 0; i < Map.Layer.Count; i++)
                {
                    Data.Write(Map.Layer[i].Name);
                    Data.Write(Map.Layer[i].Type);

                    // Azulejos
                    for (byte x = 0; x < Map.Width; x++)
                        for (byte y = 0; y < Map.Height; y++)
                        {
                            Data.Write(Map.Layer[i].Tile[x, y].X);
                            Data.Write(Map.Layer[i].Tile[x, y].Y);
                            Data.Write(Map.Layer[i].Tile[x, y].Texture);
                            Data.Write(Map.Layer[i].Tile[x, y].IsAutotile);
                        }
                }

                // Dados específicos dos azulejos
                for (byte x = 0; x < Map.Width; x++)
                    for (byte y = 0; y < Map.Height; y++)
                    {
                        Data.Write(Map.Attribute[x, y].Type);
                        Data.Write(Map.Attribute[x, y].Data_1);
                        Data.Write(Map.Attribute[x, y].Data_2);
                        Data.Write(Map.Attribute[x, y].Data_3);
                        Data.Write(Map.Attribute[x, y].Data_4);
                        Data.Write(Map.Attribute[x, y].Zone);

                        // Bloqueio direcional
                        for (byte i = 0; i < (byte)Directions.Count; i++)
                            Data.Write(Map.Attribute[x, y].Block[i]);
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
                    Data.Write(Map.NPC[i].NPC.GetID());
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
            Data.Write((short)NPC.List.Count);
            foreach (NPC NPC in NPC.List.Values)
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
                for (byte i = 0; i < (byte)Vitals.Count; i++) Data.Write(NPC.Vital[i]);
                for (byte i = 0; i < (byte)Attributes.Count; i++) Data.Write(NPC.Attribute[i]);
                Data.Write((byte)NPC.Drop.Count);
                for (byte i = 0; i < NPC.Drop.Count; i++)
                {
                    Data.Write(NPC.Drop[i].Item.GetID());
                    Data.Write(NPC.Drop[i].Amount);
                    Data.Write(NPC.Drop[i].Chance);
                }
                Data.Write(NPC.AttackNPC);
                Data.Write((byte)NPC.Allie.Count);
                for (byte i = 0; i < NPC.Allie.Count; i++) Data.Write(NPC.Allie[i].GetID());
                Data.Write((byte)NPC.Movement);
                Data.Write(NPC.Flee_Helth);
                Data.Write(NPC.Shop.GetID());
            }
            Packet(Data);
        }

        public static void Write_Items()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Write_Items);
            Data.Write((short)Item.List.Count);
            foreach (Item Item in Item.List.Values)
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
                Data.Write(Item.Req_Class.GetID());
                Data.Write(Item.Potion_Experience);
                for (byte i = 0; i < (byte)Vitals.Count; i++) Data.Write(Item.Potion_Vital[i]);
                Data.Write(Item.Equip_Type);
                for (byte i = 0; i < (byte)Attributes.Count; i++) Data.Write(Item.Equip_Attribute[i]);
                Data.Write(Item.Weapon_Damage);
            }
            Packet(Data);
        }

        public static void Write_Shops()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Write_Shops);
            Data.Write((short)Shop.List.Count);
            foreach (Shop Shop in Shop.List.Values)
            {
                // Geral
                Data.Write(Shop.ID.ToString());
                Data.Write((byte)Shop.Sold.Count);
                Data.Write((byte)Shop.Bought.Count);
                Data.Write(Shop.Name);
                Data.Write(Shop.Currency.GetID());
                for (byte j = 0; j < Shop.Sold.Count; j++)
                {
                    Data.Write(Shop.Sold[j].Item.GetID());
                    Data.Write(Shop.Sold[j].Amount);
                    Data.Write(Shop.Sold[j].Price);
                }
                for (byte j = 0; j < Shop.Bought.Count; j++)
                {
                    Data.Write(Shop.Bought[j].Item.GetID());
                    Data.Write(Shop.Bought[j].Amount);
                    Data.Write(Shop.Bought[j].Price);
                }
            }

            Packet(Data);
        }
    }
}