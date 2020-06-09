using System;
using System.Windows.Forms;

class Loop
{
    // Contagens
    public static int Timer_500 = 0, Timer_1000 = 0, Timer_5000 = 0;
    public static int Timer_Regen = 0;
    public static int Timer_Map_Items = 0;

    public static void Init()
    {
        int CPS = 0;

        while (Program.Working)
        {
            // Manuseia os dados recebidos
            Socket.HandleData();

            if (Environment.TickCount > Timer_500 + 500)
            {
                // Lógicas do mapa
                foreach (var Temp_Map in Lists.Temp_Map.Values) Temp_Map.Logic();

                // Lógica dos jogadores
                foreach (var Account in  Lists.Account)
                    if (Account.IsPlaying)
                        Account.Character.Logic();

                // Reinicia a contagem dos 500
                Timer_500 = Environment.TickCount;
            }

            // Reinicia algumas contagens
            if (Environment.TickCount > Timer_Regen + 5000) Timer_Regen = Environment.TickCount;
            if (Environment.TickCount > Timer_Map_Items + 300000) Timer_Map_Items = Environment.TickCount;

            // Faz com que a aplicação se mantenha estável
            Application.DoEvents();

            // Calcula o CPS
            if (Timer_1000 < Environment.TickCount)
            {
                Program.CPS = CPS;
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
        while (Program.Working)
        {
            Console.Write("Execute: ");
            Program.ExecuteCommand(Console.ReadLine());
        }
    }
}