using CryBits.Editors.Forms;
using CryBits.Editors.Entities;
using CryBits;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CryBits.Editors.Network
{
    class Receive
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
            Dictionary<Guid, Class> ToRemove = new Dictionary<Guid, Class>(Class.List);

            // Quantidade de classes
            short Count = Data.ReadByte();

            while (--Count >= 0)
            {
                Guid ID = new Guid(Data.ReadString());
                Class Class;

                // Obtém o dado
                if (Class.List.ContainsKey(ID))
                {
                    Class = Class.List[ID];
                    ToRemove.Remove(ID);
                }
                else
                {
                    Class = new Class(ID);
                    Class.List.Add(Class.ID, Class);
                }

                // Reseta os valores necessários
                Class.Tex_Male.Clear();
                Class.Tex_Female.Clear();

                // Lê os dados
                Class.Name = Data.ReadString();
                Class.Description = Data.ReadString();
                for (byte t = 0, Size = Data.ReadByte(); t < Size; t++) Class.Tex_Male.Add(Data.ReadInt16());
                for (byte t = 0, Size = Data.ReadByte(); t < Size; t++) Class.Tex_Female.Add(Data.ReadInt16());
                Class.Spawn_Map = Entities.Map.Get(new Guid(Data.ReadString()));
                Class.Spawn_Direction = Data.ReadByte();
                Class.Spawn_X = Data.ReadByte();
                Class.Spawn_Y = Data.ReadByte();
                for (byte v = 0; v < (byte)Vitals.Count; v++) Class.Vital[v] = Data.ReadInt16();
                for (byte a = 0; a < (byte)Attributes.Count; a++) Class.Attribute[a] = Data.ReadInt16();
                byte Num_Items = Data.ReadByte();
                for (byte a = 0; a < Num_Items; a++) Class.Item.Add(new Lists.Structures.Inventory(Item.Get(new Guid(Data.ReadString())), Data.ReadInt16()));
            }

            // Remove as classes que não tiveram os dados atualizados
            foreach (Guid Remove in ToRemove.Keys) Class.List.Remove(Remove);
        }

        private static void Map(NetIncomingMessage Data)
        {
            Guid ID = new Guid(Data.ReadString());
            Map Map;

            // Obtém o dado
            if (Map.List.ContainsKey(ID)) Map = Map.List[ID];
            else
            {
                Map = new Map(ID);
                Map.List.Add(ID, Map);
            }

            // Dados básicos
            Map.Revision = Data.ReadInt16();
            Map.Name = Data.ReadString();
            Map.Moral = (Map_Morals)Data.ReadByte();
            Map.Panorama = Data.ReadByte();
            Map.Music = (Audio.Musics)Data.ReadByte();
            Map.Color = Color.FromArgb(Data.ReadInt32());
            Map.Weather.Type = (Weathers)Data.ReadByte();
            Map.Weather.Intensity = Data.ReadByte();
            Map.Fog.Texture = Data.ReadByte();
            Map.Fog.Speed_X = Data.ReadSByte();
            Map.Fog.Speed_Y = Data.ReadSByte();
            Map.Fog.Alpha = Data.ReadByte();
            Map.Lighting = Data.ReadByte();

            // Ligações
            for (short n = 0; n < (short)Directions.Count; n++)
                Map.Link[n] = Map.Get(new Guid(Data.ReadString()));

            // Quantidade de camadas
            byte Num_Layers = Data.ReadByte();

            // Camadas
            for (byte n = 0; n < Num_Layers; n++)
            {
                // Dados básicos
                Map.Layer.Add(new MapLayer());
                Map.Layer[n].Name = Data.ReadString();
                Map.Layer[n].Type = Data.ReadByte();

                // Azulejos
                for (byte x = 0; x < Map.Width; x++)
                    for (byte y = 0; y < Map.Height; y++)
                    {
                        Map.Layer[n].Tile[x, y] = new MapTileData();
                        Map.Layer[n].Tile[x, y].X = Data.ReadByte();
                        Map.Layer[n].Tile[x, y].Y = Data.ReadByte();
                        Map.Layer[n].Tile[x, y].Texture = Data.ReadByte();
                        Map.Layer[n].Tile[x, y].IsAutotile = Data.ReadBoolean();
                    }
            }

            // Dados específicos dos azulejos
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                {
                    Map.Attribute[x, y] = new Map_Attribute();
                    Map.Attribute[x, y].Type = Data.ReadByte();
                    Map.Attribute[x, y].Data_1 = Data.ReadString();
                    Map.Attribute[x, y].Data_2 = Data.ReadInt16();
                    Map.Attribute[x, y].Data_3 = Data.ReadInt16();
                    Map.Attribute[x, y].Data_4 = Data.ReadInt16();
                    Map.Attribute[x, y].Zone = Data.ReadByte();

                    for (byte n = 0; n < (byte)Directions.Count; n++)
                        Map.Attribute[x, y].Block[n] = Data.ReadBoolean();
                }

            // Luzes
            byte Num_Lights = Data.ReadByte();
            Map.Light = new List<Map_Light>();
            if (Num_Lights > 0)
                for (byte n = 0; n < Num_Lights; n++)
                    Map.Light.Add(new Map_Light(new Rectangle(Data.ReadByte(), Data.ReadByte(), Data.ReadByte(), Data.ReadByte())));

            // NPCs
            byte Num_NPCs = Data.ReadByte();
            Map_NPC NPC = new Map_NPC();
            if (Num_NPCs > 0)
                for (byte n = 0; n < Num_NPCs; n++)
                {
                    NPC.NPC = Entities.NPC.Get(new Guid(Data.ReadString()));
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
            Dictionary<Guid, NPC> ToRemove = new Dictionary<Guid, NPC>(NPC.List);

            // Quantidade de NPCs
            short Count = Data.ReadInt16();

            while (--Count >= 0)
            {
                Guid ID = new Guid(Data.ReadString());
                NPC NPC;

                // Obtém o dado
                if (NPC.List.ContainsKey(ID))
                {
                    NPC = NPC.List[ID];
                    ToRemove.Remove(ID);
                }
                else
                {
                    NPC = new NPC(ID);
                    NPC.List.Add(NPC.ID, NPC);
                }

                // Lê os dados
                NPC.Name = Data.ReadString();
                NPC.SayMsg = Data.ReadString();
                NPC.Texture = Data.ReadInt16();
                NPC.Behaviour = Data.ReadByte();
                for (byte n = 0; n < (byte)Vitals.Count; n++) NPC.Vital[n] = Data.ReadInt16();
                NPC.SpawnTime = Data.ReadByte();
                NPC.Sight = Data.ReadByte();
                NPC.Experience = Data.ReadInt32();
                for (byte n = 0; n < (byte)Attributes.Count; n++) NPC.Attribute[n] = Data.ReadInt16();
                for (byte n = 0, Size = Data.ReadByte(); n < Size; n++) NPC.Drop.Add(new NPC_Drop(Item.Get(new Guid(Data.ReadString())), Data.ReadInt16(), Data.ReadByte()));
                NPC.AttackNPC = Data.ReadBoolean();
                for (byte n = 0, Size = Data.ReadByte(); n < Size; n++) NPC.Allie.Add(NPC.Get(new Guid(Data.ReadString())));
                NPC.Movement = (NPC_Movements)Data.ReadByte();
                NPC.Flee_Helth = Data.ReadByte();
                NPC.Shop = Shop.Get(new Guid(Data.ReadString()));
            }

            // Remove os NPCs que não tiveram os dados atualizados
            foreach (Guid Remove in ToRemove.Keys) NPC.List.Remove(Remove);
        }

        private static void Items(NetIncomingMessage Data)
        {
            // Itens a serem removidas
            Dictionary<Guid, Item> ToRemove = new Dictionary<Guid, Item>(Item.List);

            // Quantidade de itens
            short Count = Data.ReadInt16();

            while (--Count >= 0)
            {
                Guid ID = new Guid(Data.ReadString());
                Item Item;

                // Obtém o dado
                if (Item.List.ContainsKey(ID))
                {
                    Item = Item.List[ID];
                    ToRemove.Remove(ID);
                }
                else
                {
                    Item = new Item(ID);
                    Item.List.Add(Item.ID, Item);
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
                Item.Req_Class = Class.Get(new Guid(Data.ReadString()));
                Item.Potion_Experience = Data.ReadInt32();
                for (byte v = 0; v < (byte)Vitals.Count; v++) Item.Potion_Vital[v] = Data.ReadInt16();
                Item.Equip_Type = Data.ReadByte();
                for (byte a = 0; a < (byte)Attributes.Count; a++) Item.Equip_Attribute[a] = Data.ReadInt16();
                Item.Weapon_Damage = Data.ReadInt16();
            }

            // Remove os itens que não tiveram os dados atualizados
            foreach (Guid Remove in ToRemove.Keys) Item.List.Remove(Remove);
        }

        private static void Shops(NetIncomingMessage Data)
        {
            // Lojas a serem removidas
            Dictionary<Guid, Shop> ToRemove = new Dictionary<Guid, Shop>(Shop.List);

            // Quantidade de lojas
            short Count = Data.ReadInt16();

            while (--Count >= 0)
            {
                Guid ID = new Guid(Data.ReadString());
                Shop Shop;

                // Obtém o dado
                if (Shop.List.ContainsKey(ID))
                {
                    Shop = Shop.List[ID];
                    ToRemove.Remove(ID);
                }
                else
                {
                    Shop = new Shop(ID);
                    Shop.List.Add(Shop.ID, Shop);
                }

                // Reseta os valores necessários
                Shop.Sold.Clear();
                Shop.Bought.Clear();

                // Lê os dados
                Shop.Name = Data.ReadString();
                Shop.Currency = Item.Get(new Guid(Data.ReadString()));
                for (byte j = 0, Size = Data.ReadByte(); j < Size; j++) Shop.Sold.Add(new Shop_Item(Item.Get(new Guid(Data.ReadString())), Data.ReadInt16(), Data.ReadInt16()));
                for (byte j = 0, Size = Data.ReadByte(); j < Size; j++) Shop.Bought.Add(new Shop_Item(Item.Get(new Guid(Data.ReadString())), Data.ReadInt16(), Data.ReadInt16()));
            }

            // Remove as lojas que não tiveram os dados atualizados
            foreach (Guid Remove in ToRemove.Keys) Shop.List.Remove(Remove);
        }
    }
}