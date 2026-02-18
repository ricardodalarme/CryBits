using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using CryBits.Enums;
using static CryBits.Client.Logic.Camera;
using Color = SFML.Graphics.Color;
using CryBits.Client.Graphics.Renderers;

namespace CryBits.Client.Graphics;

internal static class RenderPipeline
{
  public static void Present()
  {
    // Limpa a área com um fundo preto
    Renders.RenderWindow.Clear(Color.Black);

    // Desenha as coisas em jogo
    InGame();

    // Interface do jogo
    UIRenderer.Interface(Screen.Current?.Body);

    // Desenha o chat 
    if (Screen.Current == Screens.Game) UIRenderer.Chat();

    // Exibe o que foi renderizado
    Renders.RenderWindow.Display();
  }

  private static void InGame()
  {
    // Não desenhar se não estiver em jogo
    if (Screen.Current != Screens.Game) return;

    // Atualiza a câmera
    Update();

    // Desenhos abaixo do jogador
    MapRenderer.MapPanorama();
    MapRenderer.MapTiles((byte)Layer.Ground);
    MapRenderer.MapBlood();
    MapRenderer.MapItems();

    // Desenha os Npcs
    for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
      if (TempMap.Current.Npc[i].Data != null)
        NpcRenderer.Npc(TempMap.Current.Npc[i]);

    // Desenha os jogadores
    for (byte i = 0; i < Player.List.Count; i++)
      if (Player.List[i] != Player.Me)
        if (Player.List[i].Map == Player.Me.Map)
          PlayerRenderer.PlayerCharacter(Player.List[i]);

    // Desenha o próprio jogador
    PlayerRenderer.PlayerCharacter(Player.Me);

    // Desenhos acima do jogador
    MapRenderer.MapTiles((byte)Layer.Fringe);
    MapRenderer.MapWeather();
    MapRenderer.MapFog();
    MapRenderer.MapName();

    // Desenha os membros da party
    UIRenderer.Party();

    // Desenha os dados do jogo
    if (Options.Fps) Renders.DrawText("FPS: " + Loop.Fps, 176, 7, Color.White);
    if (Options.Latency) Renders.DrawText("Latency: " + Socket.Latency, 176, 19, Color.White);
  }
}