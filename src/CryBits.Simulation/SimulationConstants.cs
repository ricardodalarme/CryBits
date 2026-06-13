namespace CryBits.Simulation;

public static class SimulationConstants
{
    public const int TicksPerSecond = 20;
    public const int AttackSpeedTicks = 15; // 750ms at 20 ticks/sec
    public const int RegenIntervalTicks = 100; // 5000ms at 20 ticks/sec
    public const int GroundItemDespawnTicks = TicksPerSecond * 300; // 5 minutes
}
