using CryBits.Entities;
using System;
using System.Collections.Generic;

namespace CryBits.Client.Entities
{
    class Class : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Class> List = new Dictionary<Guid, Class>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Class Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Dados gerais
        public string Description;
        public short[] Tex_Male;
        public short[] Tex_Female;

        public Class(Guid id) : base(id) { }
    }
}
