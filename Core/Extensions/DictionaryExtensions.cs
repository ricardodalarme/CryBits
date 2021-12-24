using System.Collections.Generic;

namespace CryBits.Extensions;

public static class DictionaryExtensions
{
    // Obtém o dado, caso ele não existir retorna nulo
    public static TV Get<TGuid, TV>(this Dictionary<TGuid, TV> dict, TGuid key) => dict.ContainsKey(key) ? dict[key] : default;
}