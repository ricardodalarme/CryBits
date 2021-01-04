using System;
using System.Collections.Generic;
using CryBits.Entities;

namespace CryBits.Client.Entities
{
    internal class Player : Character
    {
        // Lista de dados
        public static List<Player> List;

        // Obtém um jogador com determinado nome
        public static Player Get(string name) => List.Find(x => x.Name.Equals(name));

        // O próprio jogador
        public static Me Me;

        // Dados gerais dos jogadores
        public string Name { get; set; } = string.Empty;
        public short TextureNum { get; set; }
        public short Level { get; set; }
        public short[] MaxVital { get; set; } = new short[(byte)Enums.Vital.Count];
        public short[] Attribute { get; set; } = new short[(byte)Enums.Attribute.Count];
        public Item[] Equipment { get; set; } = new Item[(byte)Enums.Equipment.Count];
        public TempMap Map;

        public Player(string name)
        {
            Name = name;
        }

        public virtual void Logic()
        {
            // Dano
            if (Hurt + 325 < Environment.TickCount) Hurt = 0;

            // Movimentaçãp
            ProcessMovement();
        }
    }
}