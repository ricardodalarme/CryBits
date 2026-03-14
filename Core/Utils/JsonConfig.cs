using System.Text.Json;

namespace CryBits.Utils;

/// <summary>Shared JSON serializer options used by all read/write operations across the engine.</summary>
public static class JsonConfig
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}
