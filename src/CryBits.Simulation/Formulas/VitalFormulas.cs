using CryBits.Definitions.Characters;

namespace CryBits.Simulation.Formulas;

/// <summary>
/// Pure, stateless vital (HP/MP) calculations. Every method is a static pure function
/// with no side effects — takes data in, returns a result.
/// </summary>
public static class VitalFormulas
{
    /// <summary>Computes the maximum value of a vital for a player.</summary>
    /// <param name="vital">Which vital to compute (HP or MP).</param>
    /// <param name="baseVital">The base vital value from the player's class.</param>
    /// <param name="vitality">The player's Vitality attribute.</param>
    /// <param name="intelligence">The player's Intelligence attribute.</param>
    /// <param name="level">The player's current level.</param>
    public static short MaxVital(Vital vital, short baseVital, short vitality, short intelligence, short level)
    {
        return vital switch
        {
            Vital.Hp => (short)(baseVital + (vitality * level * 9 / 8) + 1),
            Vital.Mp => (short)(baseVital + (intelligence * level * 5 / 8) + 1),
            _ => 1
        };
    }

    public static short VitalRegeneration(Vital vital, short maxVital, short vitality, short intelligence)
    {
        return vital switch
        {
            Vital.Hp => (short)((maxVital / 20) + (vitality * 3 / 10)),
            Vital.Mp => (short)((maxVital / 20) + (intelligence / 10)),
            _ => 1
        };
    }
}
