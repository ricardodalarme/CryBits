using System;

class Clear
{
    public static void All()
    {
        // Limpa todos os dados necessários
        Players();
    }

    public static void Players()
    {
        // Redimensiona a lista
        Lists.Player = new Lists.Structures.Player[Lists.Server_Data.Max_Players + 1];
        Lists.TempPlayer = new Lists.Structures.TempPlayer[Lists.Server_Data.Max_Players + 1];

        // Limpa os dados de todos jogadores
        for (byte i = 1; i <= Lists.Server_Data.Max_Players; i++)
            Player(i);
    }

    public static void Player(byte Index)
    {
        // Limpa os dados do jogador
        Lists.Player[Index] = new Lists.Structures.Player();
        Lists.TempPlayer[Index] = new Lists.Structures.TempPlayer();
        Lists.Player[Index].User = string.Empty;
        Lists.Player[Index].Password = string.Empty;
        Lists.Player[Index].Character = new Player.Character_Structure[Lists.Server_Data.Max_Characters + 1];

        // Limpa os dados do personagem
        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
            Player_Character(Index, i);
    }

    public static void Player_Character(byte Index, byte Char_Num)
    {
        // Limpa os dados
        Lists.Player[Index].Character[Char_Num] = new Player.Character_Structure();
        Lists.Player[Index].Character[Char_Num].Index = Index;
        Lists.Player[Index].Character[Char_Num].Inventory = new Lists.Structures.Inventories[Game.Max_Inventory + 1];
        Lists.Player[Index].Character[Char_Num].Equipment = new short[(byte)Game.Equipments.Count];
        Lists.Player[Index].Character[Char_Num].Hotbar = new Lists.Structures.Hotbar[Game.Max_Hotbar + 1];
    }

    public static void Server_Data()
    {
        // Defini os dados das opções
        Lists.Server_Data.Game_Name = "CryBits";
        Lists.Server_Data.Welcome = "Welcome to CryBits.";
        Lists.Server_Data.Port = 7001;
        Lists.Server_Data.Max_Players = 15;
        Lists.Server_Data.Max_Characters = 3;
        Lists.Server_Data.Num_Classes = 1;
        Lists.Server_Data.Num_Maps = 1;
        Lists.Server_Data.Num_Items = 1;
        Lists.Server_Data.Num_NPCs = 1;
        Lists.Server_Data.Num_Tiles = 0;
    }

    public static void Class(byte Index)
    {
        // Reseta os valores
        Lists.Class[Index] = new Lists.Structures.Class();
        Lists.Class[Index].Name = string.Empty;
        Lists.Class[Index].Vital = new short[(byte)Game.Vitals.Count];
        Lists.Class[Index].Attribute = new short[(byte)Game.Attributes.Count];
        Lists.Class[Index].Tex_Male = new short[0];
        Lists.Class[Index].Tex_Female = new short[0];
        Lists.Class[Index].Item = new short[0];
        Lists.Class[Index].Spawn_Map = 1;
    }

    public static void NPC(byte Index)
    {
        // Reseta os valores
        Lists.NPC[Index] = new Lists.Structures.NPC();
        Lists.NPC[Index].Name = string.Empty;
        Lists.NPC[Index].Vital = new short[(byte)Game.Vitals.Count];
        Lists.NPC[Index].Attribute = new short[(byte)Game.Attributes.Count];
        Lists.NPC[Index].Drop = new Lists.Structures.NPC_Drop[Game.Max_NPC_Drop];
        for (byte i = 0; i < Game.Max_NPC_Drop; i++)
        {
            Lists.NPC[Index].Drop[i] = new Lists.Structures.NPC_Drop();
            Lists.NPC[Index].Drop[i].Chance = 100;
            Lists.NPC[Index].Drop[i].Amount = 1;
        }
    }

    public static void Item(byte Index)
    {
        // Reseta os valores
        Lists.Item[Index] = new Lists.Structures.Item();
        Lists.Item[Index].Name = string.Empty;
        Lists.Item[Index].Description = string.Empty;
        Lists.Item[Index].Potion_Vital = new short[(byte)Game.Vitals.Count];
        Lists.Item[Index].Equip_Attribute = new short[(byte)Game.Attributes.Count];
    }

    public static void Map(short Index)
    {
        // Reseta os valores
        Lists.Map[Index] = new Lists.Structures.Map();
        Lists.Map[Index].Name = string.Empty;
        Lists.Map[Index].Width = Game.Min_Map_Width;
        Lists.Map[Index].Height = Game.Min_Map_Height;
        Lists.Map[Index].Color = -1;
        Lists.Map[Index].Fog.Alpha = 255;
        Lists.Map[Index].Lighting = 100;
        Lists.Map[Index].Link = new short[(byte)Game.Directions.Count];
        Lists.Map[Index].Light = Array.Empty<Lists.Structures.Map_Light>();
        Lists.Map[Index].Layer = new System.Collections.Generic.List<Lists.Structures.Map_Layer>();
        Lists.Map[Index].Layer.Add(new Lists.Structures.Map_Layer());
        Lists.Map[Index].Layer[0].Name = "Ground";
        for (byte c = 0; c < Lists.Map[Index].Layer.Count; c++) Lists.Map[Index].Layer[c].Tile = new Lists.Structures.Map_Tile_Data[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];
        Lists.Map[Index].Tile = new Lists.Structures.Map_Tile[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];
        Lists.Map[Index].NPC = Array.Empty<Lists.Structures.Map_NPC>();

        // Redimensiona os bloqueios
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                Lists.Map[Index].Tile[x, y].Block = new bool[(byte)Game.Directions.Count];

        // Dados temporários
        Lists.Temp_Map[Index] = new Lists.Structures.Temp_Map();
        Lists.Temp_Map[Index].NPC = Array.Empty<Lists.Structures.Map_NPCs>();
        Lists.Temp_Map[Index].Item = new System.Collections.Generic.List<Lists.Structures.Map_Items>();
        Lists.Temp_Map[Index].Item.Add(new Lists.Structures.Map_Items());
    }

    public static void Tile(byte Index)
    {
        // Redimensiona os valores
        Lists.Tile[Index] = new Lists.Structures.Tile();
        Lists.Tile[Index].Width = 0;
        Lists.Tile[Index].Height = 0;
        Lists.Tile[Index].Data = new Lists.Structures.Tile_Data[1, 1];
        Lists.Tile[Index].Data[0, 0] = new Lists.Structures.Tile_Data();
        Lists.Tile[Index].Data[0, 0].Block = new bool[(byte)Game.Directions.Count];
    }
}