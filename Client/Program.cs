using System;
using System.Windows.Forms;
using CryBits.Client.Library;
using CryBits.Client.Logic;
using CryBits.Client.Media.Audio;
using CryBits.Client.Media.Graphics;
using CryBits.Client.Network;
using CryBits.Client.UI;

namespace CryBits.Client;

internal static class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    [STAThread]
    private static void Main()
    {
        // Verifica se todos os diretórios existem, se não existirem então criá-los
        Directories.Create();

        // Carrega todos os dados
        Read.Data();

        // Abre a janela
        Windows.OpenMenu();

        // Inicializa todos os dispositivos
        Socket.Init();
        Sound.Load();
        Renders.Init();

        // Inicia a aplicação
        Loop.Init();
    }

    public static void Close()
    {
        var waitTimer = Environment.TickCount;

        // Desconecta da rede
        Socket.Disconnect();

        // Espera até que o jogador seja desconectado
        while (Socket.IsConnected() && Environment.TickCount <= waitTimer + 1000)
            Application.DoEvents();

        // Fecha a aplicação
        Application.Exit();
    }
}