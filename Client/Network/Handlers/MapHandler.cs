using System.Collections.Generic;
using System.IO;
using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Library;
using CryBits.Client.Framework.Library.Repositories;
using CryBits.Client.Network.Senders;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Extensions;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class MapHandler
{
    internal static void MapRevision(NetDataReader data)
    {
        var needed = false;
        var id = data.GetGuid();
        var currentRevision = data.GetShort();

        // Limpa todos os outros jogadores
        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                Player.List.RemoveAt(i);

        // Verifica se é necessário baixar os dados do mapa
        if (File.Exists(Directories.MapsData.FullName + id + Directories.Format) ||
            CryBits.Entities.Map.Map.List.ContainsKey(id))
        {
            if (!CryBits.Entities.Map.Map.List.ContainsKey(id))
            {
                MapRepository.Read(id);
                TempMap.Current.Weather.Update();
                TempMap.Current.Data.Update();
            }

            if (CryBits.Entities.Map.Map.List[id].Revision != currentRevision)
                needed = true;
        }
        else
            needed = true;

        // Solicita os dados do mapa
        MapSender.RequestMap(needed);

        // Reseta os sangues do mapa
        TempMap.Current.Blood = new List<TempMapBlood>();
    }

    internal static void Map(NetDataReader data)
    {
        var map = (Map)data.ReadObject();
        var id = map.Id;

        // Obtém o dado
        if (CryBits.Entities.Map.Map.List.ContainsKey(id)) CryBits.Entities.Map.Map.List[id] = map;
        else
        {
            CryBits.Entities.Map.Map.List.Add(id, map);
            TempMap.List.Add(id, new TempMap(map));
        }

        TempMap.Current = TempMap.List[id];

        // Salva o mapa
        MapRepository.Write(map);

        // Redimensiona as partículas do clima
        TempMap.Current.Weather.UpdateType();
        TempMap.Current.Data.Update();
    }

    internal static void JoinMap()
    {
        // Se tiver, reproduz a música de fundo do mapa
        if (string.IsNullOrEmpty(TempMap.Current.Data.Music))
            Music.Stop();
        else
            Music.Play(TempMap.Current.Data.Music);
    }

    internal static void MapItems(NetDataReader data)
    {
        // Quantidade
        TempMap.Current.Item = new TempMapItems[data.GetByte()];

        // Lê os dados de todos
        for (byte i = 0; i < TempMap.Current.Item.Length; i++)
            TempMap.Current.Item[i] = new TempMapItems
            {
                Item = Item.List.Get(data.GetGuid()),
                X = data.GetByte(),
                Y = data.GetByte()
            };
    }
}
