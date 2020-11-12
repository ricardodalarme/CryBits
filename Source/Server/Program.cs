using System;
using System.Runtime.InteropServices;
using System.Threading;
using CryBits.Entities;
using CryBits.Server.Entities;
using CryBits.Server.Library;
using CryBits.Server.Logic;
using CryBits.Server.Network;

namespace CryBits.Server
{
    static class Program
    {
        // Usado para manter a aplicação aberta
        public static bool Working = true;

        // CPS do servidor
        public static int CPS;

        // Usado pra detectar quando o console é fechado
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler();
        static EventHandler _handler;

        [STAThread]
        private static void Main()
        {
            // Abre o servidor e define suas configurações
            Console.Title = "Server";
            Logo();
            Console.WriteLine("[Starting]");

            // Evento de saída do console
            _handler += new EventHandler(Exit);
            SetConsoleCtrlHandler(_handler, true);

            // Verifica se todos os diretórios existem, se não existirem então criá-los
            Directories.Create();
            Console.WriteLine("Directories created.");

            // Carrega todos os dados necessários
            Read.All();

            // Cria os mapas temporários
            Console.WriteLine("Creating temporary maps.");
            foreach (Map map in Map.List.Values) TempMap.Create_Temporary(map);

            // Cria os dispositivos da rede
            Socket.Init();
            Console.WriteLine("Network started. Port: " + Socket.Device.Port);

            // Calcula quanto tempo demorou para inicializar o servidor
            Console.WriteLine("\r\n" + "Server started. Type 'help' to see the commands." + "\r\n");

            // Inicia os laços
            Thread consoleLoop = new Thread(Loop.Commands);
            consoleLoop.Start();
            Loop.Main();
        }

        private static bool Exit()
        {
            // Salva os dados de todos os jogadores
            for (byte i = 0; i < Account.List.Count; i++)
                if (Account.List[i].IsPlaying)
                    Write.Character(Account.List[i]);

            // Fecha o servidores
            Socket.Device.Shutdown("Server was shut down.");
            Thread.Sleep(200);
            return true;
        }

        private static void Logo()
        {
            Console.WriteLine(@"  ______              _____     _
 |   ___|            |     \   | |
 |  |     _ ____   _ |   __/ _ | |_  ___
 |  |    | '__/\\ // |   \_ | || __|/ __|
 |  |___ | |    | |  |     \| || |_ \__ \
 |______||_|    |_|  |_____/|_| \__||___/
                          2D orpg engine" + "\r\n");
        }

        public static void ExecuteCommand(string command)
        {
            // Previne sobrecargas
            if (string.IsNullOrEmpty(command))
                return;

            // Separa os comandos em partes
            string[] parts = command.Split(' ');

            // Executa o determinado comando
            switch (parts[0].ToLower())
            {
                case "help":
                    Console.WriteLine(@"     List of available commands:
     defineaccess               - sets a level of access for a given player
     cps                        - shows the current server cps");
                    break;
                case "cps":
                    Console.WriteLine("CPS: " + CPS);
                    break;
                case "defineaccess":
                    byte access;

                    // Verifica se o que está digitado corretamente
                    if (parts.GetUpperBound(0) < 2 || string.IsNullOrEmpty(parts[1]) || !byte.TryParse(parts[2], out access))
                    {
                        Console.WriteLine("Use: defineaccess 'Player Name' 'Access' ");
                        return;
                    }

                    // Encontra o jogador
                    Account account = Account.List.Find(x => x.User.Equals(parts[1]));

                    if (account == null)
                    {
                        Console.WriteLine("This player is either offline or doesn't exist.");
                        return;
                    }

                    // Define o acesso do jogador
                    account.Acess = (Accesses)access;

                    // Salva os dados
                    Write.Account(account);
                    Console.WriteLine((Accesses)Convert.ToByte(parts[2]) + " access granted to " + parts[1] + ".");
                    break;
                // Se o comando não existir mandar uma mensagem de ajuda
                default:
                    Console.WriteLine("This command does not exist.");
                    break;
            }
        }
    }
}