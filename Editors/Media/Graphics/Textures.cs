using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CryBits.Editors.Library;
using SFML.Graphics;

namespace CryBits.Editors.Media.Graphics
{
    internal static class Textures
    {
        // Texturas
        public static List<Texture> Characters;
        public static List<Texture> Tiles;
        public static List<Texture> Panels;
        public static List<Texture> Buttons;
        public static List<Texture> Panoramas;
        public static List<Texture> Fogs;
        public static List<Texture> Items;
        public static Texture CheckBox;
        public static Texture TextBox;
        public static Texture Grid;
        public static Texture Weather;
        public static Texture Blank;
        public static Texture Directions;
        public static Texture Transparent;
        public static Texture Lighting;

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
            Panels = LoadTextures(Directories.TexPanels.FullName);
            Buttons = LoadTextures(Directories.TexButtons.FullName);
            Panoramas = LoadTextures(Directories.TexPanoramas.FullName);
            Fogs = LoadTextures(Directories.TexFogs.FullName);
            Items = LoadTextures(Directories.TexItems.FullName);

            // Únicas
            Weather = new Texture(Directories.TexWeather.FullName + Format);
            Blank = new Texture(Directories.TexBlanc.FullName + Format);
            Directions = new Texture(Directories.TexDirections.FullName + Format);
            Transparent = new Texture(Directories.TexTransparent.FullName + Format);
            Grid = new Texture(Directories.TexGrid.FullName + Format);
            CheckBox = new Texture(Directories.TexCheckBox.FullName + Format);
            TextBox = new Texture(Directories.TexTextBox.FullName + Format);
            Lighting = new Texture(Directories.TexLighting.FullName + Format);
        }

        public static Size ToSize(this Texture texture)
        {
            // Retorna com o tamanho da textura
            return texture != null ? new Size((int)texture.Size.X, (int)texture.Size.Y) : new Size(0, 0);
        }
    }
}