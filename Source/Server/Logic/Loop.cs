using System;
using System.Windows.Forms;

class Loop
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    // Contagens
    public static int Timer_500 = 0, Timer_1000 = 0, Timer_5000 = 0;
    public static int Timer_Player_Regen = 0;
    public static int Timer_NPC_Regen = 0;
    public static int Timer_Map_Items = 0;

    public static void Init()
    {
        int CPS = 0;

        while (Working)
        {
            // Manuseia os dados recebidos
            Socket.HandleData();

            if (Timer_500 < Environment.TickCount)
            {
                // Lógicas do mapa
                Map.Logic();

                // Lógica dos jogadores
                for (byte i = 0; i <= Game.HigherIndex; i++)
                    if (Account.IsPlaying(i))
                        Account.Character(i);
                if (Environment.TickCount > Timer_Player_Regen + 5000) Timer_Player_Regen = Environment.TickCount;

                // Reinicia a contagem
                Timer_500 = Environment.TickCount + 500;
            }

            // Faz com que a aplicação se mantenha estável
            Application.DoEvents();

            // Calcula o CPS
            if (Timer_1000 < Environment.TickCount)
            {
                Game.CPS = CPS;
                CPS = 0;
                Timer_1000 = Environment.TickCount + 1000;
            }
            else
                CPS += 1;
        }
    }

    public static void Commands()
    {
        // Laço para que seja possível a utilização de comandos pelo console
        while (Working)
        {
            Console.Write("Execute: ");
            Program.ExecuteCommand(Console.ReadLine());
        }
    }
}