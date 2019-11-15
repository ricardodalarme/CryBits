using System;
using System.IO;

class Map
{
    ////////////////
    // Numerações //
    ////////////////
    public enum Morals
    {
        Pacific,
        Dangerous,
        Amount
    }

    public enum Layers
    {
        Ground,
        Fringe,
        Amount
    }

    public enum Attributes
    {
        None,
        Block,
        Warp,
        Item,
        Amount
    }

    public static void Logic()
    {
        for (byte i = 1; i <= Lists.Map.GetUpperBound(0); i++)
        {
            // Não é necessário fazer todos os cálculos se não houver nenhum jogador no mapa
            if (!HasPlayers(i)) continue;

            // Lógica dos NPCs
            NPC.Logic(i);

            // Faz reaparecer todos os itens do mapa
            if (Environment.TickCount > Loop.Timer_Map_Items + 300000)
            {
                Lists.Map[i].Temp_Item = new System.Collections.Generic.List<Lists.Structures.Map_Items>();
                Spawn_Items(i);
                Send.Map_Items(i);
            }
        }

        // Reseta as contagens
        if (Environment.TickCount > Loop.Timer_NPC_Regen + 5000) Loop.Timer_NPC_Regen = Environment.TickCount;
        if (Environment.TickCount > Loop.Timer_Map_Items + 300000) Loop.Timer_Map_Items = Environment.TickCount;
    }

    public static byte HasNPC(short Map_Num, short X, short Y)
    {
        // Verifica se há algum npc na cordenada
        for (byte i = 1; i <= Lists.Map[Map_Num].Temp_NPC.GetUpperBound(0); i++)
            if (Lists.Map[Map_Num].Temp_NPC[i].Index > 0)
                if (Lists.Map[Map_Num].Temp_NPC[i].X == X && Lists.Map[Map_Num].Temp_NPC[i].Y == Y)
                    return i;

        return 0;
    }

    public static byte HasPlayer(short Map_Num, short X, short Y)
    {
        // Verifica se há algum Jogador na cordenada
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Player.IsPlaying(i))
                if (Player.Character(i).X == X && Player.Character(i).Y == Y && Player.Character(i).Map == Map_Num)
                    return i;

        return 0;
    }

    public static bool HasPlayers(short Map_Num)
    {
        // Verifica se tem algum jogador no mapa
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Player.IsPlaying(i))
                if (Player.Character(i).Map == Map_Num)
                    return true;

        return false;
    }

    public static byte HasItem(short Map_Num, byte X, byte Y)
    {
        // Verifica se tem algum item nas coordenadas 
        for (byte i = (byte)(Lists.Map[Map_Num].Temp_Item.Count - 1); i >= 1; i--)
            if (Lists.Map[Map_Num].Temp_Item[i].X == X && Lists.Map[Map_Num].Temp_Item[i].Y == Y)
                return i;

        return 0;
    }

    public static bool OutLimit(short Map_Num, short X, short Y)
    {
        // Verifica se as coordenas estão no limite do mapa
        if (X > Lists.Map[Map_Num].Width || Y > Lists.Map[Map_Num].Height || X < 0 || Y < 0)
            return true;
        else
            return false;
    }

    public static void NextTile(Game.Directions Direction, ref short X, ref short Y)
    {
        // Próximo azulejo
        switch (Direction)
        {
            case Game.Directions.Up: Y -= 1; break;
            case Game.Directions.Down: Y += 1; break;
            case Game.Directions.Right: X += 1; break;
            case Game.Directions.Left: X -= 1; break;
        }
    }

    public static bool Tile_Blocked(short Map_Num, short X, short Y)
    {
        // Verifica se o azulejo está bloqueado
        if (OutLimit(Map_Num, X, Y))
            return true;
        else if (Lists.Map[Map_Num].Tile[X, Y].Attribute == (byte)Attributes.Block)
            return true;

        return false;
    }

    public static bool Tile_Blocked(short Map_Num, short X, short Y, Game.Directions Direction, bool CountEntities = true)
    {
        short Next_X = X, Next_Y = Y;

        // Próximo azulejo
        NextTile(Direction, ref Next_X, ref Next_Y);

        // Verifica se o azulejo está bloqueado
        if (Tile_Blocked(Map_Num, (byte)Next_X, (byte)Next_Y))
            return true;
        else if (Lists.Map[Map_Num].Tile[Next_X, Next_Y].Block[(byte)Game.ReverseDirection(Direction)])
            return true;
        else if (Lists.Map[Map_Num].Tile[X, Y].Block[(byte)Direction])
            return true;
        else if (CountEntities && (HasPlayer(Map_Num, Next_X, Next_Y) > 0 || HasNPC(Map_Num, Next_X, Next_Y) > 0))
            return true;

        return false;
    }

    public static void Spawn_Items(short Map_Num)
    {
        Lists.Structures.Maps Data = Lists.Map[Map_Num];
        Lists.Structures.Map_Items Item = new Lists.Structures.Map_Items();

        // Verifica se tem algum atributo de item no mapa
        for (byte x = 0; x <= Data.Width; x++)
            for (byte y = 0; y <= Data.Height; y++)
                if (Data.Tile[x, y].Attribute == (byte)Attributes.Item)
                {
                    // Faz o item aparecer
                    Item.Index = Data.Tile[x, y].Data_1;
                    Item.Amount = Data.Tile[x, y].Data_2;
                    Item.X = x;
                    Item.Y = y;
                    Lists.Map[Map_Num].Temp_Item.Add(Item);
                }
    }
}

partial class Read
{
    public static void Maps()
    {
        Lists.Map = new Lists.Structures.Maps[Lists.Server_Data.Num_Maps + 1];

        // Lê os dados
        for (short i = 1; i <= Lists.Map.GetUpperBound(0); i++)
            Map(i);
    }

    public static void Map(short Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(new FileInfo(Directories.Maps.FullName + Index + Directories.Format).OpenRead());

        // Lê os dados
        Lists.Map[Index].Revision = Data.ReadInt16();
        Lists.Map[Index].Name = Data.ReadString();
        Lists.Map[Index].Width = Data.ReadByte();
        Lists.Map[Index].Height = Data.ReadByte();
        Lists.Map[Index].Moral = Data.ReadByte();
        Lists.Map[Index].Panorama = Data.ReadByte();
        Lists.Map[Index].Music = Data.ReadByte();
        Lists.Map[Index].Color = Data.ReadInt32();
        Lists.Map[Index].Weather.Type = Data.ReadByte();
        Lists.Map[Index].Weather.Intensity = Data.ReadByte();
        Lists.Map[Index].Fog.Texture = Data.ReadByte();
        Lists.Map[Index].Fog.Speed_X = Data.ReadSByte();
        Lists.Map[Index].Fog.Speed_Y = Data.ReadSByte();
        Lists.Map[Index].Fog.Alpha = Data.ReadByte();
        Lists.Map[Index].Light_Global = Data.ReadByte();
        Lists.Map[Index].Lighting = Data.ReadByte();

        // Ligações
        Lists.Map[Index].Link = new short[(byte)Game.Directions.Amount];
        for (short i = 0; i <= (short)Game.Directions.Amount - 1; i++)
            Lists.Map[Index].Link[i] = Data.ReadInt16();

        // Azulejos
        Map_Tile(Index, Data);

        // Dados específicos dos azulejos
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
            {
                Lists.Map[Index].Tile[x, y].Attribute = Data.ReadByte();
                Lists.Map[Index].Tile[x, y].Data_1 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_2 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_3 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_4 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Zone = Data.ReadByte();

                // Bloqueio direcional
                Lists.Map[Index].Tile[x, y].Block = new bool[(byte)Game.Directions.Amount];
                for (byte i = 0; i <= (byte)Game.Directions.Amount - 1; i++)
                    Lists.Map[Index].Tile[x, y].Block[i] = Data.ReadBoolean();
            }

        // Luzes
        Lists.Map[Index].Light = new Lists.Structures.Map_Light[Data.ReadByte()];
        if (Lists.Map[Index].Light.GetUpperBound(0) > 0)
            for (byte i = 0; i <= Lists.Map[Index].Light.GetUpperBound(0); i++)
            {
                Lists.Map[Index].Light[i].X = Data.ReadByte();
                Lists.Map[Index].Light[i].Y = Data.ReadByte();
                Lists.Map[Index].Light[i].Width = Data.ReadByte();
                Lists.Map[Index].Light[i].Height = Data.ReadByte();
            }

        // NPCs
        Lists.Map[Index].NPC = new Lists.Structures.Map_NPC[Data.ReadByte() + 1];
        Lists.Map[Index].Temp_NPC = new Lists.Structures.Map_NPCs[Lists.Map[Index].NPC.GetUpperBound(0) + 1];
        if (Lists.Map[Index].NPC.GetUpperBound(0) > 0)
            for (byte i = 1; i <= Lists.Map[Index].NPC.GetUpperBound(0); i++)
            {
                Lists.Map[Index].NPC[i].Index = Data.ReadInt16();
                Lists.Map[Index].NPC[i].Zone = Data.ReadByte();
                Lists.Map[Index].NPC[i].Spawn = Data.ReadBoolean();
                Lists.Map[Index].NPC[i].X = Data.ReadByte();
                Lists.Map[Index].NPC[i].Y = Data.ReadByte();
                global::NPC.Spawn(i, Index);
            }

        // Items
        Lists.Map[Index].Temp_Item = new System.Collections.Generic.List<Lists.Structures.Map_Items>();
        Lists.Map[Index].Temp_Item.Add(new Lists.Structures.Map_Items()); // Nulo
        global::Map.Spawn_Items(Index);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Map_Tile(short Index, BinaryReader Binário)
    {
        byte Num_Layers = Binário.ReadByte();

        // Redimensiona os dados
        Lists.Map[Index].Tile = new Lists.Structures.Map_Tile[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                Lists.Map[Index].Tile[x, y].Data = new Lists.Structures.Map_Tile_Data[(byte)global::Map.Layers.Amount, Num_Layers + 1];

        // Lê os azulejos
        for (byte i = 0; i <= Num_Layers; i++)
        {
            // Dados básicos
            Binário.ReadString(); // Name
            byte t = Binário.ReadByte(); // Tipo

            // Azulejos
            for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                {
                    Lists.Map[Index].Tile[x, y].Data[t, i].X = Binário.ReadByte();
                    Lists.Map[Index].Tile[x, y].Data[t, i].Y = Binário.ReadByte();
                    Lists.Map[Index].Tile[x, y].Data[t, i].Tile = Binário.ReadByte();
                    Lists.Map[Index].Tile[x, y].Data[t, i].Automatic = Binário.ReadBoolean();
                }
        }
    }
}