using System;
using System.Collections.Generic;

namespace Objects
{
    class Class : Data
    {
        // Lista de dados
        public static Dictionary<Guid, Class> List;

        // Obtém o dado, caso ele não existir retorna nulo
        public static Class Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados gerais
        public string Name;
        public string Description;
        public short[] Tex_Male;
        public short[] Tex_Female;

        public Class(Guid ID) : base(ID) { }
    }
}
