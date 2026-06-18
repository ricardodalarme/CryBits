using MemoryPack;

namespace CryBits.Protocol;

public static class PacketSerializer
{
    public static byte[] Serialize<T>(T value) => MemoryPackSerializer.Serialize(value);

    public static T Deserialize<T>(byte[] data) => MemoryPackSerializer.Deserialize<T>(data);
}
