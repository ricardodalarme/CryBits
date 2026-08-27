using CryBits.Editors.Core;

namespace CryBits.Editors.Logic;

internal class Loop(EditorShell shell)
{
    /// <summary>
    /// Run the editor tick loop: process incoming data, update map state and play map music.
    /// </summary>
    public async Task Run()
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));
        short fps = 0;
        long timer1000 = 0;

        while (shell.Working && await timer.WaitForNextTickAsync())
            try
            {
                Tick();

                if (timer1000 < Environment.TickCount64)
                {
                    shell.Fps = fps;
                    fps = 0;
                    timer1000 = Environment.TickCount64 + 1000;
                }
                else
                {
                    fps++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Editor] Loop threw an exception: {ex}");
            }

        shell.Close();
    }

    private void Tick()
    {
        shell.Connection.Poll();
        shell.MapInstance.UpdateFog();
        shell.MapInstance.UpdateWeather();
        MapsMusic();
    }

    private void MapsMusic()
    {
        // Return early when the selected map is unavailable or audio is disabled.
        var win = shell.MapsWindow;
        if (win?.SelectedMap == null) return;
        if (!win.IsOpen)
        {
            shell.Audio.StopMusic();
            return;
        }

        if (!win.ShowAudioSafe)
        {
            shell.Audio.StopMusic();
            return;
        }

        if (!win.ShowVisualizationSafe)
        {
            shell.Audio.StopMusic();
            return;
        }

        if (string.IsNullOrEmpty(win.SelectedMap?.Music))
        {
            shell.Audio.StopMusic();
            return;
        }

        // Start the map music if not already playing.
        if (shell.Audio.CurrentMusic == null ||
            shell.Audio.CurrentMusic != win.SelectedMap?.Music)
            shell.Audio.PlayMusic(win.SelectedMap!.Music);
    }
}
