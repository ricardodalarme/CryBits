using CryBits.Entities;
using System;

namespace CryBits.Client.Entities
{
    internal class TempNpc : Character
    {
        // Indice
        public Npc Data;

        public void Logic()
        {
            // Dano
            if (Hurt + 325 < Environment.TickCount) Hurt = 0;

            // Movimento
            ProcessMovement();
        }
    }
}