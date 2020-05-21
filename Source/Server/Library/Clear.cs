using System;
using System.Collections.Generic;

class Clear
{
    public static void Server_Data()
    {
        // Defini os dados das opções
        Lists.Server_Data.Game_Name = "CryBits";
        Lists.Server_Data.Welcome = "Welcome to CryBits.";
        Lists.Server_Data.Port = 7001;
        Lists.Server_Data.Max_Players = 15;
        Lists.Server_Data.Max_Characters = 3;
        Lists.Server_Data.Max_Party_Members = 3;
        Lists.Server_Data.Max_Map_Items = 100;
        Lists.Server_Data.Num_Points = 3;
        Lists.Server_Data.Num_Maps = 1;
        Lists.Server_Data.Num_NPCs = 1;
    }

    public static void NPC(short Index)
    {
        // Reseta os valores
        Lists.NPC[Index] = new Objects.NPC();
        Lists.NPC[Index].Name = string.Empty;
        Lists.NPC[Index].Vital = new short[(byte)Game.Vitals.Count];
        Lists.NPC[Index].Attribute = new short[(byte)Game.Attributes.Count];
        Lists.NPC[Index].Drop = Array.Empty<Objects.NPC_Drop>();
        Lists.NPC[Index].Allie = Array.Empty<short>();
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
        Lists.Map[Index].Layer = new List<Lists.Structures.Map_Layer>();
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
        Lists.Temp_Map[Index].NPC = Array.Empty<NPC.Structure>();
        Lists.Temp_Map[Index].Item = new List<Lists.Structures.Map_Items>();
        Lists.Temp_Map[Index].Item.Add(new Lists.Structures.Map_Items());
    }
}