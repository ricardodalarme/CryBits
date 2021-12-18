using System;
using System.Windows.Forms;
using CryBits.Editors.Forms;
using CryBits.Editors.Library;
using CryBits.Editors.Logic;
using CryBits.Editors.Media.Audio;
using CryBits.Editors.Media.Graphics;
using CryBits.Editors.Network;

namespace CryBits.Editors;

internal static class Program
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    // Medida de calculo do atraso do jogo
    public static short FPS;

    [STAThread]
    private static void Main()
    {
        // Verifica se todos os diretórios existem, se não existirem então criá-os
        Directories.Create();

        // Carrega as preferências
        Read.Options();

        // Inicializa todos os dispositivos
        Socket.Init();
        Sound.LoadAll();
        Textures.LoadAll();
        Fonts.LoadAll();

        // Abre a janela
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Login.Form = new Login();
        Login.Form.Show();

        // Inicia o laço
        Loop.Init();
    }

    public static void Close()
    {
        int waitTimer = Environment.TickCount;

        // Desconecta da rede
        Socket.Disconnect();

        // Espera até que o jogador seja desconectado
        while (Socket.IsConnected() && Environment.TickCount <= waitTimer + 1000)
            Application.DoEvents();

        // Fecha a aplicação
        Application.Exit();
    }
}