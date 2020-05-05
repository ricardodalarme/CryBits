using System;
using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Options Options = new Structures.Options();
    public static Structures.Player[] Player;
    public static Structures.Class[] Class;
    public static Structures.Character[] Characters;
    public static Structures.Map Map;
    public static Structures.Temp_Map Temp_Map;
    public static Structures.Weather[] Weather;
    public static Structures.NPC[] NPC;
    public static Structures.Item[] Item;
    public static Structures.Shop[] Shop;

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

        public class Player
        {
            // Apenas na parte do cliente
            public short X2;
            public short Y2;
            public byte Animation;
            public bool Attacking;
            public int Attack_Timer;
            public int Hurt;
            public short[] Max_Vital;
            public int Collect_Timer;
            // Geral
            public string Name;
            public byte Class;
            public short Texture_Num;
            public bool Genre;
            public short Level;
            public int Experience;
            public int ExpNeeded;
            public short Points;
            public short[] Vital;
            public short[] Attribute;
            public short Map;
            public byte X;
            public byte Y;
            public Game.Directions Direction;
            public Game.Movements Movement;
            public short[] Equipment;
            public byte[] Party;
            public byte Trade;
        }

        public class Character
        {
            public string Name;
            public short Level;
            public short Texture_Num;
        }

        public class Class
        {
            public string Name;
            public string Description;
            public short[] Tex_Male;
            public short[] Tex_Female;
        }

        [Serializable]
        public struct Map
        {
            public short Revision;
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
            public Map_Tile[,] Tile;
            public Map_Light[] Light;
            public short[] NPC;
        }

        public struct Temp_Map
        {
            public Map_NPCs[] NPC;
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

        public struct NPC
        {
            public string Name;
            public string SayMsg;
            public short Texture;
            public byte Type;
            public short[] Vital;
        }

        public struct Map_NPCs
        {
            // Apenas na parte do cliente
            public short X2;
            public short Y2;
            public byte Animation;
            public bool Attacking;
            public int Attack_Timer;
            public int Hurt;
            // Geral
            public short Index;
            public byte X;
            public byte Y;
            public Game.Directions Direction;
            public Game.Movements Movement;
            public short[] Vital;
        }

        public struct Map_Items
        {
            public short Index;
            public byte X;
            public byte Y;
        }

        public struct Item
        {
            // Geral
            public string Name;
            public string Description;
            public short Texture;
            public byte Type;
            public byte Rarity;
            public Game.BindOn Bind;
            // Requerimentos
            public short Req_Level;
            public byte Req_Class;
            // Poção
            public int Potion_Experience;
            public short[] Potion_Vital;
            // Equipamento
            public byte Equip_Type;
            public short[] Equip_Attribute;
            public short Weapon_Damage;
        }

        public struct Inventory
        {
            public short Item_Num;
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

        public class Shop
        {
            public string Name;
            public short Currency;
            public Shop_Item[] Sold;
            public Shop_Item[] Bought;
        }

        public class Shop_Item
        {
            public short Item_Num;
            public short Amount;
            public short Price;
        }
    }
}