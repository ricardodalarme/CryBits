using System;
using System.Threading;
using System.Windows.Forms;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms;
using CryBits.Editors.Network;
using Graphics = CryBits.Editors.Media.Graphics;
using Music = CryBits.Editors.Media.Audio.Music;

namespace CryBits.Editors.Logic
{
    internal static class Loop
    {
        public static void Init()
        {
            int count;
            int timer1000 = 0;
            short fps = 0;

            while (Program.Working)
            {
                count = Environment.TickCount;

                // Manuseia os dados recebidos
                Socket.HandleData();

                // Eventos
                TempMap.UpdateFog();
                TempMap.UpdateWeather();
                MapsMusic();

                // Desenha os gráficos
                Graphics.Present();

                // Faz com que a aplicação se mantenha estável
                Application.DoEvents();
                while (Environment.TickCount < count + 10) Thread.Sleep(1);

                // FPS
                if (timer1000 < Environment.TickCount)
                {
                    // Cálcula o FPS
                    Program.FPS = fps;
                    fps = 0;

                    // Reinicia a contagem
                    timer1000 = Environment.TickCount + 1000;
                }
                else
                    fps += 1;
            }

            // Fecha a aplicação
            Program.Close();
        }


        private static void MapsMusic()
        {
            // Apenas se necessário
            if (EditorMaps.Form == null || !EditorMaps.Form.Visible) goto stop;
            if (!EditorMaps.Form.butAudio.Checked) goto stop;
            if (!EditorMaps.Form.butVisualization.Checked) goto stop;
            if (EditorMaps.Form.Selected.Music == 0) goto stop;

            // Inicia a música
            if (Music.Device == null || Music.Current != (Musics)EditorMaps.Form.Selected.Music)
                Music.Play((Musics)EditorMaps.Form.Selected.Music);
            return;
        stop:
            // Para a música
            if (Music.Device != null) Music.Stop();
        }
    }
}