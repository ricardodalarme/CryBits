﻿using System;
using System.IO;
using CryBits.Editors.Media;
using CryBits.Editors.Media.Audio;

namespace CryBits.Editors.Library
{
    internal static class Directories
    {
        // Formato de todos os arquivos de dados
        public const string Format = ".dat";

        // Editor
        public static readonly FileInfo Options = new FileInfo(Environment.CurrentDirectory + @"\Data\Editors Options" + Format);

        // Cliente
        public static readonly DirectoryInfo Fonts = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Fonts\");
        public static readonly DirectoryInfo Sounds = new DirectoryInfo(Environment.CurrentDirectory + @"\Audio\Sounds\");
        public static readonly DirectoryInfo Musics = new DirectoryInfo(Environment.CurrentDirectory + @"\Audio\Musics\");
        public static readonly DirectoryInfo Tiles = new DirectoryInfo(Environment.CurrentDirectory + @"\Data\Tiles\");
        public static readonly FileInfo Tools = new FileInfo(Environment.CurrentDirectory + @"\Data\Tools" + Format);
        public static readonly DirectoryInfo TexPanels = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Interface\Panels\");
        public static readonly FileInfo TexCheckBox = new FileInfo(Environment.CurrentDirectory + @"\Graphics\Interface\CheckBox");
        public static readonly FileInfo TexTextBox = new FileInfo(Environment.CurrentDirectory + @"\Graphics\Interface\TextBox");
        public static readonly DirectoryInfo TexButtons = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Interface\Buttons\");
        public static readonly DirectoryInfo TexCharacters = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Characters\");
        public static readonly DirectoryInfo TexFaces = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Faces\");
        public static readonly DirectoryInfo TexPanoramas = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Panoramas\");
        public static readonly DirectoryInfo TexFogs = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Fogs\");
        public static readonly DirectoryInfo TexTiles = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Tiles\");
        public static readonly DirectoryInfo TexItems = new DirectoryInfo(Environment.CurrentDirectory + @"\Graphics\Items\");
        public static readonly FileInfo TexGrid = new FileInfo(Environment.CurrentDirectory + @"\Graphics\Misc\Grid");
        public static readonly FileInfo TexWeather = new FileInfo(Environment.CurrentDirectory + @"\Graphics\Misc\Weather");
        public static readonly FileInfo TexBlanc = new FileInfo(Environment.CurrentDirectory + @"\Graphics\Misc\Blank");
        public static readonly FileInfo TexDirections = new FileInfo(Environment.CurrentDirectory + @"\Graphics\Misc\Directions");
        public static readonly FileInfo TexTransparent = new FileInfo(Environment.CurrentDirectory + @"\Graphics\Misc\Transparent");
        public static readonly FileInfo TexLighting = new FileInfo(Environment.CurrentDirectory + @"\Graphics\Misc\Lighting");

        public static void Create()
        {
            // Cria os diretórios
            Fonts.Create();
            Sounds.Create();
            Musics.Create();
            Tiles.Create();
            Tools.Directory.Create();
            TexPanoramas.Create();
            TexFogs.Create();
            TexCharacters.Create();
            TexFaces.Create();
            TexPanels.Create();
            TexButtons.Create();
            TexCheckBox.Directory.Create();
            TexTextBox.Directory.Create();
            TexTiles.Create();
            TexGrid.Directory.Create();
            TexWeather.Directory.Create();
            TexBlanc.Directory.Create();
            TexDirections.Directory.Create();
            TexTransparent.Directory.Create();
            TexLighting.Directory.Create();
            TexItems.Create();

            // Lê os dados do lado do cliente
            Graphics.Init();
            Sound.Load();
            Read.Tiles();
            Read.Tools();
        }
    }
}