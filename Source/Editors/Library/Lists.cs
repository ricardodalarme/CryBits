using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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
    public static TreeNode Tool_Order;
    
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
        }

        public class Tool_Order
        {
            public byte Index;
            public Globals.Tools_Types Type;

            // Retorna a ferramenta
            public Tool Data
            {
                get
                {
                    switch (Type)
                    {
                        case Globals.Tools_Types.Button: return Lists.Button[Index];
                        case Globals.Tools_Types.TextBox: return Lists.TextBox[Index];
                        case Globals.Tools_Types.Panel: return Lists.Panel[Index];
                        case Globals.Tools_Types.CheckBox: return Lists.CheckBox[Index];
                    }
                    return null;
                }
            }

            public Tool_Order()
            {
                Set = new List<Tool_Order>();
            }
        }

        public class Tool
        {
            public string Name { get; set; }
            public Point Position { get; set; }
            public bool Visible { get; set; }
            public Globals.Windows Window { get; set; }
            public bool Viewable
            {
                get
                {
                   return (Editor_Interface.Objects.cmbWIndows.SelectedIndex == (byte)Window) && Visible;
                }
            }
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
            public bool State { get; set; }
        }

        public class Panel : Tool
        {
            public byte Texture_Num { get; set; }
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