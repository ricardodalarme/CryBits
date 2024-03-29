﻿using System;
using System.Threading;
using System.Windows.Forms;
using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Graphics;
using CryBits.Client.Network;
using CryBits.Client.UI.Events;
using Screen = CryBits.Client.Framework.Interfacily.Components.Screen;

namespace CryBits.Client.Logic;

internal static class Loop
{
    // Medida de quantos frames são renderizados por segundo 
    public static short Fps;

    // Contagens
    public static int TextBoxTimer;
    public static int ChatTimer;

    public static void Init()
    {
        var timer1000 = 0;
        var timer30 = 0;
        short fps = 0;

        while (Program.Working)
        {
            // Manuseia os dados recebidos
            Socket.HandleData();

            // Apresenta os gráficos à tela
            Renders.Present();

            // Processa os eventos da janela
            Renders.RenderWindow.DispatchEvents();

            // Eventos
            TextBox();

            if (Screen.Current == Screens.Game)
            {
                TempMap.Current.Logic();
                if (timer30 < Environment.TickCount)
                {
                    // Lógica dos jogadores
                    for (byte i = 0; i < Player.List.Count; i++)
                        Player.List[i].Logic();

                    // Lógica dos Npcs
                    for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
                        if (TempMap.Current.Npc[i].Data != null)
                            TempMap.Current.Npc[i].Logic();

                    // Reinicia a contagem
                    timer30 = Environment.TickCount + 30;
                }

                // Verifica se é necessário mostrar o painel de informações
                PanelsEvents.CheckInformation();
            }

            // Faz com que a aplicação se mantenha estável
            Application.DoEvents();
            Thread.Sleep(1);

            // Cálcula o FPS
            if (timer1000 < Environment.TickCount)
            {
                Send.Latency();
                Fps = fps;
                fps = 0;
                timer1000 = Environment.TickCount + 1000;
            }
            else
                fps++;
        }

        // Fecha o jogo
        Program.Close();
    }

    private static void TextBox()
    {
        // Contagem para a renderização da referência do último texto
        if (TextBoxTimer < Environment.TickCount)
        {
            TextBoxTimer = Environment.TickCount + 500;
            TextBoxesEvents.Signal = !TextBoxesEvents.Signal;

            // Se necessário foca o digitalizador de novo
            Framework.Interfacily.Components.TextBox.Focus();
        }
    }
}