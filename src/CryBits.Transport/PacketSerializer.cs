using System.Runtime.Serialization.Formatters.Binary;

namespace CryBits.Transport;

public static class PacketSerializer
{
    public static byte[] Serialize(object obj)
    {
#pragma warning disable SYSLIB0011
        var bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011
        using var stream = new MemoryStream();
        bf.Serialize(stream, obj);
        return stream.ToArray();
    }

    public static T Deserialize<T>(byte[] data)
    {
        using var stream = new MemoryStream(data);
#pragma warning disable SYSLIB0011
        return (T)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011
    }
}
