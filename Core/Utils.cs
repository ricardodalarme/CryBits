using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;
using CryBits.Enums;
using Lidgren.Network;

namespace CryBits;

public static class Utils
{
    // Números aleatórios
    public static readonly Random MyRandom = new();

    public static Direction ReverseDirection(Direction direction)
    {
        // Retorna a direção inversa
        switch (direction)
        {
            case Direction.Up: return Direction.Down;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: return Direction.Count;
        }
    }

    public static void NextTile(Direction direction, ref byte x, ref byte y)
    {
        // Próximo azulejo
        switch (direction)
        {
            case Direction.Up: y--; break;
            case Direction.Down: y++; break;
            case Direction.Right: x++; break;
            case Direction.Left: x--; break;
        }
    }

    // Obtém o ID de alguma entidade, caso ela não existir retorna um ID zerado
    public static Guid GetID(this Entity @object) => @object?.ID ?? Guid.Empty;

    // Obtém o dado, caso ele não existir retorna nulo
    public static TV Get<TGuid, TV>(this Dictionary<TGuid, TV> dict, TGuid key) => dict.ContainsKey(key) ? dict[key] : default;

    public static void Write(this NetBuffer buffer, Guid id) => buffer.Write(id.ToString());
    public static void Write(this BinaryWriter buffer, Guid id) => buffer.Write(id.ToString());

    public static void ObjectToByteArray(NetOutgoingMessage data, object obj)
    {
        var bf = new BinaryFormatter();
        using var stream = new MemoryStream();
        bf.Serialize(stream, obj);
        data.Write(stream.ToArray().Length);
        data.Write(stream.ToArray());
    }

    public static object ByteArrayToObject(NetBuffer data)
    {
        var size = data.ReadInt32();
        var array = data.ReadBytes(size);

        using var stream = new MemoryStream(array);
        return new BinaryFormatter().Deserialize(stream);
    }
}