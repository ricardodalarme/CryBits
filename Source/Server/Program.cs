using System;
using System.Threading;
using System.Windows.Forms;

class Program
{
    [STAThread]
    public static void Main()
    {
        // Abre o servidor e define suas configurações
        Console.Title = "Server";
        Logo();
        Console.WriteLine("[Starting]");

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

    public static void Logo()
    {
        Console.WriteLine(@"  ______              _____     _
 |   ___|            |     \   | |
 |  |     _ ____   _ |   __/ _ | |_  ___
 |  |    | '__/\\ // |   \_ | || __|/ __|
 |  |___ | |    | |  |     \| || |_ \__ \
 |______||_|    |_|  |_____/|_| \__||___/
                          2D orpg engine" + "\r\n");
    }

    public static void Close()
    {
        // Disconeta todos os jogadores e fecha o servidor
        Socket.Device.Shutdown("Server was shut down.");
        Application.Exit();
    }

    public static void ExecuteCommand(string Command)
    {
        // Previne sobrecargas
        if (string.IsNullOrEmpty(Command))
            return;

        // Transforma o comando em letras minúsculas
        Command = Command.ToLower();

        // Separa os comandos em partes
        string[] Parts = Command.Split(' ');

        // Executa o determinado comando
        switch (Parts[0].ToLower())
        {
            case "help":
                Console.WriteLine(@"     List of available commands:
     defineaccess               - sets a level of access for a given player
     cps                        - shows the current server cps
     reload                     - reload certain data ");
                break;
            case "cps":
                Console.WriteLine("CPS: " + Game.CPS);
                break;
            case "defineaccess":
                byte Acess;

                // Verifica se o que está digitado corretamente
                if (Parts.GetUpperBound(0) < 2 || string.IsNullOrEmpty(Parts[1]) || !Byte.TryParse(Parts[2], out Acess))
                {
                    Console.WriteLine("Use: defineaccess 'Player Name' 'Access' ");
                    return;
                }

                // Encontra o jogador
                byte Index = Player.Find(Parts[1]);

                if (Index == 0)
                {
                    Console.WriteLine("This player is either offline or doesn't exist.");
                    return;
                }

                // Define o acesso do jogador
                Lists.Player[Index].Acess = (Game.Accesses)Convert.ToByte(Parts[2]);

                // Salva os dados
                Write.Player(Index);
                Console.WriteLine((Game.Accesses)Convert.ToByte(Parts[2]) + " access granted to " + Parts[1] + ".");
                break;
            case "reload":
                // Verifica se o que está digitado corretamente
                if (Parts.GetUpperBound(0) < 1 || string.IsNullOrEmpty(Parts[1]))
                {
                    Console.WriteLine("Use: reload 'Item desired'");
                    return;
                }

                switch (Parts[1])
                {
                    // Recarrega os mapas
                    case "maps":
                        Read.Maps();
                        Console.WriteLine("Reloaded maps");
                        break;
                    // Recarrega as classes
                    case "classes":
                        Read.Classes();
                        Console.WriteLine("Reloaded classes");
                        break;
                    // Recarrega os NPCs
                    case "npcs":
                        Read.NPCs();
                        Console.WriteLine("Reloaded npcs");
                        break;
                    // Recarrega os itens
                    case "items":
                        Read.Items();
                        Console.WriteLine("Reloaded items");
                        break;
                }
                break;
            // Se o comando não existir mandar uma mensagem de ajuda
            default:
                Console.WriteLine("This command does not exist.");
                break;
        }
    }
}