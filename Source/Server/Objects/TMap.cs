using Network;
using System;
using System.Collections.Generic;
using Logic;
using static Logic.Utils;

namespace Objects
{
    class TMap : Data
    {
        // Lista de dados
        public static Dictionary<Guid, TMap> List = new Dictionary<Guid, TMap>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static TMap Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

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
            for (byte j = 0; j < NPC.Length; j++) NPC[j].Logic();

            // Faz reaparecer todos os itens do mapa
            if (Environment.TickCount > Loop.Timer_Map_Items + 300000)
            {
                Item = new List<TMap_Items>();
                Spawn_Items();
                Send.Map_Items(this);
            }
        }

        public TNPC HasNPC(byte X, byte Y)
        {
            // Verifica se há algum npc na cordenada
            for (byte i = 0; i < NPC.Length; i++)
                if (NPC[i].Alive)
                    if (NPC[i].X == X && NPC[i].Y == Y)
                        return NPC[i];

            return null;
        }

        public Player HasPlayer(byte X, byte Y)
        {
            // Verifica se há algum Jogador na cordenada
            foreach (var Account in Account.List)
                if (Account.IsPlaying)
                    if ((Account.Character.X, Account.Character.Y, Account.Character.Map) == (X, Y, this))
                        return Account.Character;

            return null;
        }

        public bool HasPlayers()
        {
            // Verifica se tem algum jogador no mapa
            foreach (var Account in Account.List)
                if (Account.IsPlaying)
                    if (Account.Character.Map == this)
                        return true;

            return false;
        }

        public TMap_Items HasItem(byte X, byte Y)
        {
            // Verifica se tem algum item nas coordenadas 
            for (byte i = (byte)(Item.Count - 1); i >= 0; i--)
                if (Item[i].X == X && Item[i].Y == Y)
                    return Item[i];

            return null;
        }

        public void Spawn_Items()
        {
            // Verifica se tem algum atributo de item no mapa
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    if (Data.Attribute[x, y].Type == (byte)Tile_Attributes.Item)
                    {
                        // Faz o item aparecer
                        TMap_Items Map_Item = new TMap_Items();
                        Map_Item.Item = Objects.Item.Get(new Guid(Data.Attribute[x, y].Data_1));
                        Map_Item.Amount = Data.Attribute[x, y].Data_2;
                        Map_Item.X = x;
                        Map_Item.Y = y;
                        Item.Add(Map_Item);
                    }
        }

        public bool Tile_Blocked(byte X, byte Y, Directions Direction, bool CountEntities = true)
        {
            byte Next_X = X, Next_Y = Y;

            // Próximo azulejo
            NextTile(Direction, ref Next_X, ref Next_Y);

            // Verifica se o azulejo está bloqueado
            if (Data.Tile_Blocked(Next_X, Next_Y)) return true;
            if (Data.Attribute[Next_X, Next_Y].Block[(byte)ReverseDirection(Direction)]) return true;
            if (Data.Attribute[X, Y].Block[(byte)Direction]) return true;
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
