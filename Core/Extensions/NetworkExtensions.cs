using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Lidgren.Network;

namespace CryBits.Extensions;

public static class NetworkExtensions
{
    public static void Write(this NetBuffer buffer, Guid id) => buffer.Write(id.ToString());

    public static void WriteObject(this NetOutgoingMessage data, object obj) 
    {
        var bf = new BinaryFormatter();
        using var stream = new MemoryStream();
        bf.Serialize(stream, obj);
        data.Write(stream.ToArray().Length);
        data.Write(stream.ToArray());
    }

    public static object ReadObject(this NetBuffer data)
    {
        var size = data.ReadInt32();
        var array = data.ReadBytes(size);

        using var stream = new MemoryStream(array);
        return new BinaryFormatter().Deserialize(stream);
    }
}