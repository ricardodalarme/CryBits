using System;
using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static List<Account.Structure> Account = new List<Account.Structure>();
    public static Dictionary<Guid, Structures.Class> Class = new Dictionary<Guid, Structures.Class>();
    public static Structures.Map[] Map;
    public static Structures.Temp_Map[] Temp_Map;
    public static Structures.NPC[] NPC;
    public static Structures.Item[] Item;
    public static Structures.Tile[] Tile;
    public static Dictionary<Guid, Structures.Shop> Shop = new Dictionary<Guid, Structures.Shop>();

    public static string GetID(object Object)
    {
        return Object == null ? Guid.Empty.ToString() : ((Structures.Data)Object).ID.ToString();
    }

    public static object GetData<T>(Dictionary<Guid, T> Dictionary, Guid ID)
    {
        if (Dictionary.ContainsKey(ID))
            return Dictionary[ID];
        else
            return null;
    }

    // Estrutura dos itens em gerais
    public class Structures
    {
        [Serializable]
        public struct Server_Data
        {
            public string Game_Name;
            public string Welcome;
            public short Port;
            public byte Max_Players;
            public byte Max_Characters;
            public byte Max_Party_Members;
            public byte Max_Map_Items;
            public byte Num_Points;
            public short Num_Maps;
            public byte Num_Tiles;
            public short Num_NPCs;
            public short Num_Items;
        }

        [Serializable]
        public class Data
        {
            public Guid ID;

            public Data(Guid ID)
            {
                this.ID = ID;
            }
        }

        [Serializable]
        public class Class : Data
        {
            public string Name;
            public string Description;
            public short[] Tex_Male = Array.Empty<short>();
            public short[] Tex_Female = Array.Empty<short>();
            public short Spawn_Map = 1;
            public byte Spawn_Direction;
            public byte Spawn_X;
            public byte Spawn_Y;
            public short[] Vital = new short[(byte)Game.Vitals.Count];
            public short[] Attribute = new short[(byte)Game.Attributes.Count];
            public Tuple<short, short>[] Item = Array.Empty<Tuple<short, short>>();

            public Class(Guid ID) : base(ID) { }
        }

        [Serializable]
        public struct Map
        {
            public short Revision;
            public List<Map_Layer> Layer;
            public Map_Tile[,] Tile;
            public string Name;
            public byte Width;
            public byte Height;
            public byte Moral;
            public byte Panorama;
            public byte Music;
            public int Color;
            public Map_Weather Weather;
            public Map_Fog Fog;
            public short[] Link;
            public byte Light_Global;
            public byte Lighting;
            public Map_Light[] Light;
            public Map_NPC[] NPC;
        }

        public struct Temp_Map
        {
            // Temporário
            public global::NPC.Structure[] NPC;
            public List<Map_Items> Item;
        }

        [Serializable]
        public struct Map_Tile
        {
            public byte Attribute;
            public short Data_1;
            public short Data_2;
            public short Data_3;
            public short Data_4;
            public byte Zone;
            public bool[] Block;
        }

        [Serializable]
        public class Map_Layer
        {
            public string Name;
            public byte Type;
            public Map_Tile_Data[,] Tile;
        }

        [Serializable]
        public struct Map_NPC
        {
            public short Index;
            public byte Zone;
            public bool Spawn;
            public byte X;
            public byte Y;
        }

        [Serializable]
        public struct Map_Tile_Data
        {
            public byte X;
            public byte Y;
            public byte Tile;
            public bool Auto;
        }

        [Serializable]
        public class Map_Light
        {
            public byte X;
            public byte Y;
            public byte Width;
            public byte Height;
        }

        [Serializable]
        public struct Map_Weather
        {
            public byte Type;
            public byte Intensity;
        }

        [Serializable]
        public struct Map_Fog
        {
            public byte Texture;
            public sbyte Speed_X;
            public sbyte Speed_Y;
            public byte Alpha;
        }

        [Serializable]
        public class NPC
        {
            public string Name;
            public string SayMsg;
            public short Texture;
            public byte Behaviour;
            public byte SpawnTime;
            public byte Sight;
            public int Experience;
            public short[] Vital;
            public short[] Attribute;
            public NPC_Drop[] Drop;
            public bool AttackNPC;
            public short[] Allie;
            public global::NPC.Movements Movement;
            public byte Flee_Helth;
            private Guid shop;

            public Shop Shop
            {
                get => (Shop)GetData(Lists.Shop, shop);
                set => shop = new Guid(GetID(value));
            }

            public bool IsAlied(short Index)
            {
                // Verifica se o NPC é aliado do outro
                for (byte i = 0; i < Allie.Length; i++)
                    if (Allie[i] == Index)
                        return true;

                return false;
            }
        }

        public struct Map_Items
        {
            public short Item_Num;
            public byte X;
            public byte Y;
            public short Amount;
        }

        [Serializable]
        public class Item
        {
            // Geral
            public string Name;
            public string Description;
            public short Texture;
            public byte Type;
            public bool Stackable;
            public byte Bind;
            public byte Rarity;
            // Requerimentos
            public short Req_Level;
            private Guid req_Class;
            public Class Req_Class
            {
                get => (Class)GetData(Lists.Shop, req_Class);
                set => req_Class = new Guid(GetID(value));
            }
            // Poção
            public int Potion_Experience;
            public short[] Potion_Vital;
            // Equipamento
            public byte Equip_Type;
            public short[] Equip_Attribute;
            public short Weapon_Damage;
        }

        public struct Inventories
        {
            public short Item_Num;
            public short Amount;
        }

        [Serializable]
        public class NPC_Drop
        {
            // Dados
            public short Item_Num;
            public short Amount;
            public byte Chance;

            // Construtor    
            public NPC_Drop(short Item_Num, short Amount, byte Chance)
            {
                this.Item_Num = Item_Num;
                this.Amount = Amount;
                this.Chance = Chance;
            }
        }

        public struct Hotbar
        {
            public byte Type;
            public byte Slot;
        }

        [Serializable]
        public class Tile
        {
            public byte Width;
            public byte Height;
            public Tile_Data[,] Data;
        }

        [Serializable]
        public class Tile_Data
        {
            public byte Attribute;
            public bool[] Block;
        }

        [Serializable]
        public class Shop : Data
        {
            public string Name;
            public short Currency = 1;
            public Shop_Item[] Bought = Array.Empty<Shop_Item>();
            public Shop_Item[] Sold = Array.Empty<Shop_Item>();

            public Shop(Guid ID) : base(ID) { }

            public Shop_Item BoughtItem(short Item_Num)
            {
                // Verifica se a loja vende determinado item
                for (byte i = 0; i < Bought.Length; i++)
                    if (Bought[i].Item_Num == Item_Num)
                        return Bought[i];
                return null;
            }
        }

        [Serializable]
        public class Shop_Item
        {
            public short Item_Num;
            public short Amount;
            public short Price;
        }
    }
}