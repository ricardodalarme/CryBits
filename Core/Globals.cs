using CryBits.Entities.Map;

namespace CryBits;

public static class Globals
{
    /// <summary>Runtime server configuration loaded from <c>settings.json</c>.</summary>
    public static ServerConfig Config { get; set; } = new();

    // Tamanho da grade do jogo
    public const byte Grid = 32;

    // Limites fixos
    public const byte MaxInventory = 30;
    public const byte MaxHotbar = 10;

    // Ataque
    public const short AttackSpeed = 750;

    // Animação
    public const byte AnimationRight = 0;
    public const byte AnimationStopped = 1;
    public const byte AnimationLeft = 2;
    public const byte AnimationAttack = 2;
    public const byte AnimationAmount = 4;

    // Movimentação
    public const byte MovementUp = 3;
    public const byte MovementDown = 0;
    public const byte MovementLeft = 1;
    public const byte MovementRight = 2;

    // Tamanho da tela
    public const short ScreenWidth = Map.Width * Grid;
    public const short ScreenHeight = Map.Height * Grid;

    // Clima
    public const byte MaxRainParticles = 100;
    public const short MaxSnowParticles = 635;
    public const byte MaxWeatherIntensity = 10;
    public const byte SnowMovement = 10;

    // Quantidade de zonas
    public const byte MaxZones = 20;
}