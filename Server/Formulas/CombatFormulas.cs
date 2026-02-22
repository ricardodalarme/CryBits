using System;

namespace CryBits.Server.Formulas;

/// <summary>
/// Pure, stateless combat calculations. Every method is a static pure function
/// with no side effects — takes data in, returns a result.
/// </summary>
internal static class CombatFormulas
{
    /// <summary>Computes a player's total damage output (strength + weapon bonus).</summary>
    /// <param name="strength">The player's Strength attribute value.</param>
    /// <param name="weaponDamage">Weapon damage bonus, or 0 if unarmed.</param>
    public static short PlayerDamage(short strength, short weaponDamage)
        => (short)(strength + weaponDamage);

    /// <summary>Computes a player's defense value from their Resistance attribute.</summary>
    /// <param name="resistance">The player's Resistance attribute value.</param>
    public static short PlayerDefense(short resistance) => resistance;

    /// <summary>
    /// Computes the net damage dealt after subtracting the victim's defense.
    /// Returns zero when defense exceeds attack (no negative damage).
    /// </summary>
    /// <param name="attackerDamage">Total attack damage of the attacker.</param>
    /// <param name="victimDefense">Total defense of the victim.</param>
    public static short NetDamage(short attackerDamage, short victimDefense)
        => (short)Math.Max(0, attackerDamage - victimDefense);
}
