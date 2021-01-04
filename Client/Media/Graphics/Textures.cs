using System.Collections.Generic;
using System.IO;
using CryBits.Client.Library;
using SFML.Graphics;

namespace CryBits.Client.Media.Graphics
{
    internal static class Textures
    {
        // Texturas
        public static List<Texture> Characters;
        public static List<Texture> Tiles;
        public static List<Texture> Faces;
        public static List<Texture> Panels;
        public static List<Texture> Buttons;
        public static List<Texture> Panoramas;
        public static List<Texture> Fogs;
        public static List<Texture> Items;
        public static Texture CheckBox;
        public static Texture TextBox;
        public static Texture Weather;
        public static Texture Blank;
        public static Texture Shadow;
        public static Texture Bars;
        public static Texture BarsPanel;
        public static Texture Equipments;
        public static Texture Blood;
        public static Texture PartyBars;
        
        // Formato das texturas
        private const string Format = ".png";

        private static List<Texture> LoadTextures(string directory)
        {
            short i = 1;
            List<Texture> tempTex = new List<Texture> { null };

            // Carrega todas do diretório e as adiciona a lista
            while (File.Exists(directory + i + Format))
                tempTex.Add(new Texture(directory + i++ + Format));

            // Retorna o cache da textura
            return tempTex;
        }

        public static void LoadAll()
        {
            // Conjuntos
            Characters = LoadTextures(Directories.TexCharacters.FullName);
            Tiles = LoadTextures(Directories.TexTiles.FullName);
            Faces = LoadTextures(Directories.TexFaces.FullName);
            Panels = LoadTextures(Directories.TexPanels.FullName);
            Buttons = LoadTextures(Directories.TexButtons.FullName);
            Panoramas = LoadTextures(Directories.TexPanoramas.FullName);
            Fogs = LoadTextures(Directories.TexFogs.FullName);
            Items = LoadTextures(Directories.TexItems.FullName);

            // Únicas
            Weather = new Texture(Directories.TexWeather.FullName + Format);
            Blank = new Texture(Directories.TexBlank.FullName + Format);
            CheckBox = new Texture(Directories.TexCheckBox.FullName + Format);
            TextBox = new Texture(Directories.TexTextBox.FullName + Format);
            Shadow = new Texture(Directories.TexShadow.FullName + Format);
            Bars = new Texture(Directories.TexBars.FullName + Format);
            BarsPanel = new Texture(Directories.TexBarsPanel.FullName + Format);
            Equipments = new Texture(Directories.TexEquipments.FullName + Format);
            Blood = new Texture(Directories.TexBlood.FullName + Format);
            PartyBars = new Texture(Directories.TexPartyBars.FullName + Format);
        }
    }
}