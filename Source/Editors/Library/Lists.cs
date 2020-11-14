using System;
using System.Windows.Forms;
using CryBits.Editors.Entities;
using CryBits.Entities;

namespace CryBits.Editors.Library
{
    internal static class Lists
    {
        // Armazenamento de dados
        public static Structures.Options Options = new Structures.Options();
        public static Structures.ServerData Server_Data = new Structures.ServerData();
        public static MapWeatherParticle[] Weather;
        public static Tile[] Tile;
        public static TreeNode Tool;


        // Estrutura dos itens em gerais
        public static class Structures
        {
            [Serializable]
            public struct Options
            {
                public bool Pre_Map_Grid;
                public bool Pre_Map_View;
                public bool Pre_Map_Audio;
                public string Username;
            }

            public struct ServerData
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
        }
    }
}