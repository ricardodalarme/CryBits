using System;
using System.Windows.Forms;
using CryBits.Server.Entities;

namespace CryBits.Server.Logic
{
    static class Loop
    {
        // Contagens
        public static int Timer_500, Timer_1000;
        public static int Timer_Regen;
        public static int Timer_Map_Items;

        public static void Main()
        {
            int cps = 0;

            while (Program.Working)
            {
                // Manuseia os dados recebidos
                Network.Socket.HandleData();

                if (Environment.TickCount > Timer_500 + 500)
                {
                    // Lógicas do mapa
                    foreach (var tempMap in TempMap.List.Values) tempMap.Logic();

                    // Lógica dos jogadores
                    foreach (var account in Account.List)
                        if (account.IsPlaying)
                            account.Character.Logic();

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
                    Program.CPS = cps;
                    cps = 0;
                    Timer_1000 = Environment.TickCount + 1000;
                }
                else
                    cps += 1;
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
}