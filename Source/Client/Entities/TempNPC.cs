using System;

namespace CryBits.Client.Entities
{
    class TempNPC : Character
    {
        // Indice
        public NPC Data;

        public void Logic()
        {
            // Dano
            if (Hurt + 325 < Environment.TickCount) Hurt = 0;

            // Movimento
            ProcessMovement();
        }
    }
}