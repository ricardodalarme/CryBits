using System;

class NPC : Objects.Character
{
    // Indice
    public Objects.NPC Data;

    public void Logic()
    {
        // Dano
        if (Hurt + 325 < Environment.TickCount) Hurt = 0;

        // Movimento
        ProcessMovement();
    }
}