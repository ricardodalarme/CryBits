namespace CryBits.Definitions;

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

    /// <summary>
    /// Walk speed in world-pixels per second.
    /// </summary>
    public const float WalkSpeedPixelsPerSecond = 60f;

    /// <summary>
    /// Run speed in world-pixels per second.
    /// </summary>
    public const float RunSpeedPixelsPerSecond = 90f;

    /// <summary>Screen width in pixels.</summary>
    public const short ScreenWidth = 800;
    /// <summary>Screen height in pixels.</summary>
    public const short ScreenHeight = 608;

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
