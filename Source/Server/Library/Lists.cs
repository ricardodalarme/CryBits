using System;
using System.Collections.Generic;
using Objects;

class Lists
{
    // Armazenamento de dados
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static List<Account> Account = new List<Account>();
    public static Dictionary<Guid, Class> Class = new Dictionary<Guid, Class>();
    public static Dictionary<Guid, Item> Item = new Dictionary<Guid, Item>();
    public static Dictionary<Guid, Shop> Shop = new Dictionary<Guid, Shop>();
    public static Dictionary<Guid, NPC> NPC = new Dictionary<Guid, NPC>();
    public static Dictionary<Guid, Map> Map = new Dictionary<Guid, Map>();
    public static Dictionary<Guid, TMap> Temp_Map = new Dictionary<Guid, TMap>();

    public static string GetID(Data Object) => Object == null ? Guid.Empty.ToString() : Object.ID.ToString();

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
        public class Server_Data
        {
            public string Game_Name = "CryBits";
            public string Welcome = "Welcome to CryBits.";
            public short Port = 7001;
            public byte Max_Players = 15;
            public byte Max_Characters = 3;
            public byte Max_Party_Members = 3;
            public byte Max_Map_Items = 100;
            public byte Num_Points = 3;
            public byte Max_Name_Length = 12;
            public byte Min_Name_Length = 3;
            public byte Max_Password_Length = 12;
            public byte Min_Password_Length = 3;
        }

        public struct Inventories
        {
            public Item Item;
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
    }
}