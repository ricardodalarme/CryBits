using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LiteNetLib.Utils;

namespace CryBits.Extensions;

public static class NetworkExtensions
{
    public static void PutGuid(this NetDataWriter writer, Guid id) => writer.Put(id.ToByteArray());

    public static Guid GetGuid(this NetDataReader reader)
    {
        var bytes = new byte[16];
        reader.GetBytes(bytes, 0, 16);
        return new Guid(bytes);
    }

    public static void WriteObject(this NetDataWriter data, object obj)
    {
        var bf = new BinaryFormatter();
        using var stream = new MemoryStream();
        bf.Serialize(stream, obj);
        var bytes = stream.ToArray();
        data.Put(bytes.Length);
        data.Put(bytes);
    }

    public static object ReadObject(this NetDataReader data)
    {
        var size = data.GetInt();
        var array = new byte[size];
        data.GetBytes(array, 0, size);

        using var stream = new MemoryStream(array);
        return new BinaryFormatter().Deserialize(stream);
    }
}