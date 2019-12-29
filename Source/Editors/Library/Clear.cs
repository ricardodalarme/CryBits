using System.Collections.Generic;
using System.Drawing;

class Clear
{
    public static void Options()
    {
        // Defini os dados das opções
        Lists.Options.Directory_Client = string.Empty;
        Lists.Options.Directory_Server = string.Empty;

        // Salva o que foi modificado
        Write.Options();
    }

    public static void Client_Data()
    {
        // Defini os dados das opções
        Lists.Client_Data.Num_Buttons = 1;
        Lists.Client_Data.Num_TextBoxes = 1;
        Lists.Client_Data.Num_Panels = 1;
        Lists.Client_Data.Num_CheckBoxes = 1;
    }

    public static void Server_Data()
    {
        // Defini os dados das opções
        Lists.Server_Data.Game_Name = "CryBits";
        Lists.Server_Data.Welcome = "Welcome to CryBits.";
        Lists.Server_Data.Port = 7001;
        Lists.Server_Data.Max_Players = 15;
        Lists.Server_Data.Max_Characters = 3;
        //Lists.Server_Data.Num_Classes = 1;
        //Lists.Server_Data.Num_Maps = 1;
       // Lists.Server_Data.Num_Items = 1;
      //  Lists.Server_Data.Num_NPCs = 1;
    }

    public static void Button(byte Index)
    {
        // Limpa a estrutura
        Lists.Button[Index] = new Lists.Structures.Button();
        Lists.Button[Index].General = new Lists.Structures.Tool();

        // Reseta os valores
        Lists.Button[Index].General.Name = string.Empty;
    }

    public static void TextBox(byte Index)
    {
        // Limpa a estrutura
        Lists.TextBox[Index] = new Lists.Structures.TextBox();
        Lists.TextBox[Index].General = new Lists.Structures.Tool();

        // Reseta os valores
        Lists.TextBox[Index].General.Name = string.Empty;
    }

    public static void Panel(byte Index)
    {
        // Limpa a estrutura
        Lists.Panel[Index] = new Lists.Structures.Panel();
        Lists.Panel[Index].General = new Lists.Structures.Tool();

        // Reseta os valores
        Lists.Panel[Index].General.Name = string.Empty;
    }

    public static void CheckBox(byte Index)
    {
        // Limpa a estrutura
        Lists.CheckBox[Index] = new Lists.Structures.CheckBox();
        Lists.CheckBox[Index].General = new Lists.Structures.Tool();

        // Reseta os valores
        Lists.CheckBox[Index].General.Name = string.Empty;
        Lists.CheckBox[Index].Text = string.Empty;
    }

    public static void Class(byte Index)
    {
        // Reseta os valores
        Lists.Class[Index] = new Lists.Structures.Class();
        Lists.Class[Index].Name = string.Empty;
        Lists.Class[Index].Vital = new short[(byte)Globals.Vitals.Amount];
        Lists.Class[Index].Attribute = new short[(byte)Globals.Attributes.Amount];
        Lists.Class[Index].Spawn_Map = 1;
    }

    public static void NPC(byte Index)
    {
        // Reseta os valores
        Lists.NPC[Index] = new Lists.Structures.NPC();
        Lists.NPC[Index].Name = string.Empty;
        Lists.NPC[Index].Vital = new short[(byte)Globals.Vitals.Amount];
        Lists.NPC[Index].Attribute = new short[(byte)Globals.Attributes.Amount];
        Lists.NPC[Index].Drop = new Lists.Structures.NPC_Drop[Globals.Max_NPC_Drop];
        for (byte i = 0; i < Globals.Max_NPC_Drop; i++)
        {
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
        Lists.Item[Index].Potion_Vital = new short[(byte)Globals.Vitals.Amount];
        Lists.Item[Index].Equip_Attribute = new short[(byte)Globals.Attributes.Amount];
    }

    public static void Map(short Index)
    {
        // Reseta 
        Lists.Map[Index] = new Lists.Structures.Map();
        Lists.Map[Index].Name = string.Empty;
        Lists.Map[Index].Width = Globals.Min_Map_Width;
        Lists.Map[Index].Height = Globals.Min_Map_Height;
        Lists.Map[Index].Color = -1;
        Lists.Map[Index].Fog.Alpha = 255;
        Lists.Map[Index].Lighting = 100;

        // Redimensiona 
        Lists.Map[Index].Link = new short[(byte)Globals.Directions.Amount];
        Lists.Map[Index].Light = new List<Lists.Structures.Map_Light>();
        Lists.Map[Index].Layer = new List<Lists.Structures.Map_Layer>();
        Lists.Map[Index].Layer.Add(new Lists.Structures.Map_Layer());
        Lists.Map[Index].Layer[0].Name = "Ground";
        Map_Layers(Index);
        Lists.Map[Index].Tile = new Lists.Structures.Map_Tile[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];
        Lists.Map[Index].NPC = new List<Lists.Structures.Map_NPC>();

        // Redimensiona os bloqueios
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                Lists.Map[Index].Tile[x, y].Block = new bool[(byte)Globals.Directions.Amount];
    }

    public static void Map_Layers(short Index)
    {
        for (byte c = 0; c < Lists.Map[Index].Layer.Count; c++)
        {
            // Redimensiona os azulejos
            Lists.Map[Index].Layer[c].Tile = new Lists.Structures.Map_Tile_Data[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];
            for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                    Lists.Map[Index].Layer[c].Tile[x, y].Mini = new Point[4];
        }
    }

    public static void Tile(byte Index)
    {
        Size Texture_Size = Graphics.TSize(Graphics.Tex_Tile[Index]);
        Size Size = new Size(Texture_Size.Width / Globals.Grid - 1, Texture_Size.Height / Globals.Grid - 1);

        // Redimensiona os valores
        Lists.Tile[Index].Data = new Lists.Structures.Tile_Data[Size.Width + 1, Size.Height + 1];

        for (byte x = 0; x <= Size.Width; x++)
            for (byte y = 0; y <= Size.Height; y++)
                Lists.Tile[Index].Data[x, y].Block = new bool[(byte)Globals.Directions.Amount];
    }
}