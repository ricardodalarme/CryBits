using System.Collections.Generic;
using System.Drawing;

class Lists
{
    // Armazenamento de dados
    public static Structures.Options Options = new Structures.Options();
    public static Structures.Client_Data Client_Data = new Structures.Client_Data();
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static Structures.Button[] Button;
    public static Structures.TextBox[] TextBox;
    public static Structures.CheckBox[] CheckBox;
    public static Structures.Panel[] Panel;
    public static Structures.Class[] Class;
    public static Structures.Tile[] Tile;
    public static Structures.Map[] Map;
    public static Structures.Weather[] Weather;
    public static Structures.NPC[] NPC;
    public static Structures.Item[] Item;

    // Estrutura dos itens em gerais
    public class Structures
    {
        public struct Options
        {
            public string Directory_Client;
            public string Directory_Server;
            public bool Pre_Map_Grid;
            public bool Pre_Map_View;
            public bool Pre_Map_Audio;
        }

        public struct Client_Data
        {
            public byte Num_Buttons;
            public byte Num_Panels;
            public byte Num_CheckBoxes;
            public byte Num_TextBoxes;
        }

        public struct Server_Data
        {
            public string Game_Name;
            public string Welcome;
            public short Port;
            public byte Max_Players;
            public byte Max_Characters;
            public byte Num_Classes;
            public byte Num_Tiles;
            public short Num_Maps;
            public short Num_NPCs;
            public short Num_Items;
        }

        public struct Tool
        {
            public string Name;
            public bool Visible;
            public Point Position;
        }

        public struct Button
        {
            public byte Texture;
            public Tool General;
        }

        public struct TextBox
        {
            public short Max_Chars;
            public short Width;
            public bool Password;
            public Tool General;
        }

        public struct CheckBox
        {
            public string Text;
            public bool State;
            public Tool General;
        }

        public struct Panel
        {
            public byte Texture;
            public Tool General;
        }

        public struct Class
        {
            public string Name;
            public short Texture_Male;
            public short Texture_Female;
            public short Spawn_Map;
            public byte Spawn_Direction;
            public byte Spawn_X;
            public byte Spawn_Y;
            public short[] Vital;
            public short[] Attribute;
        }

        public struct Tile
        {
            public byte Width;
            public byte Height;
            public Tile_Data[,] Data;
        }

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
            public byte x;
            public byte y;
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
            public short Index;
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

        public struct NPC
        {
            public string Name;
            public short Texture;
            public byte Behaviour;
            public byte SpawnTime;
            public byte Sight;
            public byte Experience;
            public short[] Vital;
            public short[] Attribute;
            public NPC_Drop[] Drop;
        }

        public struct Item
        {
            // Geral
            public string Name;
            public string Description;
            public short Texture;
            public byte Type;
            public short Price;
            public bool Stackable;
            public bool Bind;
            // Requerimentos
            public short Req_Level;
            public byte Req_Class;
            // Poção
            public short Potion_Experience;
            public short[] Potion_Vital;
            // Equipamento
            public byte Equip_Type;
            public short[] Equip_Attribute;
            public short Weapon_Damage;
        }

        public struct NPC_Drop
        {
            public short Item_Num;
            public short Amount;
            public byte Chance;
        }
    }
}