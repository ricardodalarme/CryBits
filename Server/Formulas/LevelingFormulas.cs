using System;

namespace CryBits.Server.Formulas;

/// <summary>
/// Pure, stateless leveling calculations. Every method is a static pure function
/// with no side effects — takes data in, returns a result.
/// </summary>
internal static class LevelingFormulas
{
    /// <summary>
    /// Computes the experience required for the player to reach the next level.
    /// </summary>
    /// <param name="level">The player's current level.</param>
    /// <param name="totalAttributes">Sum of all the player's attribute values.</param>
    /// <param name="points">Number of unspent attribute points.</param>
    public static int ExperienceNeeded(short level, short totalAttributes, byte points)
        => (int)((level + 1) * 2.5 + (totalAttributes + points) / 2);

    /// <summary>
    /// Computes the weight factor for XP distribution to a party member based on
    /// the level difference between the killer and the member.
    /// </summary>
    /// <param name="levelDifference">Absolute level difference between the two players.</param>
    public static double PartyXpWeight(int levelDifference)
    {
        double k;
        if (levelDifference < 3) k = 1.15;
        else if (levelDifference < 6) k = 1.55;
        else if (levelDifference < 10) k = 1.85;
        else k = 2.3;

        return 1 / Math.Pow(k, Math.Min(15, levelDifference));
    }
}
