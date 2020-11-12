using CryBits.Entities;
using System;
using System.Collections.Generic;

namespace CryBits.Server.Entities
{
    [Serializable]
    class Mapp 
    {

        public static void Create_Temporary(Map map)
        {
            TempMap Temp_Map = new TempMap(map.ID, map);
            TempMap.List.Add(map.ID, Temp_Map);

            // NPCs do mapa
            Temp_Map.NPC = new TempNPC[map.NPC.Length];
            for (byte i = 0; i < Temp_Map.NPC.Length; i++)
            {
                Temp_Map.NPC[i] = new TempNPC(i, Temp_Map, map.NPC[i].NPC);
                Temp_Map.NPC[i].Spawn();
            }

            // Itens do mapa
            Temp_Map.Spawn_Items();
        }

    }
}