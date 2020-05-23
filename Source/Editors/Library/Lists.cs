using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

class Lists
{
    // Armazenamento de dados
    public static Structures.Options Options = new Structures.Options();
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static Dictionary<Guid, Structures.Class> Class = new Dictionary<Guid, Structures.Class>();
    public static Structures.Tile[] Tile;
    public static Dictionary<Guid, Structures.Map> Map = new Dictionary<Guid, Structures.Map>();
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

        [Serializable]
        public class Options
        {
            public string Directory_Client;
            public bool Pre_Map_Grid;
            public bool Pre_Map_View;
            public bool Pre_Map_Audio;
            public string Username = string.Empty;
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
            public byte Max_Name_Length;
            public byte Min_Name_Length;
            public byte Max_Password_Length;
            public byte Min_Password_Length;
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

            public override string ToString() => "[Button] " + Name;
        }

        public class TextBox : Tool
        {
            public short Max_Characters { get; set; }
            public short Width { get; set; }
            public bool Password { get; set; }

            public override string ToString() => "[TextBox] " + Name;
        }

        public class CheckBox : Tool
        {
            public string Text { get; set; }
            public bool Checked { get; set; }

            public override string ToString() => "[CheckBox] " + Name;
        }

        public class Panel : Tool
        {
            public byte Texture_Num { get; set; }

            public override string ToString() => "[Panel] " + Name;
        }

        public class Class : Data
        {
            public string Name = string.Empty;
            public string Description;
            public BindingList<short> Tex_Male = new BindingList<short>();
            public BindingList<short> Tex_Female = new BindingList<short>();
            public Map Spawn_Map;
            public byte Spawn_Direction;
            public byte Spawn_X;
            public byte Spawn_Y;
            public short[] Vital = new short[(byte)Globals.Vitals.Count];
            public short[] Attribute = new short[(byte)Globals.Attributes.Count];
            public BindingList<Inventory> Item = new BindingList<Inventory>();

            public Class(Guid ID) : base(ID) { }
            public override string ToString() => Name;
        }

        public class Inventory
        {
            public Item Item;
            public short Amount;

            public Inventory(Item Item, short Amount)
            {
                this.Item = Item;
                this.Amount = Amount;
            }
            public override string ToString() => Item.Name + " - " + Amount + "x";
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

        public class Map : Data
        {
            public short Revision;
            public List<Map_Layer> Layer = new List<Map_Layer>();
            public Map_Tile[,] Tile;
            public string Name;
            public byte Width;
            public byte Height;
            public byte Moral;
            public byte Panorama;
            public byte Music;
            public int Color;
            public Map_Weather Weather = new Map_Weather();
            public Map_Fog Fog = new Map_Fog();
            public Map[] Link = new Map[(byte)Globals.Directions.Count];
            public byte Light_Global;
            public byte Lighting;
            public List<Map_Light> Light = new List<Map_Light>();
            public List<Map_NPC> NPC = new List<Map_NPC>();

            public Map(Guid ID) : base(ID) { }
            public override string ToString() => Name;

            // Verifica se as coordenas estão no limite do mapa
            public bool OutLimit(short x, short y) => x > Width || y > Height || x < 0 || y < 0;
        }

        public class Map_Tile
        {
            public byte Attribute;
            public short Data_1;
            public short Data_2;
            public short Data_3;
            public short Data_4;
            public string Data_5;
            public byte Zone;
            public bool[] Block = new bool[(byte)Globals.Directions.Count];
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

        public class Map_Weather
        {
            public byte Type;
            public byte Intensity;
        }

        public class Map_Fog
        {
            public byte Texture;
            public sbyte Speed_X;
            public sbyte Speed_Y;
            public byte Alpha;
        }

        public class Map_Tile_Data
        {
            public byte X;
            public byte Y;
            public byte Tile;
            public bool Auto;
            public Point[] Mini = new Point[4];
        }

        public class Map_Layer
        {
            public string Name;
            public byte Type;
            public Map_Tile_Data[,] Tile;
        }

        public class Map_NPC
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
            public BindingList<NPC_Drop> Drop = new BindingList<NPC_Drop>();
            public bool AttackNPC;
            public BindingList<NPC> Allie = new BindingList<NPC>();
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

        public class NPC_Drop : Inventory
        {
            public byte Chance;

            public NPC_Drop(Item Item, short Amount, byte Chance) : base(Item, Amount)
            {
                this.Chance = Chance;
            }
            public override string ToString() => Item.Name + " [" + Amount + "x, " + Chance + "%]";
        }

        public class Shop : Data
        {
            public string Name = string.Empty;
            public Item Currency;
            public BindingList<Shop_Item> Sold = new BindingList<Shop_Item>();
            public BindingList<Shop_Item> Bought = new BindingList<Shop_Item>();

            public Shop(Guid ID) : base(ID) { }
            public override string ToString() => Name;
        }

        public class Shop_Item : Inventory
        {
            public short Price;

            public Shop_Item(Item Item, short Amount, short Price) : base(Item, Amount)
            {
                this.Price = Price;
            }

            public override string ToString() => Item.Name + " - " + Amount + "x [$" + Price + "]";
        }
    }
}