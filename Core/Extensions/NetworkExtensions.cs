using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LiteNetLib.Utils;

namespace CryBits.Extensions;

public static class NetworkExtensions
{
    public static void WriteObject(this NetDataWriter data, object obj)
    {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        var bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011 // Type or member is obsolete
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
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        return new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
    }
}
