using CryBits.Enums;

namespace CryBits.Server.Formulas;

/// <summary>
/// Pure, stateless vital (HP/MP) calculations. Every method is a static pure function
/// with no side effects — takes data in, returns a result.
/// </summary>
internal static class VitalFormulas
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
            Vital.Hp => (short)(baseVital + vitality * 1.50 * (level * 0.75) + 1),
            Vital.Mp => (short)(baseVital + intelligence * 1.25 * (level * 0.5) + 1),
            _ => 1
        };
    }

    /// <summary>Computes the regeneration amount for a player vital.</summary>
    /// <param name="vital">Which vital to compute (HP or MP).</param>
    /// <param name="maxVital">The player's current max value for this vital.</param>
    /// <param name="vitality">The player's Vitality attribute.</param>
    /// <param name="intelligence">The player's Intelligence attribute.</param>
    public static short PlayerRegeneration(Vital vital, short maxVital, short vitality, short intelligence)
    {
        return vital switch
        {
            Vital.Hp => (short)(maxVital * 0.05 + vitality * 0.3),
            Vital.Mp => (short)(maxVital * 0.05 + intelligence * 0.1),
            _ => 1
        };
    }

    /// <summary>Computes the regeneration amount for an NPC vital.</summary>
    /// <param name="vital">Which vital to compute (HP or MP).</param>
    /// <param name="npcBaseVital">The NPC's base vital value from its definition.</param>
    /// <param name="npcVitality">The NPC's Vitality attribute.</param>
    /// <param name="npcIntelligence">The NPC's Intelligence attribute.</param>
    public static short NpcRegeneration(Vital vital, short npcBaseVital, short npcVitality, short npcIntelligence)
    {
        return vital switch
        {
            Vital.Hp => (short)(npcBaseVital * 0.05 + npcVitality * 0.3),
            Vital.Mp => (short)(npcBaseVital * 0.05 + npcIntelligence * 0.1),
            _ => 0
        };
    }
}
