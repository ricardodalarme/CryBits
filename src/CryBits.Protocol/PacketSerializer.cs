using MemoryPack;

namespace CryBits.Protocol;

public static class PacketSerializer
{
    public static byte[] Serialize<T>(T value) => MemoryPackSerializer.Serialize(value);

    public static T Deserialize<T>(byte[] data) where T : class => MemoryPackSerializer.Deserialize<T>(data) ?? throw new InvalidOperationException("Deserialization returned null");
}
