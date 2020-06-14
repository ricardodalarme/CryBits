using Network;
using System;
using System.Windows.Forms;

class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    [STAThread]
    static void Main()
    {
        // Verifica se todos os diretórios existem, se não existirem então criá-los
        Directories.Create();

        // Carrega todos os dados
        Read.Data();

        // Abre a janela
        Interface.Windows.OpenMenu();

        // Inicializa todos os dispositivos
        Socket.Init();
        Audio.Sound.Load();
        Graphics.Init();

        // Inicia a aplicação
        Loop.Init();
    }

    public static void Close()
    {
        int Wait_Timer = Environment.TickCount;

        // Desconecta da rede
        Socket.Disconnect();

        // Espera até que o jogador seja desconectado
        while (Socket.IsConnected() && Environment.TickCount <= Wait_Timer + 1000)
            Application.DoEvents();

        // Fecha a aplicação
        Application.Exit();
    }
}