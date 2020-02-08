using System;
using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Server_Data Server_Data = new Structures.Server_Data();
    public static Structures.Player[] Player;
    public static Structures.TempPlayer[] Temp_Player;
    public static Structures.Class[] Class;
    public static Structures.Map[] Map;
    public static Structures.Temp_Map[] Temp_Map;
    public static Structures.NPC[] NPC;
    public static Structures.Item[] Item;
    public static Structures.Tile[] Tile;

    // Estrutura dos itens em gerais
    public class Structures
    {
        [Serializable]
        public struct Server_Data
        {
            public string Game_Name;
            public string Welcome;
            public short Port;
            public byte Max_Players;
            public byte Max_Characters;
            public byte Num_Classes;
            public short Num_Maps;
            public byte Num_Tiles;
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
            public bool InEditor;
            public string Party_Invitation;
        }

        [Serializable]
        public class Class
        {
            public string Name;
            public string Description;
            public short[] Tex_Male;
            public short[] Tex_Female;
            public short Spawn_Map;
            public byte Spawn_Direction;
            public byte Spawn_X;
            public byte Spawn_Y;
            public short[] Vital;
            public short[] Attribute;
            public Tuple<short, short>[] Item;
        }

        [Serializable]
        public struct Map
        {
            public short Revision;
            public List<Map_Layer> Layer;
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
        }

        public struct Temp_Map
        {
            // Temporário
            public Map_NPCs[] NPC;
            public List<Map_Items> Item;
        }

        [Serializable]
        public struct Map_Tile
        {
            public byte Attribute;
            public short Data_1;
            public short Data_2;
            public short Data_3;
            public short Data_4;
            public byte Zone;
            public bool[] Block;
        }

        [Serializable]
        public class Map_Layer
        {
            public string Name;
            public byte Type;
            public Map_Tile_Data[,] Tile;
        }

        [Serializable]
        public struct Map_NPC
        {
            public short Index;
            public byte Zone;
            public bool Spawn;
            public byte X;
            public byte Y;
        }

        [Serializable]
        public struct Map_Tile_Data
        {
            public byte X;
            public byte Y;
            public byte Tile;
            public bool Auto;
        }

        [Serializable]
        public class Map_Light
        {
            public byte X;
            public byte Y;
            public byte Width;
            public byte Height;
        }

        [Serializable]
        public struct Map_Weather
        {
            public byte Type;
            public byte Intensity;
        }

        [Serializable]
        public struct Map_Fog
        {
            public byte Texture;
            public sbyte Speed_X;
            public sbyte Speed_Y;
            public byte Alpha;
        }

        [Serializable]
        public class NPC
        {
            public string Name;
            public string SayMsg;
            public short Texture;
            public byte Behaviour;
            public byte SpawnTime;
            public byte Sight;
            public int Experience;
            public short[] Vital;
            public short[] Attribute;
            public NPC_Drop[] Drop;
            public bool AttackNPC;
            public short[] Allie;
            public global::NPC.Movements Movement;
            public byte Flee_Helth;
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

        [Serializable]
        public class Item
        {
            // Geral
            public string Name;
            public string Description;
            public short Texture;
            public byte Type;
            public short Price;
            public bool Stackable;
            public byte Bind;
            public byte Rarity;
            // Requerimentos
            public short Req_Level;
            public byte Req_Class;
            // Poção
            public int Potion_Experience;
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

        [Serializable]
        public class NPC_Drop
        {
            // Dados
            public short Item_Num;
            public short Amount;
            public byte Chance;

            // Construtor    
            public NPC_Drop(short Item_Num, short Amount, byte Chance)
            {
                this.Item_Num = Item_Num;
                this.Amount = Amount;
                this.Chance = Chance;
            }
        }

        public struct Hotbar
        {
            public byte Type;
            public byte Slot;
        }

        [Serializable]
        public class Tile
        {
            public byte Width;
            public byte Height;
            public Tile_Data[,] Data;
        }

        [Serializable]
        public class Tile_Data
        {
            public byte Attribute;
            public bool[] Block;
        }
    }
}