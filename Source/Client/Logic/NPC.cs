using System;

class NPC : Character
{
    // Indice
    public Lists.Structures.NPC Data;

    public void Logic()
    {
        // Dano
        if (Hurt + 325 < Environment.TickCount) Hurt = 0;

        // Movimento
        ProcessMovement();
    }
}