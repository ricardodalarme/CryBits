using Entities;
using System;
using System.Windows.Forms;

static class Lists
{
    // Armazenamento de dados
    public static Structures.Options Options = new Structures.Options();
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static Structures.Weather[] Weather;
    public static Tile[] Tile;
    public static TreeNode Tool;


    // Estrutura dos itens em gerais
    public class Structures
    {
        [Serializable]
        public struct Options
        {
            public string Directory_Client;
            public bool Pre_Map_Grid;
            public bool Pre_Map_View;
            public bool Pre_Map_Audio;
            public string Username;
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

        public struct Weather
        {
            public bool Visible;
            public int x;
            public int y;
            public int Speed;
            public int Start;
            public bool Back;
        }
    }
}