using CryBits.Server.Entities;
using CryBits.Server.Network;
using System;
using System.Threading;

namespace CryBits.Server.Logic
{
    internal static class Loop
    {
        // Medida de quantos loops são executados por segundo 
        public static int CPS;

        // Contagens
        private static int _timer500, _timer1000;
        public static int TimerRegen;
        public static int TimerMapItems;

        public static void Main()
        {
            int cps = 0;

            while (Program.Working)
            {
                // Manuseia os dados recebidos
                Socket.HandleData();

                if (Environment.TickCount > _timer500 + 500)
                {
                    // Lógicas do mapa
                    foreach (var tempMap in TempMap.List.Values) tempMap.Logic();

                    // Lógica dos jogadores
                    foreach (var account in Account.List)
                        if (account.IsPlaying)
                            account.Character.Logic();

                    // Reinicia a contagem dos 500
                    _timer500 = Environment.TickCount;
                }

                // Reinicia algumas contagens
                if (Environment.TickCount > TimerRegen + 5000) TimerRegen = Environment.TickCount;
                if (Environment.TickCount > TimerMapItems + 300000) TimerMapItems = Environment.TickCount;

                // Faz com que a aplicação se mantenha estável
                Thread.Sleep(0); 

                // Calcula o CPS
                if (_timer1000 < Environment.TickCount)
                {
                    CPS = cps;
                    cps = 0;
                    _timer1000 = Environment.TickCount + 1000;
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