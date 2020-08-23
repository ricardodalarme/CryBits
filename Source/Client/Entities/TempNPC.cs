using System;

namespace Objects
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