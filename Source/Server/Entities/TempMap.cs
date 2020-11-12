using System;
using System.Collections.Generic;
using CryBits.Entities;
using CryBits.Server.Logic;
using CryBits.Server.Network;
using static CryBits.Utils;

namespace CryBits.Server.Entities
{
    internal class TempMap : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, TempMap> List = new Dictionary<Guid, TempMap>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static TempMap Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Dados
        public Map Data;
        public TempNPC[] NPC = Array.Empty<TempNPC>();
        public List<MapItems> Item = new List<MapItems>();

        // Construtor
        public TempMap(Guid id, Map map) : base(id)
        {
            Data = map;
        }

        public void Logic()
        {
            // Não é necessário fazer todos os cálculos se não houver nenhum jogador no mapa
            if (!HasPlayers()) return;

            // Lógica dos NPCBehaviour
            for (byte j = 0; j < NPC.Length; j++) NPC[j].Logic();

            // Faz reaparecer todos os itens do mapa
            if (Environment.TickCount > Loop.Timer_Map_Items + 300000)
            {
                Item = new List<MapItems>();
                Spawn_Items();
                Send.Map_Items(this);
            }
        }

        public TempNPC HasNPC(byte x, byte y)
        {
            // Verifica se há algum npc na cordenada
            for (byte i = 0; i < NPC.Length; i++)
                if (NPC[i].Alive)
                    if (NPC[i].X == x && NPC[i].Y == y)
                        return NPC[i];

            return null;
        }

        public Player HasPlayer(byte x, byte y)
        {
            // Verifica se há algum Jogador na cordenada
            foreach (var account in Account.List)
                if (account.IsPlaying)
                    if ((account.Character.X, account.Character.Y, account.Character.Map) == (x, y, this))
                        return account.Character;

            return null;
        }

        public bool HasPlayers()
        {
            // Verifica se tem algum jogador no mapa
            foreach (var account in Account.List)
                if (account.IsPlaying)
                    if (account.Character.Map == this)
                        return true;

            return false;
        }

        public MapItems HasItem(byte x, byte y)
        {
            // Verifica se tem algum item nas coordenadas 
            for (int i = Item.Count - 1; i >= 0; i--)
                if (Item[i].X == x && Item[i].Y == y)
                    return Item[i];

            return null;
        }

        public void Spawn_Items()
        {
            // Verifica se tem algum atributo de item no mapa
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    if (Data.Attribute[x, y].Type == (byte)TileAttributes.Item)
                        // Adiciona o item
                        Item.Add(new MapItems
                        {
                            Item = CryBits.Entities.Item.Get(new Guid(Data.Attribute[x, y].Data_1)),
                            Amount = Data.Attribute[x, y].Data_2,
                            X = x,
                            Y = y
                        });
        }

        public bool Tile_Blocked(byte x, byte y, Directions direction, bool countEntities = true)
        {
            byte nextX = x, nextY = y;

            // Próximo azulejo
            NextTile(direction, ref nextX, ref nextY);

            // Verifica se o azulejo está bloqueado
            if (Data.Tile_Blocked(nextX, nextY)) return true;
            if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
            if (Data.Attribute[x, y].Block[(byte)direction]) return true;
            if (countEntities && (HasPlayer(nextX, nextY) != null || HasNPC(nextX, nextY) != null)) return true;
            return false;
        }

        public static void Create_Temporary(Map map)
        {
            TempMap tempMap = new TempMap(map.ID, map);
            List.Add(map.ID, tempMap);

            // NPCBehaviour do mapa
            tempMap.NPC = new TempNPC[map.NPC.Count];
            for (byte i = 0; i < tempMap.NPC.Length; i++)
            {
                tempMap.NPC[i] = new TempNPC(i, tempMap, map.NPC[i].NPC);
                tempMap.NPC[i].Spawn();
            }

            // Itens do mapa
            tempMap.Spawn_Items();
        }
    }

    internal class MapItems
    {
        public Item Item;
        public byte X;
        public byte Y;
        public short Amount;
    }
}
