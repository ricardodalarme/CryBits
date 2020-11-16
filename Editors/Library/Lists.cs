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
        }
    }
}