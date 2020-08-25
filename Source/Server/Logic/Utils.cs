using System;

namespace Logic
{
    static class Utils
    {
        // Números aleatórios
        public static Random MyRandom = new Random();

        // Configurações
        public static string Game_Name = "CryBits";
        public static string Welcome_Message = "Welcome to CryBits.";
        public static short Port = 7001;
        public static byte Max_Players = 15;
        public static byte Max_Characters = 3;
        public static byte Max_Party_Members = 3;
        public static byte Max_Map_Items = 100;
        public static byte Num_Points = 3;
        public static byte Max_Name_Length = 12;
        public static byte Min_Name_Length = 3;
        public static byte Max_Password_Length = 12;
        public static byte Min_Password_Length = 3;

        // Limites fixos
        public const byte Max_Inventory = 30;
        public const byte Max_Hotbar = 10;
        public const byte Max_DirBlock = 3;

        ////////////////
        // Numerações //
        ////////////////
        public enum Directions
        {
            Up,
            Down,
            Left,
            Right,
            Count
        }

        public enum Accesses
        {
            None,
            Moderator,
            Editor,
            Administrator
        }

        public enum Vitals
        {
            HP,
            MP,
            Count
        }

        public enum Attributes
        {
            Strength,
            Resistance,
            Intelligence,
            Agility,
            Vitality,
            Count
        }

        public enum Messages
        {
            Game,
            Map,
            Global,
            Private
        }

        public enum Map_Morals
        {
            Pacific,
            Dangerous,
        }

        public enum NPC_Behaviour
        {
            Friendly,
            AttackOnSight,
            AttackWhenAttacked,
            ShopKeeper
        }

        public enum NPC_Movements
        {
            MoveRandomly,
            TurnRandomly,
            StandStill
        }

        public enum Tile_Attributes
        {
            None,
            Block,
            Warp,
            Item,
        }

        public enum Targets
        {
            Player = 1,
            NPC
        }

        public enum Items
        {
            None,
            Equipment,
            Potion
        }

        public enum Equipments
        {
            Weapon,
            Armor,
            Helmet,
            Shield,
            Amulet,
            Count
        }

        public enum Hotbars
        {
            None,
            Item
        }

        public enum BindOn
        {
            None,
            Pickup,
            Equip
        }

        public enum Trade_Status
        {
            Waiting,
            Confirmed,
            Accepted,
            Declined
        }

        public static Directions ReverseDirection(Directions Direction)
        {
            // Retorna a direção inversa
            switch (Direction)
            {
                case Directions.Up: return Directions.Down;
                case Directions.Down: return Directions.Up;
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                default: return Directions.Count;
            }
        }

        public static void NextTile(Directions Direction, ref byte X, ref byte Y)
        {
            // Próximo azulejo
            switch (Direction)
            {
                case Directions.Up: Y -= 1; break;
                case Directions.Down: Y += 1; break;
                case Directions.Right: X += 1; break;
                case Directions.Left: X -= 1; break;
            }
        }

        public static void Swap<T>(ref T Item1, ref T Item2)
        {
            // Troca dois elementos
            T Temp = Item1;
            Item1 = Item2;
            Item2 = Temp;
        }
    }
}