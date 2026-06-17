namespace CryBits.Simulation.Formulas;

/// <summary>
/// Pure, stateless combat calculations. Every method is a static pure function
/// with no side effects — takes data in, returns a result.
/// </summary>
public static class CombatFormulas
{
    public static short BaseDamage(short strength, short weaponDamage)
        => (short)(strength + weaponDamage);

    public static short BaseDefense(short resistance) => resistance;

    public static short NetDamage(short attackerDamage, short victimDefense)
        => (short)Math.Max(0, attackerDamage - victimDefense);
}
