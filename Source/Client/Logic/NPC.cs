using System;

class NPC : Character
{
    // Indice
    public short Index;

    public void Logic()
    {
        // Dano
        if (Hurt + 325 < Environment.TickCount) Hurt = 0;

        // Movimento
        ProcessMovement();
    }
}