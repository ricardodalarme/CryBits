using System;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Utils;
using CryBits.Enums;
using static CryBits.Client.Utils.TextUtils;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal static class PlayerRenderer
{
  public static void PlayerCharacter(Player player)
  {
    // Desenha o jogador
    PlayerTexture(player);
    PlayerName(player);
    PlayerBars(player);
  }

  private static void PlayerTexture(Player player)
  {
    var column = AnimationStopped;
    var hurt = false;

    // Previne sobrecargas
    if (player.TextureNum <= 0 || player.TextureNum > Textures.Characters.Count) return;

    // Define a animação
    if (player.Attacking && player.AttackTimer + AttackSpeed / 2 > Environment.TickCount)
      column = AnimationAttack;
    else
    {
      if (player.X2 > 8 && player.X2 < Grid) column = player.Animation;
      if (player.X2 < -8 && player.X2 > Grid * -1) column = player.Animation;
      if (player.Y2 > 8 && player.Y2 < Grid) column = player.Animation;
      if (player.Y2 < -8 && player.Y2 > Grid * -1) column = player.Animation;
    }

    // Demonstra que o personagem está sofrendo dano
    if (player.Hurt > 0) hurt = true;

    // Desenha o jogador
    CharacterRenderer.Character(player.TextureNum,
      new Point(CameraUtils.ConvertX(player.PixelX), CameraUtils.ConvertY(player.PixelY)), player.Direction,
      column, hurt);
  }

  private static void PlayerBars(Player player)
  {
    var value = player.Vital[(byte)Vital.Hp];

    // Apenas se necessário
    if (value <= 0 || value >= player.MaxVital[(byte)Vital.Hp]) return;

    // Cálcula a largura da barra
    var characterSize = Textures.Characters[player.TextureNum].ToSize();
    var fullWidth = characterSize.Width / AnimationAmount;
    var width = value * fullWidth / player.MaxVital[(byte)Vital.Hp];

    // Posição das barras
    var position = new Point
    {
      X = CameraUtils.ConvertX(player.PixelX),
      Y = CameraUtils.ConvertY(player.PixelY) + characterSize.Height / AnimationAmount + 4
    };

    // Desenha as barras 
    Renders.Render(Textures.Bars, position.X, position.Y, 0, 4, fullWidth, 4);
    Renders.Render(Textures.Bars, position.X, position.Y, 0, 0, width, 4);
  }

  private static void PlayerName(Player player)
  {
    var texture = Textures.Characters[player.TextureNum];
    int nameSize = MeasureString(player.Name);

    // Posição do texto
    var position = new Point
    {
      X = player.PixelX + texture.ToSize().Width / AnimationAmount / 2 - nameSize / 2,
      Y = player.PixelY - texture.ToSize().Height / AnimationAmount / 2
    };

    // Cor do texto
    var color = player == Player.Me ? Color.Yellow : Color.White;

    // Desenha o texto
    Renders.DrawText(player.Name, CameraUtils.ConvertX(position.X), CameraUtils.ConvertY(position.Y), color);
  }
}