using System;
using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static List<Objects.Account> Account = new List<Objects.Account>();
    public static Dictionary<Guid, Objects.Class> Class = new Dictionary<Guid, Objects.Class>();
    public static Dictionary<Guid, Objects.Item> Item = new Dictionary<Guid, Objects.Item>();
    public static Dictionary<Guid, Objects.Shop> Shop = new Dictionary<Guid, Objects.Shop>();
    public static Dictionary<Guid, Objects.NPC> NPC = new Dictionary<Guid, Objects.NPC>();
    public static Dictionary<Guid, Objects.Map> Map = new Dictionary<Guid, Objects.Map>();
    public static Dictionary<Guid, Objects.TMap> Temp_Map = new Dictionary<Guid, Objects.TMap>();

    public static string GetID(Structures.Data Object) => Object == null ? Guid.Empty.ToString() : Object.ID.ToString();

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

        public struct TempCharacter
        {
            public string Name;
            public short Texture_Num;
            public short Level;
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
    }
}