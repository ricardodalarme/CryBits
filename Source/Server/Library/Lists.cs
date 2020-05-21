using System;
using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static List<Account.Structure> Account = new List<Account.Structure>();
    public static Dictionary<Guid, Objects.Class> Class = new Dictionary<Guid, Objects.Class>();
    public static Dictionary<Guid, Objects.Item> Item = new Dictionary<Guid, Objects.Item>();
    public static Dictionary<Guid, Objects.Shop> Shop = new Dictionary<Guid, Objects.Shop>();
    public static Objects.NPC[] NPC;
    public static Structures.Map[] Map;
    public static Structures.Temp_Map[] Temp_Map;
    public static Structures.Tile[] Tile;

    public static string GetID(Structures.Data Object)
    {
        return Object == null ? Guid.Empty.ToString() : Object.ID.ToString();
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
            public NPC.Structure[] NPC;
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
            public string Data_5;
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

      

        public struct Map_Items
        {
            public Objects.Item Item;
            public byte X;
            public byte Y;
            public short Amount;
        }

        public struct Inventories
        {
            public Objects.Item Item;
            public short Amount;
        }

        public struct Trade_Slot
        {
            public short Slot_Num;
            public short Amount;
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
    }
}