﻿using System;
using System.Collections.Generic;

namespace Objects
{
    class NPC : Data
    {
        // Lista de dados
        public static Dictionary<Guid, NPC> List;

        // Obtém o dado, caso ele não existir retorna nulo
        public static NPC Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados gerais
        public string Name;
        public string SayMsg;
        public short Texture;
        public byte Type;
        public short[] Vital = new short[(byte)Utils.Vitals.Count];

        public NPC(Guid ID) : base(ID) { }
    }
}
