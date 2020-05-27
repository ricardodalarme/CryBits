using System;
using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Options Options = new Structures.Options();
    public static List<Player.Structure> Player = new List<Player.Structure>();
    public static Dictionary<Guid, Structures.Class> Class = new Dictionary<Guid, Structures.Class>();
    public static Structures.Character[] Characters;
    public static Structures.Map Map;
    public static Structures.Temp_Map Temp_Map = new Structures.Temp_Map();
    public static Structures.Weather[] Weather;
    public static Dictionary<Guid, Structures.NPC> NPC = new Dictionary<Guid, Structures.NPC>();
    public static Dictionary<Guid, Structures.Item> Item = new Dictionary<Guid, Structures.Item>();
    public static Dictionary<Guid, Structures.Shop> Shop = new Dictionary<Guid, Structures.Shop>();

    public static object GetData<T>(Dictionary<Guid, T> Dictionary, Guid ID)
    {
        if (Dictionary.ContainsKey(ID))
            return Dictionary[ID];
        else
            return null;
    }

    public static string GetID(object Object)
    {
        return Object == null ? Guid.Empty.ToString() : ((Structures.Data)Object).ID.ToString();
    }

    // Estrutura dos itens em gerais
    public class Structures
    {
        public class Data
        {
            public Guid ID;

            public Data(Guid ID)
            {
                this.ID = ID;
            }
        }

        [Serializable]
        public struct Options
        {
            public string Game_Name;
            public bool SaveUsername;
            public bool Sounds;
            public bool Musics;
            public bool Chat;
            public bool FPS;
            public bool Latency;
            public bool Party;
            public bool Trade;
            public string Username;
        }

        public class Character
        {
            public string Name;
            public short Level;
            public short Texture_Num;
        }

        public class Class : Data
        {
            public string Name;
            public string Description;
            public short[] Tex_Male;
            public short[] Tex_Female;

            public Class(Guid ID) : base(ID) { }
        }

        [Serializable]
        public struct Map
        {
            public short Revision;
            public string Name;
            public byte Moral;
            public byte Panorama;
            public byte Music;
            public int Color;
            public Map_Weather Weather;
            public Map_Fog Fog;
            public short[] Link;
            public Map_Tile[,] Tile;
            public Map_Light[] Light;
            public short[] NPC;
        }

        public struct Temp_Map
        {
            public global::NPC[] NPC;
            public Map_Items[] Item;
            public List<Map_Blood> Blood;
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
        public struct Map_Tile
        {
            public byte Attribute;
            public bool[] Block;
            public Map_Tile_Data[,] Data;
        }

        [Serializable]
        public struct Map_Tile_Data
        {
            public byte X;
            public byte Y;
            public byte Tile;
            public bool Automatic;
            public System.Drawing.Point[] Mini;
        }

        [Serializable]
        public struct Map_Light
        {
            public byte X;
            public byte Y;
            public byte Width;
            public byte Height;
        }

        public struct Weather
        {
            public bool Visible;
            public int x;
            public int y;
            public int Speed;
            public int Start;
            public bool Back;
        }

        public class NPC : Data
        {
            public string Name;
            public string SayMsg;
            public short Texture;
            public byte Type;
            public short[] Vital = new short[(byte)Game.Vitals.Count];

            public NPC(Guid ID) : base(ID) { }
        }

        public struct Map_Items
        {
            public Item Item;
            public byte X;
            public byte Y;
        }

        public class Item : Data
        {
            // Geral
            public string Name = string.Empty;
            public string Description = string.Empty;
            public short Texture;
            public byte Type;
            public byte Rarity;
            public Game.BindOn Bind;
            // Requerimentos
            public short Req_Level;
            public Class Req_Class;
            // Poção
            public int Potion_Experience;
            public short[] Potion_Vital = new short[(byte)Game.Vitals.Count];
            // Equipamento
            public byte Equip_Type;
            public short[] Equip_Attribute = new short[(byte)Game.Equipments.Count];
            public short Weapon_Damage;

            public Item(Guid ID) : base(ID) { }
        }

        public struct Inventory
        {
            public Item Item;
            public short Amount;
        }

        public struct Hotbar
        {
            public byte Type;
            public byte Slot;
        }

        public class Map_Blood
        {
            // Dados
            public byte Texture_Num;
            public short X;
            public short Y;
            public byte Opacity;

            // Construtor
            public Map_Blood(byte Texture_Num, short X, short Y, byte Opacity)
            {
                this.Texture_Num = Texture_Num;
                this.X = X;
                this.Y = Y;
                this.Opacity = Opacity;
            }
        }

        public class Shop : Data
        {
            public string Name;
            public Item Currency;
            public Shop_Item[] Sold;
            public Shop_Item[] Bought;

            public Shop(Guid ID) : base(ID) { }
        }

        public class Shop_Item
        {
            public Item Item;
            public short Amount;
            public short Price;
        }
    }
}