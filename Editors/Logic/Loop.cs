using System;
using System.Threading;
using CryBits.Client.Framework.Audio;
using CryBits.Editors.Entities;
using CryBits.Editors.AvaloniaUI.Forms;
using CryBits.Editors.Graphics;
using CryBits.Editors.Network;

namespace CryBits.Editors.Logic;

internal static class Loop
{
    public static void Init()
    {
        var timer1000 = 0;
        short fps = 0;

        while (Program.Working)
        {
            var count = Environment.TickCount;

            // Manuseia os dados recebidos
            Socket.HandleData();

            // Eventos
            TempMap.UpdateFog();
            TempMap.UpdateWeather();
            MapsMusic();

            // Desenha os gráficos
            Renders.Present();

            // Faz com que a aplicação se mantenha estável
            while (Environment.TickCount < count + 10) Thread.Sleep(1);

            // FPS
            if (timer1000 < Environment.TickCount)
            {
                // Cálcula o FPS
                Program.Fps = fps;
                fps = 0;

                // Reinicia a contagem
                timer1000 = Environment.TickCount + 1000;
            }
            else
                fps++;
        }

        // Fecha a aplicação
        Program.Close();
    }

    private static void MapsMusic()
    {
        // Apenas se necessário
        var win = EditorMapsWindow.Instance;
        if (win == null) return;
        if (win.SelectedMap == null) return;
        if (!win.IsOpen) { Music.Stop(); return; }
        if (!win.ShowAudioSafe) { Music.Stop(); return; }
        if (!win.ShowVisualizationSafe) { Music.Stop(); return; }
        if (string.IsNullOrEmpty(win.SelectedMap?.Music)) { Music.Stop(); return; }

        // Inicia a música
        if (Music.Device == null || Music.Current != win.SelectedMap?.Music)
            Music.Play(win.SelectedMap!.Music);
    }
}