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

    public static void Server_Data()
    {
        // Defini os dados das opções
        Lists.Server_Data.Game_Name = "CryBits";
        Lists.Server_Data.Welcome = "Welcome to CryBits.";
        Lists.Server_Data.Port = 7001;
        Lists.Server_Data.Max_Players = 15;
        Lists.Server_Data.Max_Characters = 3;
    }

    public static void Class(byte Index)
    {
        // Reseta os valores
        Lists.Class[Index] = new Lists.Structures.Class();
        Lists.Class[Index].Name = string.Empty;
        Lists.Class[Index].Vital = new short[(byte)Globals.Vitals.Count];
        Lists.Class[Index].Attribute = new short[(byte)Globals.Attributes.Count];
        Lists.Class[Index].Tex_Male = new List<short>();
        Lists.Class[Index].Tex_Female = new List<short>();
        Lists.Class[Index].Item = new List<System.Tuple<short, short>>();
        Lists.Class[Index].Spawn_Map = 1;
    }

    public static void NPC(short Index)
    {
        // Reseta os valores
        Lists.NPC[Index] = new Lists.Structures.NPC();
        Lists.NPC[Index].Name = string.Empty;
        Lists.NPC[Index].Vital = new short[(byte)Globals.Vitals.Count];
        Lists.NPC[Index].Attribute = new short[(byte)Globals.Attributes.Count];
        Lists.NPC[Index].Drop = new List<Lists.Structures.NPC_Drop>();
        Lists.NPC[Index].Allie = new List<short>();
    }

    public static void Item(short Index)
    {
        // Reseta os valores
        Lists.Item[Index] = new Lists.Structures.Item();
        Lists.Item[Index].Name = string.Empty;
        Lists.Item[Index].Description = string.Empty;
        Lists.Item[Index].Potion_Vital = new short[(byte)Globals.Vitals.Count];
        Lists.Item[Index].Equip_Attribute = new short[(byte)Globals.Attributes.Count];
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
        Lists.Map[Index].Link = new short[(byte)Globals.Directions.Count];
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
                Lists.Map[Index].Tile[x, y].Block = new bool[(byte)Globals.Directions.Count];
    }

    private static void Map_Layers(short Index)
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
        Lists.Tile[Index].Width = (byte)Size.Width;
        Lists.Tile[Index].Height = (byte)Size.Height;
        Lists.Tile[Index].Data = new Lists.Structures.Tile_Data[Size.Width + 1, Size.Height + 1];

        for (byte x = 0; x <= Size.Width; x++)
            for (byte y = 0; y <= Size.Height; y++)
            {
                Lists.Tile[Index].Data[x, y] = new Lists.Structures.Tile_Data();
                Lists.Tile[Index].Data[x, y].Block = new bool[(byte)Globals.Directions.Count];
            }
    }

    public static void Sprite(short Index)
    {
        // Reseta os valores
        Lists.Sprite[Index] = new Lists.Structures.Sprite();
        Lists.Sprite[Index].Frame_Width = 32;
        Lists.Sprite[Index].Frame_Height = 32;
        Lists.Sprite[Index].Movement = new Lists.Structures.Sprite_Movement[(byte)Globals.Movements.Count];
        for (byte i = 0; i < (byte)Globals.Movements.Count; i++)
        {
            Lists.Sprite[Index].Movement[i] = new Lists.Structures.Sprite_Movement();
            Lists.Sprite[Index].Movement[i].Direction = new Lists.Structures.Sprite_Movement_Direction[(byte)Globals.Directions.Count];
            for (byte n = 0; n < (byte)Globals.Directions.Count; n++)
            {
                Lists.Sprite[Index].Movement[i].Direction[n] = new Lists.Structures.Sprite_Movement_Direction();
                Lists.Sprite[Index].Movement[i].Direction[n].Frames = 1;
            }
        }
    }
}