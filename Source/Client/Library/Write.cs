using System.IO;

class Write
{
    public static void Data()
    {
        // Salva todos os dados
        Options();
    }

    public static void Options()
    {
        // Cria um arquivo temporário
        BinaryWriter Data = new BinaryWriter(File.OpenWrite(Directories.Options.FullName));

        // Carrega todas as opções
        Data.Write(Lists.Options.GameName);
        Data.Write(Lists.Options.SaveUsername);
        Data.Write(Lists.Options.Sounds);
        Data.Write(Lists.Options.Musics);
        Data.Write(Lists.Options.Username);

        // Fecha o arquivo
        Data.Dispose();
    }

    public static void Map(short Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.Maps_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Map.Revision);
        Data.Write(Lists.Map.Name);
        Data.Write(Lists.Map.Width);
        Data.Write(Lists.Map.Height);
        Data.Write(Lists.Map.Moral);
        Data.Write(Lists.Map.Panorama);
        Data.Write(Lists.Map.Music);
        Data.Write(Lists.Map.Color);
        Data.Write(Lists.Map.Weather.Type);
        Data.Write(Lists.Map.Weather.Intensity);
        Data.Write(Lists.Map.Fog.Texture);
        Data.Write(Lists.Map.Fog.Speed_X);
        Data.Write(Lists.Map.Fog.Speed_Y);
        Data.Write(Lists.Map.Fog.Alpha);

        // Ligação
        for (short i = 0; i <= (short)Game.Directions.Amount - 1; i++)
            Data.Write(Lists.Map.Link[i]);

        // Azulejos
        Data.Write((byte)Lists.Map.Tile[0, 0].Data.GetUpperBound(1));
        for (byte x = 0; x <= Lists.Map.Width; x++)
            for (byte y = 0; y <= Lists.Map.Height; y++)
                for (byte c = 0; c <= (byte)global::Map.Layers.Amount - 1; c++)
                    for (byte q = 0; q <= Lists.Map.Tile[x, y].Data.GetUpperBound(1); q++)
                    {
                        Data.Write(Lists.Map.Tile[x, y].Data[c, q].x);
                        Data.Write(Lists.Map.Tile[x, y].Data[c, q].y);
                        Data.Write(Lists.Map.Tile[x, y].Data[c, q].Tile);
                        Data.Write(Lists.Map.Tile[x, y].Data[c, q].Automatic);
                    }

        // Dados específicos dos azulejos
        for (byte x = 0; x <= Lists.Map.Width; x++)
            for (byte y = 0; y <= Lists.Map.Height; y++)
            {
                Data.Write(Lists.Map.Tile[x, y].Attribute);
                for (byte i = 0; i <= (byte)Game.Directions.Amount - 1; i++)
                    Data.Write(Lists.Map.Tile[x, y].Block[i]);
            }

        // Luzes
        Data.Write(Lists.Map.Light.GetUpperBound(0));
        if (Lists.Map.Light.GetUpperBound(0) > 0)
            for (byte i = 0; i <= Lists.Map.Light.GetUpperBound(0); i++)
            {
                Data.Write(Lists.Map.Light[i].X);
                Data.Write(Lists.Map.Light[i].Y);
                Data.Write(Lists.Map.Light[i].Width);
                Data.Write(Lists.Map.Light[i].Height);
            }

        // NPCs
        Data.Write(Lists.Map.NPC.GetUpperBound(0));
        if (Lists.Map.NPC.GetUpperBound(0) > 0)
            for (byte i = 1; i <= Lists.Map.NPC.GetUpperBound(0); i++)
                Data.Write(Lists.Map.NPC[i]);

        // Fecha o sistema
        Data.Dispose();
    }
}