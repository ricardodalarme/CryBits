using System;
using System.Collections.Generic;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Server.Logic;
using CryBits.Server.Network;
using static CryBits.Utils;
using Item = CryBits.Entities.Item;

namespace CryBits.Server.Entities
{
    internal class TempMap : Entity
    {
        // Lista de dados
        public static readonly Dictionary<Guid, TempMap> List = new Dictionary<Guid, TempMap>();

        // Dados
        public readonly Map Data;
        public TempNpc[] Npc = Array.Empty<TempNpc>();
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

            // Lógica dos Npcs
            for (byte j = 0; j < Npc.Length; j++) Npc[j].Logic();

            // Faz reaparecer todos os itens do mapa
            if (Environment.TickCount > Loop.TimerMapItems + 300000)
            {
                Item = new List<MapItems>();
                SpawnItems();
                Send.MapItems(this);
            }
        }

        public TempNpc HasNpc(byte x, byte y)
        {
            // Verifica se há algum Npc na cordenada
            for (byte i = 0; i < Npc.Length; i++)
                if (Npc[i].Alive)
                    if (Npc[i].X == x && Npc[i].Y == y)
                        return Npc[i];

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

        public void SpawnItems()
        {
            // Verifica se tem algum atributo de item no mapa
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    if (Data.Attribute[x, y].Type == (byte)TileAttribute.Item)
                        // Adiciona o item
                        Item.Add(new MapItems(CryBits.Entities.Item.List.Get(new Guid(Data.Attribute[x, y].Data1)), Data.Attribute[x, y].Data2, x, y));
        }

        public bool TileBlocked(byte x, byte y, Direction direction, bool countEntities = true)
        {
            byte nextX = x, nextY = y;

            // Próximo azulejo
            NextTile(direction, ref nextX, ref nextY);

            // Verifica se o azulejo está bloqueado
            if (Data.TileBlocked(nextX, nextY)) return true;
            if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
            if (Data.Attribute[x, y].Block[(byte)direction]) return true;
            if (countEntities && (HasPlayer(nextX, nextY) != null || HasNpc(nextX, nextY) != null)) return true;
            return false;
        }

        public static void CreateTemporary(Map map, bool isOriginal)
        {
            Guid id = isOriginal ? map.ID : Guid.NewGuid();
            TempMap tempMap = new TempMap(id, map);
            List.Add(id, tempMap);

            // NpcBehaviour do mapa
            tempMap.Npc = new TempNpc[map.Npc.Count];
            for (byte i = 0; i < tempMap.Npc.Length; i++)
            {
                tempMap.Npc[i] = new TempNpc(i, tempMap, map.Npc[i].Npc);
                tempMap.Npc[i].Spawn();
            }

            // Itens do mapa
            tempMap.SpawnItems();
        }
    }

    internal class MapItems : ItemSlot
    {
        public byte X;
        public byte Y;

        public MapItems(Item item, short amount, byte x, byte y) : base(item, amount)
        {
            X = x;
            Y = y;
        }
    }
}
