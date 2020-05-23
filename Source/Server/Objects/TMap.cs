using System;
using System.Collections.Generic;

namespace Objects
{
    class TMap : Lists.Structures.Data
    {
        // Dados
        public Map Data;
        public TNPC[] NPC = Array.Empty<TNPC>();
        public List<TMap_Items> Item = new List<TMap_Items>();

        // Construtor
        public TMap(Guid ID, Map Map) : base(ID)
        {
            Data = Map;
        }

        public void Logic()
        {
            // Não é necessário fazer todos os cálculos se não houver nenhum jogador no mapa
            if (!HasPlayers()) return;

            // Lógica dos NPCs
            for (byte j = 1; j < NPC.Length; j++) NPC[j].Logic();

            // Faz reaparecer todos os itens do mapa
            if (Environment.TickCount > Loop.Timer_Map_Items + 300000)
            {
                Item = new List<TMap_Items>();
                Spawn_Items();
                Send.Map_Items(this);
            }
        }

        public TNPC HasNPC(short X, short Y)
        {
            // Verifica se há algum npc na cordenada
            for (byte i = 1; i < NPC.Length; i++)
                if (NPC[i].Alive)
                    if (NPC[i].X == X && NPC[i].Y == Y)
                        return NPC[i];

            return null;
        }

        public Player HasPlayer(short X, short Y)
        {
            // Verifica se há algum Jogador na cordenada
            for (byte i = 0; i < Lists.Account.Count; i++)
                if (Lists.Account[i].IsPlaying)
                    if (Lists.Account[i].Character.X == X && Lists.Account[i].Character.Y == Y && Lists.Account[i].Character.Map == this)
                        return Lists.Account[i].Character;

            return null;
        }

        public bool HasPlayers()
        {
            // Verifica se tem algum jogador no mapa
            for (byte i = 0; i < Lists.Account.Count; i++)
                if (Lists.Account[i].IsPlaying)
                    if (Lists.Account[i].Character.Map == this)
                        return true;

            return false;
        }

        public byte HasItem(byte X, byte Y)
        {
            // Verifica se tem algum item nas coordenadas 
            for (byte i = (byte)(Item.Count - 1); i >= 1; i--)
                if (Item[i].X == X && Item[i].Y == Y)
                    return i;

            return 0;
        }

        public void Spawn_Items()
        {
            TMap_Items Map_Item = new TMap_Items();

            // Verifica se tem algum atributo de item no mapa
            for (byte x = 0; x < Data.Width; x++)
                for (byte y = 0; y < Data.Height; y++)
                    if (Data.Tile[x, y].Attribute == (byte)Game.Tile_Attributes.Item)
                    {
                        // Faz o item aparecer
                        Map_Item.Item = (Item)Lists.GetData(Lists.Item, new Guid(Data.Tile[x, y].Data_5));
                        Map_Item.Amount = Data.Tile[x, y].Data_2;
                        Map_Item.X = x;
                        Map_Item.Y = y;
                        Item.Add(Map_Item);
                    }
        }

        public bool Tile_Blocked(short X, short Y, Game.Directions Direction, bool CountEntities = true)
        {
            short Next_X = X, Next_Y = Y;

            // Próximo azulejo
            Game.NextTile(Direction, ref Next_X, ref Next_Y);

            // Verifica se o azulejo está bloqueado
            if (Data.Tile_Blocked((byte)Next_X, (byte)Next_Y)) return true;
            if (Data.Tile[Next_X, Next_Y].Block[(byte)Game.ReverseDirection(Direction)]) return true;
            if (Data.Tile[X, Y].Block[(byte)Direction]) return true;
            if (CountEntities && (HasPlayer(Next_X, Next_Y) != null || HasNPC(Next_X, Next_Y) != null)) return true;
            return false;
        }
    }

    class TMap_Items
    {
        public Item Item;
        public byte X;
        public byte Y;
        public short Amount;
    }
}
