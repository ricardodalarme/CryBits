using Objects;
using System;
using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Options Options = new Structures.Options();
    public static List<Player.Structure> Player = new List<Player.Structure>();
    public static Dictionary<Guid, Class> Class = new Dictionary<Guid, Class>();
    public static Dictionary<Guid, Map> Map = new Dictionary<Guid, Map>();
    public static Dictionary<Guid, TMap> Temp_Map = new Dictionary<Guid, TMap>();
    public static Dictionary<Guid, NPC> NPC = new Dictionary<Guid, NPC>();
    public static Dictionary<Guid, Item> Item = new Dictionary<Guid, Item>();
    public static Dictionary<Guid, Shop> Shop = new Dictionary<Guid, Shop>();
    public static Structures.Character[] Characters;
    public static Structures.Weather[] Weather;

    public static object GetData<T>(Dictionary<Guid, T> Dictionary, Guid ID)
    {
        if (Dictionary.ContainsKey(ID)) return Dictionary[ID];
        return null;
    }

    public static string GetID(object Object)
    {
        return Object == null ? Guid.Empty.ToString() : ((Data)Object).ID.ToString();
    }

    // Estrutura dos itens em gerais
    public class Structures
    {
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

        public struct Weather
        {
            public bool Visible;
            public int x;
            public int y;
            public int Speed;
            public int Start;
            public bool Back;
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
    }
}