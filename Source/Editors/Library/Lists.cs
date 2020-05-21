using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class Lists
{
    // Armazenamento de dados
    public static Structures.Options Options = new Structures.Options();
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static Dictionary<Guid, Structures.Class> Class = new Dictionary<Guid, Structures.Class>();
    public static Structures.Tile[] Tile;
    public static Structures.Map[] Map;
    public static Structures.Weather[] Weather;
    public static Dictionary<Guid, Structures.NPC> NPC = new Dictionary<Guid, Structures.NPC>();
    public static Dictionary<Guid, Structures.Item> Item = new Dictionary<Guid, Structures.Item>();
    public static Dictionary<Guid, Structures.Shop> Shop = new Dictionary<Guid, Structures.Shop>();
    public static TreeNode Tool;

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
        public class Data
        {
            public Guid ID;

            public Data(Guid ID)
            {
                this.ID = ID;
            }
        }

        public struct Options
        {
            public string Directory_Client;
            public string Directory_Server;
            public bool Pre_Map_Grid;
            public bool Pre_Map_View;
            public bool Pre_Map_Audio;
        }

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
        }

        public class Tool
        {
            public string Name { get; set; }
            public Point Position { get; set; }
            public bool Visible { get; set; }
            public Globals.Windows Window { get; set; }
        }

        public class Button : Tool
        {
            public byte Texture_Num { get; set; }
        }

        public class TextBox : Tool
        {
            public short Max_Characters { get; set; }
            public short Width { get; set; }
            public bool Password { get; set; }
        }

        public class CheckBox : Tool
        {
            public string Text { get; set; }
            public bool Checked { get; set; }
        }

        public class Panel : Tool
        {
            public byte Texture_Num { get; set; }
        }

        public class Class : Data
        {
            public string Name = string.Empty;
            public string Description;
            public List<short> Tex_Male = new List<short>();
            public List<short> Tex_Female = new List<short>();
            public short Spawn_Map = 1;
            public byte Spawn_Direction;
            public byte Spawn_X;
            public byte Spawn_Y;
            public short[] Vital = new short[(byte)Globals.Vitals.Count];
            public short[] Attribute = new short[(byte)Globals.Attributes.Count];
            public List<Tuple<Item, short>> Item = new List<Tuple<Item, short>>();

            public Class(Guid ID) : base(ID) { }
            public override string ToString() => Name;
        }

        [Serializable]
        public struct Tile
        {
            public byte Width;
            public byte Height;
            public Tile_Data[,] Data;
        }

        [Serializable]
        public struct Tile_Data
        {
            public byte Attribute;
            public bool[] Block;
        }

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
            public List<Map_Light> Light;
            public List<Map_NPC> NPC;
        }

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

        public class Map_Light
        {
            public byte X;
            public byte Y;
            public byte Width;
            public byte Height;

            public Map_Light(Rectangle Rec)
            {
                // Define os dados da estrutura
                X = (byte)Rec.X;
                Y = (byte)Rec.Y;
                Width = (byte)Rec.Width;
                Height = (byte)Rec.Height;
            }

            public Rectangle Rec
            {
                get
                {
                    return new Rectangle(X, Y, Width, Height);
                }
            }
        }

        public struct Map_Weather
        {
            public byte Type;
            public byte Intensity;
        }

        public struct Map_Fog
        {
            public byte Texture;
            public sbyte Speed_X;
            public sbyte Speed_Y;
            public byte Alpha;
        }

        public struct Map_Tile_Data
        {
            public byte X;
            public byte Y;
            public byte Tile;
            public bool Auto;
            public Point[] Mini;
        }

        public class Map_Layer
        {
            public string Name;
            public byte Type;
            public Map_Tile_Data[,] Tile;
        }

        public struct Map_NPC
        {
            public NPC NPC;
            public byte Zone;
            public bool Spawn;
            public byte X;
            public byte Y;
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
            public string Name = string.Empty;
            public string SayMsg = string.Empty;
            public short Texture;
            public byte Behaviour;
            public byte SpawnTime;
            public byte Sight;
            public int Experience;
            public short[] Vital = new short[(byte)Globals.Vitals.Count];
            public short[] Attribute = new short[(byte)Globals.Attributes.Count];
            public List<NPC_Drop> Drop = new List<NPC_Drop>();
            public bool AttackNPC;
            public List<NPC> Allie = new List<NPC>();
            public Globals.NPC_Movements Movement;
            public byte Flee_Helth;
            public Shop Shop;

            public NPC(Guid ID) : base(ID) { }
            public override string ToString() => Name;
        }

        public class Item : Data
        {
            // Geral
            public string Name = string.Empty;
            public string Description = string.Empty;
            public short Texture;
            public byte Type;
            public bool Stackable;
            public byte Bind;
            public byte Rarity;
            // Requerimentos
            public short Req_Level;
            public Class Req_Class;
            // Poção
            public int Potion_Experience;
            public short[] Potion_Vital = new short[(byte)Globals.Vitals.Count];
            // Equipamento
            public byte Equip_Type;
            public short[] Equip_Attribute = new short[(byte)Globals.Attributes.Count];
            public short Weapon_Damage;

            public Item(Guid ID) : base(ID) { }
            public override string ToString() => Name;
        }

        public class NPC_Drop
        {
            private Guid item;
            public Item Item
            {
                get => (Item)Lists.GetData(Lists.Item, item);
                set => item = new Guid(GetID(value));
            }
            public short Amount;
            public byte Chance;

            public NPC_Drop(Item Item, short Amount, byte Chance)
            {
                this.Item = Item;
                this.Amount = Amount;
                this.Chance = Chance;
            }
        }

        public class Shop : Data
        {
            public string Name = string.Empty;
            public Item Currency;
            public List<Shop_Item> Sold = new List<Shop_Item>();
            public List<Shop_Item> Bought = new List<Shop_Item>();

            public Shop(Guid ID) : base(ID) { }
            public override string ToString() => Name;
        }

        public class Shop_Item
        {
            public Item Item;
            public short Amount;
            public short Price;

            public Shop_Item(Item Item, short Amount, short Price)
            {
                this.Item = Item;
                this.Amount = Amount;
                this.Price = Price;
            }
        }
    }
}