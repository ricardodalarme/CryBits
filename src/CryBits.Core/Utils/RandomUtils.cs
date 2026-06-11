using System;

namespace CryBits.Utils;

public static class RandomUtils
{
    /// <summary>Shared <see cref="Random"/> instance used across the project.</summary>
    public static readonly Random MyRandom = new();
}
