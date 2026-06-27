using System.Text.Json;
using System.Text.Json.Serialization;

namespace CryBits.Definitions.Utils;

public class Array2DConverter<T> : JsonConverter<T[,]>
{
    public override T[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jagged = JsonSerializer.Deserialize<T[][]>(ref reader, options);
        if (jagged == null || jagged.Length == 0) return new T[0, 0];

        var rows = jagged.Length;
        var cols = jagged[0].Length;
        var result = new T[rows, cols];
        for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++)
                result[i, j] = jagged[i][j];

        return result;
    }

    public override void Write(Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options)
    {
        var rows = value.GetLength(0);
        var cols = value.GetLength(1);
        var jagged = new T[rows][];
        for (var i = 0; i < rows; i++)
        {
            jagged[i] = new T[cols];
            for (var j = 0; j < cols; j++)
                jagged[i][j] = value[i, j];
        }

        JsonSerializer.Serialize(writer, jagged, options);
    }
}
