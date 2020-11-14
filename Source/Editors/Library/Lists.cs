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
        public static Structures.ServerData ServerData = new Structures.ServerData();
        public static MapWeatherParticle[] Weather;
        public static Tile[] Tile;
        public static TreeNode Tool;


        // Estrutura dos itens em gerais
        public static class Structures
        {
            [Serializable]
            public struct Options
            {
                public bool PreMapGrid;
                public bool PreMapView;
                public bool PreMapAudio;
                public string Username;
            }

            public struct ServerData
            {
                public string GameName;
                public string Welcome;
                public short Port;
                public byte MaxPlayers;
                public byte MaxCharacters;
                public byte MaxPartyMembers;
                public byte MaxMapItems;
                public byte NumPoints;
                public byte MaxNameLength;
                public byte MinNameLength;
                public byte MaxPasswordLength;
                public byte MinPasswordLength;
            }
        }
    }
}