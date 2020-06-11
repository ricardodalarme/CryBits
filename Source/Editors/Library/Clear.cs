using Objects;
using System.Drawing;
using static Utils;

namespace Library
{
    static class Clear
    {
        public static void Options()
        {
            // Defini os dados das opções
            Lists.Options.Directory_Client = string.Empty;

            // Salva o que foi modificado
            Write.Options();
        }

        public static void Server_Data()
        {
            // Defini os dados das opções
            Lists.Server_Data.Game_Name = "CryBits";
            Lists.Server_Data.Welcome = "Welcome to CryBits.";
            Lists.Server_Data.Port = 7001;
            Lists.Server_Data.Max_Players = 15;
            Lists.Server_Data.Max_Characters = 3;
        }

        public static void Tile(byte Index)
        {
            Size Texture_Size = Graphics.TSize(Graphics.Tex_Tile[Index]);
            Size Size = new Size(Texture_Size.Width / Grid - 1, Texture_Size.Height / Grid - 1);

            // Redimensiona os valores
            Lists.Tile[Index] = new Tile();
            Lists.Tile[Index].Width = (byte)Size.Width;
            Lists.Tile[Index].Height = (byte)Size.Height;
            Lists.Tile[Index].Data = new Tile_Data[Size.Width + 1, Size.Height + 1];

            for (byte x = 0; x <= Size.Width; x++)
                for (byte y = 0; y <= Size.Height; y++)
                {
                    Lists.Tile[Index].Data[x, y] = new Tile_Data();
                    Lists.Tile[Index].Data[x, y].Block = new bool[(byte)Directions.Count];
                }
        }
    }
}