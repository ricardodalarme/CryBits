using System;
using System.Collections.Generic;

namespace Objects
{
    class TMap
    {
        // Lista de dados
        public static Dictionary<Guid, TMap> List;

        // Dados gerais
        public Map Data;
        public TNPC[] NPC;
        public TMap_Items[] Item = new TMap_Items[0];
        public List<TMap_Blood> Blood = new List<TMap_Blood>();

        public TMap(Map Data)
        {
            this.Data = Data;
        }

        private bool HasNPC(byte X, byte Y)
        {
            // Verifica se há algum npc na cordenada
            for (byte i = 0; i < NPC.Length; i++)
                if (NPC[i].Data != null)
                    if ((NPC[i].X, NPC[i].Y) == (X, Y))
                        return true;

            return false;
        }

        private bool HasPlayer(short X, short Y)
        {
            // Verifica se há algum Jogador na cordenada
            for (byte i = 0; i < Player.List.Count; i++)
                if ((Player.List[i].X, Player.List[i].Y, Player.List[i].Map) == (X, Y, this))
                    return true;

            return false;
        }

        public bool Tile_Blocked(byte X, byte Y, Game.Directions Direction)
        {
            byte Next_X = X, Next_Y = Y;

            // Próximo azulejo
            Mapper.NextTile(Direction, ref Next_X, ref Next_Y);

            // Verifica se está indo para uma ligação
            if (Mapper.OutOfLimit(Next_X, Next_Y)) return Data.Link[(byte)Direction] == 0;

            // Verifica se o azulejo está bloqueado
            if (Data.Tile[Next_X, Next_Y].Attribute == (byte)Mapper.Layer_Attributes.Block) return true;
            if (Data.Tile[Next_X, Next_Y].Block[(byte)Game.ReverseDirection(Direction)]) return true;
            if (Data.Tile[X, Y].Block[(byte)Direction]) return true;
            if (HasPlayer(Next_X, Next_Y) || HasNPC(Next_X, Next_Y)) return true;
            return false;
        }
    }

    class TMap_Items
    {
        public Item Item;
        public byte X;
        public byte Y;
    }

    class TMap_Blood
    {
        // Dados
        public byte Texture_Num;
        public short X;
        public short Y;
        public byte Opacity;

        // Construtor
        public TMap_Blood(byte Texture_Num, short X, short Y, byte Opacity)
        {
            this.Texture_Num = Texture_Num;
            this.X = X;
            this.Y = Y;
            this.Opacity = Opacity;
        }
    }
}