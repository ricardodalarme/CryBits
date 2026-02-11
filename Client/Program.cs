using System;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Library;
using CryBits.Client.Graphics;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using CryBits.Client.UI;
using CryBits.Client.UI.Events;

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
        Read.Tools();
        Read.Options();

        // Adiciona os eventos aos componentes
        CheckBoxEvents.Bind();
        ButtonsEvents.Bind();
        PanelsEvents.Bind();
        TextBoxesEvents.Bind();
        Window.Bind();

        // Abre a janela
        Window.OpenMenu();

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
            Socket.HandleData();

        // Fecha a aplicação
        Working = false;
        Environment.Exit(0);
    }
}
