using CryBits.Entities.Map;

namespace CryBits;

public static class Globals
{
    /// <summary>Runtime server configuration loaded from <c>settings.json</c>.</summary>
    public static ServerConfig Config { get; set; } = new();

    /// <summary>Game grid size in pixels.</summary>
    public const byte Grid = 32;

    /// <summary>Maximum number of inventory slots per player.</summary>
    public const byte MaxInventory = 30;
    /// <summary>Maximum number of hotbar slots.</summary>
    public const byte MaxHotbar = 10;

    /// <summary>Attack cooldown (milliseconds).</summary>
    public const short AttackSpeed = 750;

    /// <summary>Animation row index for right-facing frames.</summary>
    public const byte AnimationRight = 0;
    /// <summary>Animation row index for stopped frame.</summary>
    public const byte AnimationStopped = 1;
    /// <summary>Animation row index for left-facing frames.</summary>
    public const byte AnimationLeft = 2;
    /// <summary>Animation row index used for attack frame.</summary>
    public const byte AnimationAttack = 2;
    /// <summary>Number of animation frames per axis.</summary>
    public const byte AnimationAmount = 4;

    /// <summary>Movement direction constants (tile offsets).</summary>
    public const byte MovementUp = 3;
    public const byte MovementDown = 0;
    public const byte MovementLeft = 1;
    public const byte MovementRight = 2;

    /// <summary>Screen width in pixels (Map.Width * Grid).</summary>
    public const short ScreenWidth = Map.Width * Grid;
    /// <summary>Screen height in pixels (Map.Height * Grid).</summary>
    public const short ScreenHeight = Map.Height * Grid;

    /// <summary>Maximum rain particle count for weather effects.</summary>
    public const byte MaxRainParticles = 100;
    /// <summary>Maximum snow particle count for weather effects.</summary>
    public const short MaxSnowParticles = 635;
    /// <summary>Maximum supported weather intensity.</summary>
    public const byte MaxWeatherIntensity = 10;
    /// <summary>Horizontal snow movement amplitude.</summary>
    public const byte SnowMovement = 10;

    /// <summary>Maximum number of zone definitions.</summary>
    public const byte MaxZones = 20;
}