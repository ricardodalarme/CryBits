using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static Structures.Player[] Player;
    public static Structures.TempPlayer[] TempPlayer;
    public static Structures.Classes[] Class;
    public static Structures.Maps[] Map;
    public static Structures.NPCs[] NPC;
    public static Structures.Items[] Item;

    // Estrutura dos itens em gerais
    public class Structures
    {
        public struct Server_Data
        {
            public string Game_Name;
            public string Welcome;
            public short Port;
            public byte Max_Players;
            public byte Max_Characters;
            public byte Num_Classes;
            public short Num_Maps;
            public short Num_NPCs;
            public short Num_Items;
        }

        public struct Player
        {
            public string User;
            public string Password;
            public Game.Accesses Acess;
            public global::Player.Character_Structure[] Character;
        }

        public struct TempPlayer
        {
            public bool Playing;
            public byte Using;
            public bool GettingMap;
        }

        public struct Classes
        {
            public string Name;
            public short Texture_Male;
            public short Texture_Female;
            public short Spawn_Map;
            public byte Spawn_Direction;
            public byte Spawn_X;
            public byte Spawn_Y;
            public short[] Vital;
            public short[] Attribute;
        }

        public struct Maps
        {
            public short Revision;
            public Map_Tile[,] Tile;
            public string Name;
            public byte Width;
            public byte Height;
            public byte Moral;
            public byte Panorama;
            public byte Music;
            public int Color;
            public Map_Weather Weather;
            public Map_Fog Fog;
            public short[] Link;
            public byte Light_Global;
            public byte Lighting;
            public Map_Light[] Light;
            public Map_NPC[] NPC;

            // Temporário
            public Map_NPCs[] Temp_NPC;
            public List<Map_Items> Temp_Item;
        }

        public struct Map_NPC
        {
            public short Index;
            public byte Zone;
            public bool Spawn;
            public byte X;
            public byte Y;
        }

        public struct Map_Tile
        {
            public byte Zone;
            public byte Attribute;
            public short Data_1;
            public short Data_2;
            public short Data_3;
            public short Data_4;
            public bool[] Block;
            public Map_Tile_Data[,] Data;
        }

        public struct Map_Tile_Data
        {
            public byte X;
            public byte Y;
            public byte Tile;
            public bool Automatic;
        }

        public class Map_Light
        {
            public byte X;
            public byte Y;
            public byte Width;
            public byte Height;
        }

        public struct Map_Weather
        {
            public byte Type;
            public byte Intensity;
        }

        public struct Map_Fog
        {
            public byte Texture;
            public sbyte Speed_X;
            public sbyte Speed_Y;
            public byte Alpha;
        }

        public struct NPCs
        {
            public string Name;
            public short Texture;
            public byte Behaviour;
            public byte SpawnTime;
            public byte Sight;
            public byte Experience;
            public short[] Vital;
            public short[] Attribute;
            public NPC_Drop[] Drop;
        }

        public struct Map_NPCs
        {
            public short Index;
            public byte X;
            public byte Y;
            public Game.Directions Direction;
            public byte Target_Type;
            public byte Target_Index;
            public short[] Vital;
            public int Spawn_Timer;
            public int Attack_Timer;
        }

        public struct Map_Items
        {
            public short Index;
            public byte X;
            public byte Y;
            public short Amount;
        }

        public struct Items
        {
            // Geral
            public string Name;
            public string Description;
            public short Texture;
            public byte Type;
            public short Price;
            public bool Stackable;
            public bool Bind;
            // Requerimentos
            public short Req_Level;
            public byte Req_Classe;
            // Poção
            public short Potion_Experience;
            public short[] Potion_Vital;
            // Equipamento
            public byte Equip_Type;
            public short[] Equip_Attribute;
            public short Weapon_Damage;
        }

        public struct Inventories
        {
            public short Item_Num;
            public short Amount;
        }

        public struct NPC_Drop
        {
            public short Item_Num;
            public short Amount;
            public byte Chance;
        }

        public struct Hotbar
        {
            public byte Type;
            public byte Slot;
        }
    }
}