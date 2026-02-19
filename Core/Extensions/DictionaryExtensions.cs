using System.Collections.Generic;

namespace CryBits.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Return the value for <paramref name="key"/> or default if the key is not present.
    /// </summary>
    public static TV Get<TGuid, TV>(this Dictionary<TGuid, TV> dict, TGuid key) => dict.ContainsKey(key) ? dict[key] : default;
}