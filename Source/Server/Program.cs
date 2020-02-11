using System;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // Usado pra detectar quando o console é fechado
    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
    private delegate bool EventHandler();
    static EventHandler Handler;

    [STAThread]
    static void Main()
    {
        // Abre o servidor e define suas configurações
        Console.Title = "Server";
        Logo();
        Console.WriteLine("[Starting]");

        // Evento de saída do console
        Handler += new EventHandler(Exit);
        SetConsoleCtrlHandler(Handler, true);

        // Verifica se todos os diretórios existem, se não existirem então criá-los
        Directories.Create();
        Console.WriteLine("Directories created.");

        // Limpa e carrega todos os dados necessários
        Read.All();
        Clear.All();

        // Cria os dispositivos da rede
        Socket.Init();
        Console.WriteLine("Network started. Port: " + Socket.Device.Port);

        // Calcula quanto tempo demorou para inicializar o servidor
        Console.WriteLine("\r\n" + "Server started. Type 'help' to see the commands." + "\r\n");

        // Inicia os laços
        Thread Console_Loop = new Thread(Loop.Commands);
        Console_Loop.Start();
        Loop.Main();
    }

    private static bool Exit()
    {
        // Salva os dados de todos os jogadores
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Player.IsPlaying(i)) Write.Player(i);

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

    public static void ExecuteCommand(string Command)
    {
        // Previne sobrecargas
        if (string.IsNullOrEmpty(Command))
            return;

        // Separa os comandos em partes
        string[] Parts = Command.Split(' ');

        // Executa o determinado comando
        switch (Parts[0].ToLower())
        {
            case "help":
                Console.WriteLine(@"     List of available commands:
     defineaccess               - sets a level of access for a given player
     cps                        - shows the current server cps");
                break;
            case "cps":
                Console.WriteLine("CPS: " + Game.CPS);
                break;
            case "defineaccess":
                byte Access;

                // Verifica se o que está digitado corretamente
                if (Parts.GetUpperBound(0) < 2 || string.IsNullOrEmpty(Parts[1]) || !Byte.TryParse(Parts[2], out Access))
                {
                    Console.WriteLine("Use: defineaccess 'Player Name' 'Access' ");
                    return;
                }

                // Encontra o jogador
                byte Index = Player.FindUser(Parts[1]);

                if (Index == 0)
                {
                    Console.WriteLine("This player is either offline or doesn't exist.");
                    return;
                }

                // Define o acesso do jogador
                Lists.Player[Index].Acess = (Game.Accesses)Access;

                // Salva os dados
                Write.Player(Index);
                Console.WriteLine((Game.Accesses)Convert.ToByte(Parts[2]) + " access granted to " + Parts[1] + ".");
                break;
            // Se o comando não existir mandar uma mensagem de ajuda
            default:
                Console.WriteLine("This command does not exist.");
                break;
        }
    }
}